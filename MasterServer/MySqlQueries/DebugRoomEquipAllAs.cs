using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.MySqlQueries
{
	// Token: 0x02000830 RID: 2096
	[ConsoleCmdAttributes(CmdName = "debug_equip_all_as", ArgsSize = 2)]
	internal class DebugRoomEquipAllAs : IConsoleCmd
	{
		// Token: 0x06002B68 RID: 11112 RVA: 0x000BB7B8 File Offset: 0x000B9BB8
		public DebugRoomEquipAllAs(IDALService dalService, ICatalogService eCatService, IUserRepository userRepository, IProfileItems profileItems, IGameRoomManager gameRoomManager, IItemsPurchase itemsPurchase, IItemStats itemStats, IItemCache itemCache, IItemRepairDescriptionRepository repairRepository)
		{
			this.m_dalService = dalService;
			this.m_eCatService = eCatService;
			this.m_userRepository = userRepository;
			this.m_profileItems = profileItems;
			this.m_gameRoomManager = gameRoomManager;
			this.m_itemPurchase = itemsPurchase;
			this.m_itemStats = itemStats;
			this.m_itemCache = itemCache;
			this.m_repairRepository = repairRepository;
		}

		// Token: 0x06002B69 RID: 11113 RVA: 0x000BB810 File Offset: 0x000B9C10
		public void ExecuteCmd(string[] args)
		{
			if (args.Length != 2)
			{
				Log.Warning("Incorrect input params. Use \"debug_equip_all_as profile_id\".");
				return;
			}
			ulong profileId;
			if (!ulong.TryParse(args[1], out profileId))
			{
				Log.Warning("Unable to parse profile_id parameter. Check the value is valid.");
				return;
			}
			IGameRoom room = this.m_gameRoomManager.GetRoomByPlayer(profileId);
			if (room == null)
			{
				Log.Warning("Unable to find game room.");
				return;
			}
			UserInfo.User user = this.m_userRepository.GetUser(profileId);
			if (user == null)
			{
				Log.Warning<ulong>("There is no such user with profile_id = {0} thar is logged in", profileId);
				return;
			}
			Dictionary<ulong, SProfileItem>.ValueCollection templateItems = this.m_profileItems.GetProfileItems(profileId, EquipOptions.All).Values;
			Dictionary<ulong, CustomerItem>.ValueCollection templateCustomerItems = this.m_eCatService.GetCustomerItems(user.UserID).Values;
			room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				foreach (RoomPlayer roomPlayer in room.Players)
				{
					if (roomPlayer.ProfileID != profileId)
					{
						this.m_dalService.ItemSystem.DebugUnlockAllItems(roomPlayer.ProfileID);
						UserInfo.User user2 = this.m_userRepository.GetUser(roomPlayer.ProfileID);
						using (Dictionary<ulong, CustomerItem>.ValueCollection.Enumerator enumerator2 = templateCustomerItems.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								CustomerItem templateCustomerItem = enumerator2.Current;
								CatalogItem item = this.m_eCatService.GetCatalogItems().Values.First((CatalogItem x) => x.Name == templateCustomerItem.CatalogItem.Name);
								OfferItem item2 = new OfferItem
								{
									Item = item,
									ExpirationTime = ((templateCustomerItem.ExpirationTimeUTC != 0UL) ? TimeUtils.UTCTimestampToTimeSpan(templateCustomerItem.ExpirationTimeUTC - templateCustomerItem.BuyTimeUTC - (ulong)DateTime.UtcNow.TimeOfDay.TotalSeconds) : TimeSpan.FromSeconds(0.0)),
									Quantity = templateCustomerItem.Quantity
								};
								if (templateCustomerItem.OfferType == OfferType.Permanent)
								{
									item2.DurabilityPoints = templateCustomerItem.TotalDurabilityPoints;
									SItem sitem;
									if (!this.m_itemCache.GetAllItemsByName().TryGetValue(templateCustomerItem.CatalogItem.Name, out sitem))
									{
										throw new ApplicationException(string.Format("Unknown item {0}", templateCustomerItem.CatalogItem.Name));
									}
									SRepairItemDesc srepairItemDesc;
									if (!this.m_repairRepository.GetRepairItemDesc(sitem.ID, item.ID, out srepairItemDesc))
									{
										throw new ApplicationException(string.Format("Can't give item {0}! Item is not permanent.", templateCustomerItem.CatalogItem.Name));
									}
									item2.RepairCost = srepairItemDesc.RepairCost.ToString();
								}
								this.m_eCatService.AddCustomerItem(user2.UserID, item2, false);
							}
						}
						this.m_itemPurchase.SyncProfileItemsWithCatalog(new ProfileProxy(roomPlayer.ProfileID));
						Dictionary<ulong, SProfileItem>.ValueCollection values = this.m_profileItems.GetProfileItems(roomPlayer.ProfileID, EquipOptions.All).Values;
						using (Dictionary<ulong, SProfileItem>.ValueCollection.Enumerator enumerator3 = templateItems.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								SProfileItem templateItem = enumerator3.Current;
								SProfileItem sprofileItem = values.FirstOrDefault((SProfileItem d) => d.ItemID == templateItem.ItemID);
								if (sprofileItem != null)
								{
									this.m_dalService.ItemSystem.UpdateProfileItem(roomPlayer.ProfileID, sprofileItem.ProfileItemID, templateItem.SlotIDs, sprofileItem.AttachedTo, sprofileItem.Config);
								}
							}
						}
					}
				}
			});
		}

		// Token: 0x0400171B RID: 5915
		private readonly IDALService m_dalService;

		// Token: 0x0400171C RID: 5916
		private readonly ICatalogService m_eCatService;

		// Token: 0x0400171D RID: 5917
		private readonly IUserRepository m_userRepository;

		// Token: 0x0400171E RID: 5918
		private readonly IProfileItems m_profileItems;

		// Token: 0x0400171F RID: 5919
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04001720 RID: 5920
		private readonly IItemsPurchase m_itemPurchase;

		// Token: 0x04001721 RID: 5921
		private readonly IItemStats m_itemStats;

		// Token: 0x04001722 RID: 5922
		private readonly IItemCache m_itemCache;

		// Token: 0x04001723 RID: 5923
		private readonly IItemRepairDescriptionRepository m_repairRepository;
	}
}
