using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200037E RID: 894
	public static class ServerItem
	{
		// Token: 0x06001441 RID: 5185 RVA: 0x00052554 File Offset: 0x00050954
		public static XmlElement GetXml(SItem item, XmlDocument factory, string tagName)
		{
			if (!item.Active)
			{
				Log.Error<string>("Serializing inactive item '{0}'", item.Name);
			}
			XmlElement xmlElement = factory.CreateElement(tagName);
			xmlElement.SetAttribute("id", item.ID.ToString());
			xmlElement.SetAttribute("name", item.Name);
			xmlElement.SetAttribute("locked", (!item.Locked) ? "0" : "1");
			xmlElement.SetAttribute("max_buy_amount", item.MaxAmount.ToString());
			return xmlElement;
		}

		// Token: 0x06001442 RID: 5186 RVA: 0x000525F4 File Offset: 0x000509F4
		public static XmlElement GetXml(SProfileItem item, XmlDocument factory, string tagName)
		{
			XmlElement xmlElement = factory.CreateElement(tagName);
			xmlElement.SetAttribute("id", item.ProfileItemID.ToString());
			xmlElement.SetAttribute("name", item.GameItem.Name);
			xmlElement.SetAttribute("attached_to", item.AttachedTo.ToString());
			xmlElement.SetAttribute("config", item.Config);
			xmlElement.SetAttribute("slot", ((!item.IsExpired) ? item.SlotIDs : 0UL).ToString());
			xmlElement.SetAttribute("equipped", ((!item.IsExpired) ? EquipCheck.ConvertSlotIdsToEquipped(item.SlotIDs) : 0UL).ToString());
			xmlElement.SetAttribute("default", (!item.IsDefault) ? "0" : "1");
			xmlElement.SetAttribute("permanent", (item.OfferType != OfferType.Permanent) ? "0" : "1");
			xmlElement.SetAttribute("expired_confirmed", ((!item.IsExpired) ? 0 : 1).ToString());
			ulong num = 0UL;
			if (EquipCheck.ParseDecalClasses(item.Config, out num))
			{
				xmlElement.SetAttribute("equipped", num.ToString());
			}
			xmlElement.SetAttribute("buy_time_utc", item.BuyTimeUTC.ToString());
			if (item.OfferType == OfferType.Expiration)
			{
				xmlElement.SetAttribute("expiration_time_utc", item.ExpirationTimeUTC.ToString());
				xmlElement.SetAttribute("seconds_left", item.SecondsLeft.ToString());
			}
			else if (item.OfferType == OfferType.Permanent)
			{
				xmlElement.SetAttribute("total_durability_points", item.TotalDurabilityPoints.ToString());
				xmlElement.SetAttribute("durability_points", item.DurabilityPoints.ToString());
			}
			else if (item.OfferType == OfferType.Consumable)
			{
				xmlElement.SetAttribute("quantity", item.Quantity.ToString());
			}
			return xmlElement;
		}
	}
}
