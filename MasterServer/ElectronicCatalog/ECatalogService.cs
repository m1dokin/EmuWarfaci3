using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.DAL;
using MasterServer.DAL.Exceptions;
using MasterServer.DAL.Utils;
using MasterServer.Database;
using MasterServer.ElectronicCatalog.Exceptions;
using MasterServer.ElectronicCatalog.ShopSupplier;
using MasterServer.GameLogic.ItemsSystem.Regular;
using MasterServer.Platform.Payment;
using MasterServer.Platform.Payment.Exceptions;
using MasterServer.Users;
using Ninject;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000245 RID: 581
	[Service]
	[Singleton]
	internal class ECatalogService : ServiceModule, ICatalogService, IDebugCatalogService, IDBUpdateListener
	{
		// Token: 0x06000C89 RID: 3209 RVA: 0x0003082C File Offset: 0x0002EC2C
		public ECatalogService(IJobSchedulerService jobSchedulerService, IDALService dalService, IShopService shopService, [Optional] IPaymentService paymentService, [Optional] IDebugPaymentService debugPaymentService, IDBUpdateService dbUpdateService, IECatalogOffersUpdater catalogOffersUpdater, IConfigProvider<RegularItemConfig> regularItemConfigProvider)
		{
			this.m_jobSchedulerService = jobSchedulerService;
			this.m_dalService = dalService;
			this.m_paymentService = paymentService;
			this.m_debugPaymentService = debugPaymentService;
			this.m_dbUpdateService = dbUpdateService;
			this.m_catalogOffersUpdater = catalogOffersUpdater;
			this.m_shopService = shopService;
			this.m_regularItemConfigProvider = regularItemConfigProvider;
			if (this.m_paymentService != null)
			{
				this.m_paymentCallback = new ECatalogService.PaymentCallback(this.m_paymentService, this);
			}
			if (Resources.RealmDBUpdaterPermission)
			{
				this.m_dbUpdateService.RegisterListener(this);
				ServicesManager.OnExecutionPhaseChanged += this.OnExecutionPhaseChanged;
			}
		}

		// Token: 0x14000028 RID: 40
		// (add) Token: 0x06000C8A RID: 3210 RVA: 0x000308E4 File Offset: 0x0002ECE4
		// (remove) Token: 0x06000C8B RID: 3211 RVA: 0x0003091C File Offset: 0x0002ED1C
		public event CatalogItemsUpdatedDelegate CatalogItemsUpdated;

		// Token: 0x06000C8C RID: 3212 RVA: 0x00030954 File Offset: 0x0002ED54
		public override void Start()
		{
			base.Start();
			ConfigSection section = Resources.ECatalogSettings.GetSection("ECatalog");
			section.OnConfigChanged += this.OnConfigChanged;
			int cacheResetTimeoutMin;
			int.TryParse(section.Get("CacheResetTimeoutMin"), out cacheResetTimeoutMin);
			section.TryGet("cash_transaction_time_min", out this.m_cashTransactionTime, default(TimeSpan));
			this.ConfigureCacheTimeout(cacheResetTimeoutMin);
			if (Resources.RealmDBUpdaterPermission)
			{
				this.m_jobSchedulerService.AddJob("ecat_backup_logs");
			}
		}

		// Token: 0x06000C8D RID: 3213 RVA: 0x000309D8 File Offset: 0x0002EDD8
		public override void Stop()
		{
			if (Resources.RealmDBUpdaterPermission)
			{
				this.m_dbUpdateService.UnregisterListener(this);
				ServicesManager.OnExecutionPhaseChanged -= this.OnExecutionPhaseChanged;
			}
			base.Stop();
		}

		// Token: 0x06000C8E RID: 3214 RVA: 0x00030A08 File Offset: 0x0002EE08
		private void CheckCache()
		{
			if (this.m_catalogItems.Count == 0 || (this.m_cachingTime != TimeSpan.Zero && DateTime.Now - this.m_lastCacheUpdate > this.m_cachingTime))
			{
				this.UpdateCache();
			}
		}

		// Token: 0x06000C8F RID: 3215 RVA: 0x00030A60 File Offset: 0x0002EE60
		public void UpdateCache()
		{
			this.m_catalogItems = (from x in this.m_dalService.ECatalog.GetCatalogItems()
			where x.Active
			select x).ToDictionary((CatalogItem x) => x.Name);
			this.m_lastCacheUpdate = DateTime.Now;
		}

		// Token: 0x06000C90 RID: 3216 RVA: 0x00030AD2 File Offset: 0x0002EED2
		public IEnumerable<StoreOffer> GetStoreOffers()
		{
			return this.m_shopService.GetOffers();
		}

		// Token: 0x06000C91 RID: 3217 RVA: 0x00030ADF File Offset: 0x0002EEDF
		public StoreOffer GetStoreOfferById(int supplierId, ulong offerId)
		{
			return this.m_shopService.GetStoreOfferById(supplierId, offerId);
		}

		// Token: 0x06000C92 RID: 3218 RVA: 0x00030AF0 File Offset: 0x0002EEF0
		public Dictionary<string, CatalogItem> GetCatalogItems()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.CheckCache();
			}
			return this.m_catalogItems;
		}

		// Token: 0x06000C93 RID: 3219 RVA: 0x00030B3C File Offset: 0x0002EF3C
		public bool TryGetCatalogItem(string itemName, out CatalogItem item)
		{
			foreach (CatalogItem catalogItem in this.m_dalService.ECatalog.GetCatalogItems())
			{
				if (catalogItem.Active && catalogItem.Name == itemName)
				{
					item = catalogItem;
					return true;
				}
			}
			item = default(CatalogItem);
			return false;
		}

		// Token: 0x06000C94 RID: 3220 RVA: 0x00030BD0 File Offset: 0x0002EFD0
		private Dictionary<ulong, CustomerItem> GetCustomerItemsInternal(ulong customerId, Predicate<CustomerItem> searchFunc)
		{
			IEnumerable<CustomerItem> customerItems = this.m_dalService.ECatalog.GetCustomerItems(customerId);
			return (from item in customerItems
			where searchFunc(item)
			select item).ToDictionary((CustomerItem item) => item.InstanceID);
		}

		// Token: 0x06000C95 RID: 3221 RVA: 0x00030C30 File Offset: 0x0002F030
		public Dictionary<ulong, CustomerItem> GetCustomerItems(ulong customerId)
		{
			return this.GetCustomerItemsInternal(customerId, (CustomerItem x) => x.CatalogItem.Active);
		}

		// Token: 0x06000C96 RID: 3222 RVA: 0x00030C58 File Offset: 0x0002F058
		public CustomerItem GetCustomerItem(ulong customerId, string itemName, OfferType offerType)
		{
			return this.GetCustomerItemsInternal(customerId, (CustomerItem x) => x.CatalogItem.Active).FirstOrDefault((KeyValuePair<ulong, CustomerItem> x) => x.Value.CatalogItem.Name == itemName && x.Value.OfferType == offerType).Value;
		}

		// Token: 0x06000C97 RID: 3223 RVA: 0x00030CB6 File Offset: 0x0002F0B6
		public Dictionary<ulong, CustomerItem> GetInactiveCustomerItems(ulong customerId)
		{
			return this.GetCustomerItemsInternal(customerId, (CustomerItem x) => !x.CatalogItem.Active);
		}

		// Token: 0x06000C98 RID: 3224 RVA: 0x00030CDC File Offset: 0x0002F0DC
		public void SetMoney(ulong customerId, Currency currency, ulong money)
		{
			if (currency == Currency.CryMoney && this.m_paymentService != null)
			{
				if (this.m_debugPaymentService == null)
				{
					throw new ApplicationException("Can't add cry money without DebugPaymentService!");
				}
				this.m_debugPaymentService.SetMoney(customerId, money);
			}
			this.m_dalService.ECatalog.SetMoney(customerId, currency, money);
		}

		// Token: 0x06000C99 RID: 3225 RVA: 0x00030D31 File Offset: 0x0002F131
		public List<CustomerAccount> GetCustomerAccounts(ulong customerId)
		{
			return this.GetCustomerAccounts(customerId, Enum.GetValues(typeof(Currency)).Cast<Currency>());
		}

		// Token: 0x06000C9A RID: 3226 RVA: 0x00030D50 File Offset: 0x0002F150
		public List<CustomerAccount> GetCustomerAccounts(ulong customerId, IEnumerable<Currency> currencies)
		{
			List<CustomerAccount> list = this.m_dalService.ECatalog.GetCustomerAccounts(customerId).ToList<CustomerAccount>();
			if (currencies.Contains(Currency.CryMoney))
			{
				this.SyncAccountsWithExternalSystems(customerId, list);
			}
			return list.FindAll((CustomerAccount acc) => currencies.Contains(acc.Currency));
		}

		// Token: 0x06000C9B RID: 3227 RVA: 0x00030DAC File Offset: 0x0002F1AC
		public CustomerAccount GetCustomerAccount(ulong customerId, Currency currency)
		{
			return this.GetCustomerAccounts(customerId, new Currency[]
			{
				currency
			}).SingleOrDefault<CustomerAccount>();
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x00030DC4 File Offset: 0x0002F1C4
		public AddCustomerItemResponse AddCustomerItem(ulong customerId, OfferItem item, bool ignoreLimit)
		{
			RegularItemConfig regularItemConfig = this.m_regularItemConfigProvider.Get();
			return this.m_dalService.ECatalog.AddCustomerItem(customerId, item, regularItemConfig.StackingEnabled, ignoreLimit);
		}

		// Token: 0x06000C9D RID: 3229 RVA: 0x00030DF8 File Offset: 0x0002F1F8
		public PurchaseOfferResponse AddCustomerBoxItems(ulong customerId, ulong boxItemId, List<OfferItem> box)
		{
			RegularItemConfig regularItemConfig = this.m_regularItemConfigProvider.Get();
			return this.m_dalService.ECatalog.AddCustomerBoxItems(customerId, boxItemId, box, regularItemConfig.StackingEnabled);
		}

		// Token: 0x06000C9E RID: 3230 RVA: 0x00030E2A File Offset: 0x0002F22A
		public PurchaseResult PurchaseOffer(UserInfo.User user, long offerHash, StoreOffer offer)
		{
			return this.PurchaseOffers(user, offerHash, new StoreOffer[]
			{
				offer
			});
		}

		// Token: 0x06000C9F RID: 3231 RVA: 0x00030E40 File Offset: 0x0002F240
		public PurchaseResult PurchaseOffers(UserInfo.User user, long offerHash, IEnumerable<StoreOffer> offers)
		{
			if (offers.Any((StoreOffer x) => x.GetPriceByCurrency(Currency.CryMoney) > 0UL))
			{
				this.SyncAccountsWithExternalSystems(user.UserID);
			}
			return this.m_shopService.PurchaseOffers(user, offerHash, offers, this.m_paymentCallback);
		}

		// Token: 0x06000CA0 RID: 3232 RVA: 0x00030E95 File Offset: 0x0002F295
		public ConsumeItemResponse ConsumeItem(ulong customerId, ulong itemId, ushort quantity)
		{
			return this.m_dalService.ECatalog.ConsumeItem(customerId, itemId, quantity);
		}

		// Token: 0x06000CA1 RID: 3233 RVA: 0x00030EAA File Offset: 0x0002F2AA
		public void UpdateItemDurability(ulong customerId, ulong catalogInstanceId, int addDurabilityPoints)
		{
			this.m_dalService.ECatalog.UpdateCatalogItemDurability(customerId, catalogInstanceId, addDurabilityPoints);
		}

		// Token: 0x06000CA2 RID: 3234 RVA: 0x00030EBF File Offset: 0x0002F2BF
		public void RepairPermanentItem(ulong customerId, ulong itemId, int durability, int totalDurability)
		{
			this.m_dalService.ECatalog.RepairPermanentItem(customerId, itemId, durability, totalDurability);
		}

		// Token: 0x06000CA3 RID: 3235 RVA: 0x00030ED6 File Offset: 0x0002F2D6
		public MoneyUpdateResult SpendMoney(ulong customerId, Currency currencyId, ulong ammount, SpendMoneyReason spendMoneyReason)
		{
			return this.SpendMoney(customerId, currencyId, ammount, 0UL, spendMoneyReason);
		}

		// Token: 0x06000CA4 RID: 3236 RVA: 0x00030EE5 File Offset: 0x0002F2E5
		public MoneyUpdateResult SpendMoney(ulong customerId, Currency currencyId, ulong ammount, ulong catalogId, SpendMoneyReason spendMoneyReason)
		{
			if (currencyId == Currency.CryMoney)
			{
				this.SyncAccountsWithExternalSystems(customerId);
			}
			return this.m_dalService.ECatalog.SpendMoney(customerId, currencyId, ammount, catalogId, spendMoneyReason, this.m_paymentCallback);
		}

		// Token: 0x06000CA5 RID: 3237 RVA: 0x00030F14 File Offset: 0x0002F314
		public MoneyUpdateResultMulti SpendMoney(ulong customerId, IEnumerable<KeyValuePair<Currency, ulong>> money, ulong catalogId, SpendMoneyReason spendMoneyReason)
		{
			if (money.Any((KeyValuePair<Currency, ulong> kv) => kv.Key == Currency.CryMoney))
			{
				this.SyncAccountsWithExternalSystems(customerId);
			}
			return this.m_dalService.ECatalog.SpendMoney(customerId, money, catalogId, spendMoneyReason, this.m_paymentCallback);
		}

		// Token: 0x06000CA6 RID: 3238 RVA: 0x00030F6C File Offset: 0x0002F36C
		public void AddMoney(ulong customerId, Currency currency, ulong gainedMoney, string transactionId = "")
		{
			if (currency == Currency.CryMoney && this.m_paymentService != null)
			{
				if (this.m_debugPaymentService == null)
				{
					throw new ApplicationException("Can't add cry money without DebugPaymentService!");
				}
				ulong money = this.m_debugPaymentService.AddMoney(customerId, gainedMoney);
				this.m_dalService.ECatalog.SetMoney(customerId, Currency.CryMoney, money);
			}
			else
			{
				try
				{
					this.m_dalService.ECatalog.AddMoney(customerId, currency, gainedMoney, transactionId, this.m_cashTransactionTime);
				}
				catch (DalException innerException)
				{
					throw new ECatGiveMoneyTransactionException(string.Format("Give money transaction collision found. UserId : {0} TransactionId : {1}", customerId, transactionId), innerException);
				}
			}
		}

		// Token: 0x06000CA7 RID: 3239 RVA: 0x00031014 File Offset: 0x0002F414
		public void DeleteCustomerItem(ulong customerId, ulong catalogInstanceId)
		{
			this.m_dalService.ECatalog.DeleteCustomerItem(customerId, catalogInstanceId);
		}

		// Token: 0x06000CA8 RID: 3240 RVA: 0x00031028 File Offset: 0x0002F428
		public void DebugResetCustomerItems(ulong customerID)
		{
			this.m_dalService.ECatalog.DebugResetCustomerItems(customerID);
		}

		// Token: 0x06000CA9 RID: 3241 RVA: 0x0003103C File Offset: 0x0002F43C
		public bool BackupLogs(TimeSpan logRecordLifetime, TimeSpan dbTimeout, int batchSize)
		{
			if (!Resources.RealmDBUpdaterPermission)
			{
				Log.Warning("Logs backup works on realm DB updater only.");
				return true;
			}
			bool result;
			lock (this)
			{
				Log.Info("Starting ecatalog log backup ...");
				bool flag2 = this.m_dalService.ECatalog.BackupLogs(logRecordLifetime, dbTimeout, batchSize);
				Log.Info("Ecatalog log backup finished");
				result = flag2;
			}
			return result;
		}

		// Token: 0x06000CAA RID: 3242 RVA: 0x000310B8 File Offset: 0x0002F4B8
		public bool ReloadOffers()
		{
			if (!Resources.RealmDBUpdaterPermission)
			{
				return false;
			}
			RegularItemConfig regularItemConfig = this.m_regularItemConfigProvider.Get();
			string text = Path.Combine(Resources.GetResourcesDirectory(), "catalog_data.xml");
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(text);
			string text2 = xmlDocument.DocumentElement.GetAttribute("hash") + regularItemConfig.MaxAmount;
			if (Resources.DebugContentEnabled)
			{
				CRC32 crc = new CRC32();
				crc.GetHash(Encoding.ASCII.GetBytes(text2 + Resources.DebugContentEnabled));
				text2 = crc.CRCVal.ToString();
			}
			if (this.m_dbUpdateService.GetECatDataGroupHash(xmlDocument.DocumentElement.Name) != text2)
			{
				Log.Info<string, string>("Updating data group {0}, hash {1}", xmlDocument.DocumentElement.Name, text2);
				using (FileStream fileStream = File.Open(text, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					this.UpdateCatalogContent(fileStream);
				}
				this.m_dbUpdateService.SetECatDataGroupHash(xmlDocument.DocumentElement.Name, text2);
			}
			else
			{
				Log.Info<string>("Same hash for group {0}", xmlDocument.DocumentElement.Name);
			}
			this.m_catalogOffersUpdater.Update(this.m_dbUpdateService);
			return true;
		}

		// Token: 0x06000CAB RID: 3243 RVA: 0x00031218 File Offset: 0x0002F618
		public void DebugLogsBackup()
		{
			if (!Resources.RealmDBUpdaterPermission)
			{
				Log.Warning("Logs backup works on realm DB updater only.");
				return;
			}
			this.m_jobSchedulerService.AddJob("ecat_backup_logs", new SimpleJobScheduler());
		}

		// Token: 0x06000CAC RID: 3244 RVA: 0x00031244 File Offset: 0x0002F644
		public void DebugDeleteAllItems(ulong customerId)
		{
			foreach (CustomerItem customerItem in this.m_dalService.ECatalog.GetCustomerItems(customerId))
			{
				this.m_dalService.ECatalog.DeleteCustomerItem(customerId, customerItem.InstanceID);
			}
		}

		// Token: 0x06000CAD RID: 3245 RVA: 0x000312B8 File Offset: 0x0002F6B8
		public IEnumerable<EcatLogHistory> DebugGetLogHistory(ulong customerId)
		{
			return this.m_dalService.ECatalog.DebugGetLogHistory(customerId);
		}

		// Token: 0x06000CAE RID: 3246 RVA: 0x000312CB File Offset: 0x0002F6CB
		public void ClearGiveMoneyTransactionHistory(ulong customerId)
		{
			this.m_dalService.ECatalog.DebugClearGiveMoneyTransactionHistory(customerId);
		}

		// Token: 0x06000CAF RID: 3247 RVA: 0x000312DE File Offset: 0x0002F6DE
		public void DebugGenEcatRecords(uint count, uint dayInterval)
		{
			this.m_dalService.ECatalog.DebugGenEcatRecords(count, dayInterval);
		}

		// Token: 0x06000CB0 RID: 3248 RVA: 0x000312F2 File Offset: 0x0002F6F2
		private void ConfigureCacheTimeout(int cacheResetTimeoutMin)
		{
			if (cacheResetTimeoutMin == 0)
			{
				this.m_cachingTime = TimeSpan.Zero;
				Log.Info("ECatalog cache reset timeout is disabled");
			}
			else
			{
				this.m_cachingTime = TimeSpan.FromMinutes((double)cacheResetTimeoutMin);
				Log.Info<int>("ECatalog cache reset timeout is {0}", cacheResetTimeoutMin);
			}
		}

		// Token: 0x06000CB1 RID: 3249 RVA: 0x0003132C File Offset: 0x0002F72C
		private void OnConfigChanged(ConfigEventArgs e)
		{
			if (string.Equals(e.Name, "CacheResetTimeoutMin", StringComparison.CurrentCultureIgnoreCase))
			{
				this.ConfigureCacheTimeout(e.iValue);
			}
			else if (string.Equals(e.Name, "cash_transaction_time_min", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_cashTransactionTime = e.TimeSpanValue;
			}
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x00031384 File Offset: 0x0002F784
		private void SyncAccountsWithExternalSystems(ulong customerId)
		{
			if (this.m_paymentService != null)
			{
				List<CustomerAccount> accounts = this.m_dalService.ECatalog.GetCustomerAccounts(customerId).ToList<CustomerAccount>();
				this.SyncAccountsWithExternalSystems(customerId, accounts);
			}
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x000313BC File Offset: 0x0002F7BC
		private void SyncAccountsWithExternalSystems(ulong customerId, List<CustomerAccount> accounts)
		{
			if (this.m_paymentService == null)
			{
				return;
			}
			ulong num = 0UL;
			try
			{
				num = this.m_paymentService.GetMoney(customerId);
			}
			catch (PaymentServiceException e)
			{
				Log.Error(e);
			}
			int num2 = accounts.FindIndex((CustomerAccount acc) => acc.Currency == Currency.CryMoney);
			if (num2 < 0)
			{
				num2 = accounts.Count;
				accounts.Add(new CustomerAccount
				{
					CustomerId = customerId,
					Currency = Currency.CryMoney,
					Money = 0UL
				});
			}
			if (num != accounts[num2].Money)
			{
				this.m_dalService.ECatalog.SetMoney(customerId, Currency.CryMoney, num);
				accounts[num2] = new CustomerAccount
				{
					CustomerId = customerId,
					Currency = Currency.CryMoney,
					Money = num
				};
			}
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x000314B4 File Offset: 0x0002F8B4
		public IEnumerable<Currency> GetCurrenciesForOffer(int supplierId, ulong offerid)
		{
			StoreOffer storeOfferById = this.GetStoreOfferById(supplierId, offerid);
			return from pt in storeOfferById.Prices
			where pt.Price > 0UL
			select pt.Currency;
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x00031514 File Offset: 0x0002F914
		public bool OnDBUpdateStage(IDBUpdateService updater, DBUpdateStage stage)
		{
			if (stage == DBUpdateStage.PreUpdate)
			{
				return this.EcatPreUpdateDB();
			}
			if (stage == DBUpdateStage.Schema)
			{
				return this.UpdateDB();
			}
			if (stage == DBUpdateStage.Procedures)
			{
				return this.UpdateDBProcedures();
			}
			if (stage == DBUpdateStage.CheckVersion)
			{
				return this.CheckDBVersion();
			}
			return stage != DBUpdateStage.Data || this.UpdateData();
		}

		// Token: 0x06000CB6 RID: 3254 RVA: 0x00031568 File Offset: 0x0002F968
		private bool UpdateDBProcedures()
		{
			if (this.m_dbUpdater.HasDBUpdateRights)
			{
				this.m_dbUpdater.UpdateProcedures();
			}
			return true;
		}

		// Token: 0x06000CB7 RID: 3255 RVA: 0x00031588 File Offset: 0x0002F988
		private bool UpdateDB()
		{
			if (this.m_dbUpdater == null)
			{
				this.m_dbUpdater = new ECatDBUpdater();
			}
			if (!this.m_dbUpdater.HasDBUpdateRights)
			{
				return this.CheckDBVersion();
			}
			if (this.m_dbUpdater.ServerSchema.LatestVersion == DBVersion.Zero)
			{
				Console.WriteLine("ECatalog database does not exist, do you wish to create it? (Y/N)");
				bool createDB = false;
				ConsoleCmdManager.ConsoleInteract(delegate(ConsoleKey key)
				{
					if (key == ConsoleKey.Y)
					{
						createDB = true;
					}
					return true;
				}, new ConsoleKey[]
				{
					ConsoleKey.Y
				});
				if (!createDB)
				{
					Console.WriteLine("Cancelled.");
					return false;
				}
				this.m_dbUpdater.CreateDatabase();
			}
			if (!this.m_dbUpdater.CheckEncoding())
			{
				return false;
			}
			if (!this.m_dbUpdater.ServerSchema.Equals(this.m_dbUpdater.LocalSchema))
			{
				Console.WriteLine("Do you want to update ECatalog database from version {0} to version {1}? (Y/N)", this.m_dbUpdater.ServerSchema.LatestVersion, this.m_dbUpdater.LocalSchema.LatestVersion);
				bool doUpdate = false;
				ConsoleCmdManager.ConsoleInteract(delegate(ConsoleKey key)
				{
					if (key == ConsoleKey.Y)
					{
						doUpdate = true;
					}
					return true;
				}, new ConsoleKey[]
				{
					ConsoleKey.Y
				});
				if (!doUpdate)
				{
					Console.WriteLine("Cancelled.");
					return false;
				}
				this.m_dbUpdater.UpdateDatabase();
			}
			Log.Info<DBVersion>("ECatalog schema version {0}", this.m_dbUpdater.ServerSchema.LatestVersion);
			return true;
		}

		// Token: 0x06000CB8 RID: 3256 RVA: 0x00031704 File Offset: 0x0002FB04
		private bool CheckDBVersion()
		{
			if (this.m_dbUpdater == null)
			{
				this.m_dbUpdater = new ECatDBUpdater();
			}
			if (!this.m_dbUpdater.ServerSchema.Equals(this.m_dbUpdater.LocalSchema))
			{
				Log.Error<DBVersion, DBVersion>("ECatalog database version {0} does not match local version {1} and server does not have realm_db_updater permissions", this.m_dbUpdater.ServerSchema.LatestVersion, this.m_dbUpdater.LocalSchema.LatestVersion);
				return false;
			}
			return true;
		}

		// Token: 0x06000CB9 RID: 3257 RVA: 0x00031774 File Offset: 0x0002FB74
		private bool UpdateData()
		{
			if (!this.m_dbUpdater.HasStandaloneDB || Resources.RealmDBUpdaterPermission)
			{
				this.ReloadOffers();
			}
			return true;
		}

		// Token: 0x06000CBA RID: 3258 RVA: 0x00031798 File Offset: 0x0002FB98
		private string GetECatUpdaterId()
		{
			return string.Format("{0}/{1}", Resources.Jid, Resources.RealmId);
		}

		// Token: 0x06000CBB RID: 3259 RVA: 0x000317B3 File Offset: 0x0002FBB3
		private void ResetUpdaterPermission()
		{
			this.m_dalService.ECatalog.ResetUpdaterPermission();
		}

		// Token: 0x06000CBC RID: 3260 RVA: 0x000317C5 File Offset: 0x0002FBC5
		private bool TryLockUpdaterPermission(string onlineId)
		{
			return this.m_dalService.ECatalog.TryLockUpdaterPermission(onlineId);
		}

		// Token: 0x06000CBD RID: 3261 RVA: 0x000317D8 File Offset: 0x0002FBD8
		private bool EcatPreUpdateDB()
		{
			string ecatUpdaterId = this.GetECatUpdaterId();
			if (Resources.DBUpdaterEcatPermissionReset)
			{
				Log.Warning("Master Server will reset db updater permission now");
				this.ResetUpdaterPermission();
			}
			if (Resources.RealmDBUpdaterPermission && !this.TryLockUpdaterPermission(ecatUpdaterId))
			{
				Log.Error("MasterServer failed to lock db updater permission for ecatalog");
				return false;
			}
			return true;
		}

		// Token: 0x06000CBE RID: 3262 RVA: 0x0003182C File Offset: 0x0002FC2C
		private void UpdateCatalogContent(Stream catalogData)
		{
			IEnumerable<CatalogItem> catalogItems = this.m_dalService.ECatalog.GetCatalogItems();
			Dictionary<string, ulong> dictionary = catalogItems.ToDictionary((CatalogItem item) => item.Name, (CatalogItem item) => item.ID);
			RegularItemConfig regularItemConfig = this.m_regularItemConfigProvider.Get();
			XmlTextReader xmlTextReader = new XmlTextReader(catalogData);
			while (xmlTextReader.Read())
			{
				if (xmlTextReader.NodeType == XmlNodeType.Element)
				{
					if (xmlTextReader.Name.ToLower() == "item")
					{
						bool flag = xmlTextReader.GetAttribute("testcontent") == "1";
						if (Resources.DebugContentEnabled || !flag)
						{
							string attribute = xmlTextReader.GetAttribute("name");
							string attribute2 = xmlTextReader.GetAttribute("description");
							int maxAmount;
							if (!int.TryParse(xmlTextReader.GetAttribute("max_amount"), out maxAmount))
							{
								maxAmount = (int)regularItemConfig.MaxAmount;
							}
							bool stackable = xmlTextReader.GetAttribute("stackable") == "1";
							string attribute3 = xmlTextReader.GetAttribute("type");
							this.ValidateCatalogData(attribute3, stackable);
							this.m_dalService.ECatalog.AddItem(attribute, attribute2, maxAmount, stackable, attribute3);
							dictionary.Remove(attribute);
						}
					}
				}
			}
			foreach (KeyValuePair<string, ulong> keyValuePair in dictionary)
			{
				this.m_dalService.ECatalog.ActivateItem(keyValuePair.Value, false);
				Log.Info<string>("Deactivate missed catalog item {0}", keyValuePair.Key);
			}
			if (this.CatalogItemsUpdated != null)
			{
				this.CatalogItemsUpdated();
			}
		}

		// Token: 0x06000CBF RID: 3263 RVA: 0x00031A18 File Offset: 0x0002FE18
		private void ValidateCatalogData(string type, bool stackable)
		{
			if (type == "random_box" && stackable)
			{
				throw new Exception("Box item cannot be stackable");
			}
		}

		// Token: 0x06000CC0 RID: 3264 RVA: 0x00031A3B File Offset: 0x0002FE3B
		private void OnExecutionPhaseChanged(ExecutionPhase executionPhase)
		{
			if (executionPhase == ExecutionPhase.Stopping)
			{
				this.UnlockUpdaterPermission(this.GetECatUpdaterId());
			}
		}

		// Token: 0x06000CC1 RID: 3265 RVA: 0x00031A50 File Offset: 0x0002FE50
		private void UnlockUpdaterPermission(string onlineId)
		{
			this.m_dalService.ECatalog.UnlockUpdaterPermission(onlineId);
		}

		// Token: 0x040005CD RID: 1485
		private TimeSpan m_cachingTime;

		// Token: 0x040005CE RID: 1486
		private DateTime m_lastCacheUpdate = DateTime.MinValue;

		// Token: 0x040005CF RID: 1487
		private TimeSpan m_cashTransactionTime;

		// Token: 0x040005D0 RID: 1488
		private Dictionary<string, CatalogItem> m_catalogItems = new Dictionary<string, CatalogItem>();

		// Token: 0x040005D1 RID: 1489
		private readonly object m_lock = new object();

		// Token: 0x040005D2 RID: 1490
		private readonly IJobSchedulerService m_jobSchedulerService;

		// Token: 0x040005D3 RID: 1491
		private readonly IDALService m_dalService;

		// Token: 0x040005D4 RID: 1492
		private readonly IPaymentService m_paymentService;

		// Token: 0x040005D5 RID: 1493
		private readonly IDebugPaymentService m_debugPaymentService;

		// Token: 0x040005D6 RID: 1494
		private readonly IDBUpdateService m_dbUpdateService;

		// Token: 0x040005D7 RID: 1495
		private readonly IECatalogOffersUpdater m_catalogOffersUpdater;

		// Token: 0x040005D8 RID: 1496
		private readonly IShopService m_shopService;

		// Token: 0x040005D9 RID: 1497
		private ECatDBUpdater m_dbUpdater;

		// Token: 0x040005DA RID: 1498
		private readonly IPaymentCallback m_paymentCallback;

		// Token: 0x040005DB RID: 1499
		private readonly IConfigProvider<RegularItemConfig> m_regularItemConfigProvider;

		// Token: 0x02000246 RID: 582
		private class PaymentCallback : MarshalByRefObject, IPaymentCallback
		{
			// Token: 0x06000CCF RID: 3279 RVA: 0x00031AF4 File Offset: 0x0002FEF4
			public PaymentCallback(IPaymentService paymentService, ICatalogService catalogService)
			{
				this.m_paymentService = paymentService;
				this.m_catalogService = catalogService;
			}

			// Token: 0x06000CD0 RID: 3280 RVA: 0x00031B0C File Offset: 0x0002FF0C
			public PaymentCallbackResult SpendMoney(ulong userId, ulong amount, SpendMoneyReason spendMoneyReason)
			{
				PaymentResult result = this.m_paymentService.SpendMoney(userId, amount, spendMoneyReason);
				return this.ToPaymentCallbackResult(result);
			}

			// Token: 0x06000CD1 RID: 3281 RVA: 0x00031B30 File Offset: 0x0002FF30
			public PaymentCallbackResult SpendMoneyByOfferId(ulong userId, int supplierId, ulong offerId)
			{
				StoreOffer storeOfferById = this.m_catalogService.GetStoreOfferById(supplierId, offerId);
				if (storeOfferById == null)
				{
					throw new ApplicationException(string.Format("Can't find offer by id {0}", offerId));
				}
				PaymentResult result = this.m_paymentService.SpendMoney(userId, storeOfferById);
				return this.ToPaymentCallbackResult(result);
			}

			// Token: 0x06000CD2 RID: 3282 RVA: 0x00031B7C File Offset: 0x0002FF7C
			private PaymentCallbackResult ToPaymentCallbackResult(PaymentResult result)
			{
				if (result == PaymentResult.Ok)
				{
					return PaymentCallbackResult.Ok;
				}
				if (result != PaymentResult.NotEnoughMoney)
				{
					throw new ApplicationException(string.Format("Can't translate payment result {0}", result));
				}
				return PaymentCallbackResult.NotEnoughMoney;
			}

			// Token: 0x06000CD3 RID: 3283 RVA: 0x00031BA9 File Offset: 0x0002FFA9
			public override object InitializeLifetimeService()
			{
				return null;
			}

			// Token: 0x040005E9 RID: 1513
			private readonly IPaymentService m_paymentService;

			// Token: 0x040005EA RID: 1514
			private readonly ICatalogService m_catalogService;
		}
	}
}
