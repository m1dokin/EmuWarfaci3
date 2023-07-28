using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.SponsorSystem;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200035C RID: 860
	[Service]
	[Singleton]
	internal class ItemsReimbursement : ServiceModule, IItemsReimbursement
	{
		// Token: 0x06001348 RID: 4936 RVA: 0x0004EE2E File Offset: 0x0004D22E
		public ItemsReimbursement(IDBUpdateService dbUpdateService, IProfileItems profileItems, ICatalogService catalogService, IItemCache itemCache, ISponsorUnlock sponsorUnlock, ILogService logService, IDALService dalService)
		{
			this.m_dbUpdateService = dbUpdateService;
			this.m_profileItems = profileItems;
			this.m_catalogService = catalogService;
			this.m_itemCache = itemCache;
			this.m_sponsorUnlock = sponsorUnlock;
			this.m_logService = logService;
			this.m_dalService = dalService;
		}

		// Token: 0x14000040 RID: 64
		// (add) Token: 0x06001349 RID: 4937 RVA: 0x0004EE6C File Offset: 0x0004D26C
		// (remove) Token: 0x0600134A RID: 4938 RVA: 0x0004EEA4 File Offset: 0x0004D2A4
		public event ReimbursementItemsUpdatedDelegate ReimbursementItemsUpdated;

		// Token: 0x0600134B RID: 4939 RVA: 0x0004EEDA File Offset: 0x0004D2DA
		public override void Init()
		{
		}

		// Token: 0x0600134C RID: 4940 RVA: 0x0004EEDC File Offset: 0x0004D2DC
		public override void Start()
		{
			string path = Path.Combine(Resources.GetResourcesDirectory(), "items_to_reimburse.xml");
			using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				this.ReadConfig(fileStream);
				this.CheckConsistency();
			}
		}

		// Token: 0x0600134D RID: 4941 RVA: 0x0004EF34 File Offset: 0x0004D334
		public override void Stop()
		{
		}

		// Token: 0x0600134E RID: 4942 RVA: 0x0004EF38 File Offset: 0x0004D338
		public List<string> ProcessReimbursement(ulong user_id, ulong profile_id)
		{
			List<string> list = new List<string>();
			Dictionary<ulong, CustomerItem> inactiveCustomerItems = this.m_catalogService.GetInactiveCustomerItems(user_id);
			Dictionary<ulong, SProfileItem> profileItems = this.m_profileItems.GetProfileItems(profile_id, EquipOptions.All);
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profile_id);
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				foreach (ItemToReimburse itemToReimburse in this.m_items)
				{
					using (Dictionary<ulong, CustomerItem>.ValueCollection.Enumerator enumerator2 = inactiveCustomerItems.Values.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							CustomerItem customerItem = enumerator2.Current;
							string name = customerItem.CatalogItem.Name;
							if (itemToReimburse.name == name)
							{
								foreach (KeyValuePair<ulong, SProfileItem> keyValuePair in from p in profileItems
								where p.Value.CatalogID == customerItem.InstanceID
								select p)
								{
									this.m_profileItems.DeleteProfileItem(profile_id, keyValuePair.Value.ProfileItemID);
									this.m_profileItems.LockItem(profile_id, keyValuePair.Value.ItemID);
									logGroup.ItemLockedLog(user_id, profile_id, keyValuePair.Value.ItemID, keyValuePair.Value.GameItem.Name);
								}
								this.m_catalogService.AddMoney(user_id, itemToReimburse.currency, itemToReimburse.moneyAmount, string.Empty);
								this.m_catalogService.DeleteCustomerItem(user_id, customerItem.InstanceID);
								logGroup.ItemDestroyLog(user_id, profile_id, profileInfo.Nickname, profileInfo.RankInfo.RankId, customerItem.CatalogItem.ID, customerItem.InstanceID, customerItem.CatalogItem.Type, customerItem.CatalogItem.Name, 0, string.Empty);
								logGroup.ShopMoneyChangedLog(user_id, profile_id, (long)((itemToReimburse.currency != Currency.GameMoney) ? 0UL : itemToReimburse.moneyAmount), (long)((itemToReimburse.currency != Currency.CryMoney) ? 0UL : itemToReimburse.moneyAmount), (long)((itemToReimburse.currency != Currency.CrownMoney) ? 0UL : itemToReimburse.moneyAmount), LogGroup.ProduceType.Reimbursement, TransactionStatus.OK, string.Empty, string.Empty);
								list.Add(itemToReimburse.message);
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x0600134F RID: 4943 RVA: 0x0004F250 File Offset: 0x0004D650
		public List<ItemToReimburse> GetItemsToReimburse()
		{
			return this.m_items;
		}

		// Token: 0x06001350 RID: 4944 RVA: 0x0004F258 File Offset: 0x0004D658
		private void CheckConsistency()
		{
			Dictionary<ulong, SItem>.ValueCollection values = this.m_itemCache.GetAllItems().Values;
			Dictionary<string, CatalogItem> catalogItems = this.m_catalogService.GetCatalogItems();
			using (List<ItemToReimburse>.Enumerator enumerator = this.m_items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ItemToReimburse itemToReimburse = enumerator.Current;
					if (catalogItems.Values.Count((CatalogItem item) => item.Name == itemToReimburse.name) > 0)
					{
						throw new ApplicationException(string.Format("Item {0} is present in e-catalog, but marked to reimburse", itemToReimburse.name));
					}
					if (this.m_sponsorUnlock.IsItemPresentInSponsors(itemToReimburse.name))
					{
						throw new ApplicationException(string.Format("Item {0} is present in sponsors, but marked to reimburse", itemToReimburse.name));
					}
					if (values.Count((SItem item) => item.Name == itemToReimburse.name) > 0)
					{
						throw new ApplicationException(string.Format("Item {0} is present in game database, but marked to reimburse", itemToReimburse.name));
					}
				}
			}
		}

		// Token: 0x06001351 RID: 4945 RVA: 0x0004F378 File Offset: 0x0004D778
		private void ReadConfig(Stream config)
		{
			List<ItemToReimburse> list = new List<ItemToReimburse>();
			XmlTextReader xmlTextReader = new XmlTextReader(config);
			if (xmlTextReader.Read())
			{
				while (xmlTextReader.Read())
				{
					if (xmlTextReader.NodeType == XmlNodeType.Element)
					{
						if (xmlTextReader.Name == "item")
						{
							string attribute = xmlTextReader.GetAttribute("name");
							string attribute2 = xmlTextReader.GetAttribute("message");
							string attribute3 = xmlTextReader.GetAttribute("currency");
							Currency currency = (Currency)Enum.Parse(typeof(Currency), attribute3, true);
							ulong moneyAmount = ulong.Parse(xmlTextReader.GetAttribute("money_amount"));
							list.Add(new ItemToReimburse(attribute, attribute2, currency, moneyAmount));
						}
					}
				}
			}
			this.m_items = list;
			if (this.ReimbursementItemsUpdated != null)
			{
				this.ReimbursementItemsUpdated();
			}
		}

		// Token: 0x040008EF RID: 2287
		private readonly IDBUpdateService m_dbUpdateService;

		// Token: 0x040008F0 RID: 2288
		private readonly IProfileItems m_profileItems;

		// Token: 0x040008F1 RID: 2289
		private readonly ICatalogService m_catalogService;

		// Token: 0x040008F2 RID: 2290
		private readonly IItemCache m_itemCache;

		// Token: 0x040008F3 RID: 2291
		private readonly ISponsorUnlock m_sponsorUnlock;

		// Token: 0x040008F4 RID: 2292
		private readonly ILogService m_logService;

		// Token: 0x040008F5 RID: 2293
		private readonly IDALService m_dalService;

		// Token: 0x040008F7 RID: 2295
		private List<ItemToReimburse> m_items;
	}
}
