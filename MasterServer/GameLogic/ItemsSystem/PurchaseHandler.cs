using System;
using System.Globalization;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200037B RID: 891
	internal class PurchaseHandler : IPurchaseListener
	{
		// Token: 0x06001431 RID: 5169 RVA: 0x0002FFE7 File Offset: 0x0002E3E7
		public PurchaseHandler(ulong profileId, XmlElement parentNode)
		{
			this.m_profileId = profileId;
			this.m_outputNode = parentNode.OwnerDocument.CreateElement("purchased_item");
			parentNode.AppendChild(this.m_outputNode);
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x0003001C File Offset: 0x0002E41C
		public void HandleProfileItem(SPurchasedItem item, StoreOffer offer)
		{
			XmlElement xmlElement = this.CreateItemNode("profile_item");
			xmlElement.SetAttribute("name", item.Item.Name);
			xmlElement.SetAttribute("profile_item_id", item.ProfileItemID.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("offerId", ((offer == null) ? 0UL : offer.StoreID).ToString(CultureInfo.InvariantCulture));
			if (!string.IsNullOrEmpty(item.AddedExpiration))
			{
				xmlElement.SetAttribute("added_expiration", item.AddedExpiration);
			}
			xmlElement.SetAttribute("added_quantity", item.AddedQuantity.ToString(CultureInfo.InvariantCulture));
			XmlElement xmlElement2 = xmlElement;
			string name = "error_status";
			int status = (int)item.Status;
			xmlElement2.SetAttribute(name, status.ToString(CultureInfo.InvariantCulture));
			if (item.Status == TransactionStatus.OK)
			{
				IProfileItems service = ServicesManager.GetService<IProfileItems>();
				SProfileItem profileItem = service.GetProfileItem(this.m_profileId, item.ProfileItemID);
				if (profileItem == null)
				{
					Log.Info(string.Format("ProfileItem is null! ProfileId: {0}; ProfileItemId: {1}; PurchasedItemTrasactionStatus: {2}; StoreId: {3};CatalogItemId: {4}; CatalogItemName: {5}", new object[]
					{
						this.m_profileId,
						item.ProfileItemID,
						item.Status,
						(offer == null) ? 0UL : offer.StoreID,
						(offer == null) ? 0UL : offer.Content.Item.ID,
						(offer == null) ? "offer == null" : offer.Content.Item.Name
					}));
				}
				XmlElement xml = ServerItem.GetXml(profileItem, this.m_outputNode.OwnerDocument, "item");
				xmlElement.AppendChild(xml);
				IItemStats service2 = ServicesManager.GetService<IItemStats>();
				StackableItemStats stackableItemStats = service2.GetStackableItemStats(item.Item.ID);
				if (stackableItemStats != null && stackableItemStats.IsStackable && item.Item.Type == "coin")
				{
					this.OnConsumableChanged(item);
				}
			}
			this.m_outputNode.AppendChild(xmlElement);
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x00030244 File Offset: 0x0002E644
		public void HandleMetaGameItem(SPurchasedItem item, StoreOffer offer)
		{
			XmlElement xmlElement = this.CreateItemNode("meta_game_item");
			xmlElement.SetAttribute("name", item.Item.Name);
			xmlElement.SetAttribute("profile_item_id", item.ProfileItemID.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("offerId", ((offer == null) ? 0UL : offer.StoreID).ToString(CultureInfo.InvariantCulture));
			this.m_outputNode.AppendChild(xmlElement);
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x000302C8 File Offset: 0x0002E6C8
		public virtual SNotification CreateNotification(OfferItem givenItem, string message = "", bool notify = true)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x000302D0 File Offset: 0x0002E6D0
		public void HandleExperience(SItem item, ulong added, SRankInfo total, StoreOffer offer)
		{
			XmlElement xmlElement = this.CreateItemNode("exp");
			xmlElement.SetAttribute("name", item.Name);
			xmlElement.SetAttribute("added", added.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("total", total.Points.ToString(CultureInfo.InvariantCulture));
			if (offer != null)
			{
				xmlElement.SetAttribute("offerId", offer.StoreID.ToString(CultureInfo.InvariantCulture));
			}
			this.m_outputNode.AppendChild(xmlElement);
		}

		// Token: 0x06001436 RID: 5174 RVA: 0x00030360 File Offset: 0x0002E760
		public void HandleMoney(SItem item, Currency currency, ulong added, ulong total, StoreOffer offer)
		{
			string type;
			switch (currency)
			{
			case Currency.GameMoney:
				type = "game_money";
				break;
			case Currency.CryMoney:
				type = "cry_money";
				break;
			case Currency.CrownMoney:
				type = "crown_money";
				break;
			default:
				throw new Exception("Unsupported currency " + currency.ToString());
			}
			XmlElement xmlElement = this.CreateItemNode(type);
			xmlElement.SetAttribute("name", item.Name);
			xmlElement.SetAttribute("added", added.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("total", total.ToString(CultureInfo.InvariantCulture));
			if (offer != null)
			{
				xmlElement.SetAttribute("offerId", offer.StoreID.ToString(CultureInfo.InvariantCulture));
			}
			this.m_outputNode.AppendChild(xmlElement);
		}

		// Token: 0x06001437 RID: 5175 RVA: 0x0003043C File Offset: 0x0002E83C
		private XmlElement CreateItemNode(string type)
		{
			return this.m_outputNode.OwnerDocument.CreateElement(type);
		}

		// Token: 0x06001438 RID: 5176 RVA: 0x0003045C File Offset: 0x0002E85C
		private void OnConsumableChanged(SPurchasedItem item)
		{
			IGameRoomManager service = ServicesManager.GetService<IGameRoomManager>();
			IGameRoom roomByPlayer = service.GetRoomByPlayer(this.m_profileId);
			if (roomByPlayer != null)
			{
				string serverJid = string.Empty;
				string sessionId = string.Empty;
				roomByPlayer.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
				{
					ServerExtension extension = r.GetExtension<ServerExtension>();
					if (extension.GameRunning)
					{
						serverJid = extension.ServerOnlineID;
						SessionExtension extension2 = r.GetExtension<SessionExtension>();
						sessionId = extension2.SessionID;
					}
				});
				if (!string.IsNullOrEmpty(serverJid))
				{
					QueryManager.RequestSt("shop_sync_consumables", serverJid, new object[]
					{
						sessionId,
						this.m_profileId,
						item.ProfileItemID,
						item.AddedQuantity
					});
				}
			}
		}

		// Token: 0x04000958 RID: 2392
		private readonly ulong m_profileId;

		// Token: 0x04000959 RID: 2393
		private readonly XmlElement m_outputNode;
	}
}
