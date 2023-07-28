using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Jobs;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog.Exceptions;
using MasterServer.ElectronicCatalog.ShopSupplier;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000249 RID: 585
	[Service]
	[Singleton]
	internal class ShopService : ServiceModule, IShopService
	{
		// Token: 0x06000CDF RID: 3295 RVA: 0x00031C54 File Offset: 0x00030054
		public ShopService(IEnumerable<IShopSupplier> suppliers, IJobSchedulerService jobSchedulerService)
		{
			foreach (IShopSupplier shopSupplier in suppliers)
			{
				if (this.m_shopSuppliers.ContainsKey(shopSupplier.SupplierId))
				{
					throw new ApplicationException(string.Format("ShopSupplier {0} has duplicated id {1}", shopSupplier.GetType().Name, shopSupplier.SupplierId));
				}
				this.m_shopSuppliers.Add(shopSupplier.SupplierId, shopSupplier);
			}
			this.m_jobSchedulerService = jobSchedulerService;
		}

		// Token: 0x1400002A RID: 42
		// (add) Token: 0x06000CE0 RID: 3296 RVA: 0x00031D20 File Offset: 0x00030120
		// (remove) Token: 0x06000CE1 RID: 3297 RVA: 0x00031D58 File Offset: 0x00030158
		public event Action<IEnumerable<StoreOffer>> OffersUpdated;

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06000CE2 RID: 3298 RVA: 0x00031D8E File Offset: 0x0003018E
		public long OffersHash
		{
			get
			{
				this.GetOffers();
				return this.m_offerHash;
			}
		}

		// Token: 0x06000CE3 RID: 3299 RVA: 0x00031DA0 File Offset: 0x000301A0
		public override void Init()
		{
			base.Init();
			ConfigSection section = Resources.ECatalogSettings.GetSection("ECatalog");
			section.OnConfigChanged += this.OnConfigChanged;
			ConfigSection section2 = Resources.ModuleSettings.GetSection("Shop");
			this.m_maxBatchSize = int.Parse(section2.Get("MaxBatchSize"));
			section2.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x06000CE4 RID: 3300 RVA: 0x00031E0D File Offset: 0x0003020D
		public override void Start()
		{
			base.Start();
			this.m_jobSchedulerService.AddJob("offer_synchronization");
		}

		// Token: 0x06000CE5 RID: 3301 RVA: 0x00031E28 File Offset: 0x00030228
		public override void Stop()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("Shop");
			section.OnConfigChanged -= this.OnConfigChanged;
			foreach (KeyValuePair<int, IShopSupplier> keyValuePair in this.m_shopSuppliers)
			{
				keyValuePair.Value.Dispose();
			}
			this.m_shopSuppliers.Clear();
			base.Stop();
		}

		// Token: 0x06000CE6 RID: 3302 RVA: 0x00031EBC File Offset: 0x000302BC
		public IEnumerable<StoreOffer> GetOffers()
		{
			if (!this.m_offers.Any<StoreOffer>())
			{
				this.ReloadOffers();
			}
			return this.m_offers;
		}

		// Token: 0x06000CE7 RID: 3303 RVA: 0x00031EDC File Offset: 0x000302DC
		public StoreOffer GetStoreOfferById(int supplierId, ulong offerId)
		{
			return this.GetOffers().FirstOrDefault((StoreOffer x) => x.StoreID == offerId && x.SupplierID == supplierId);
		}

		// Token: 0x06000CE8 RID: 3304 RVA: 0x00031F14 File Offset: 0x00030314
		public PurchaseResult PurchaseOffers(UserInfo.User user, long offerHash, IEnumerable<StoreOffer> offers, IPaymentCallback paymentCallback)
		{
			int supplierId = offers.First<StoreOffer>().SupplierID;
			if (offers.Any((StoreOffer x) => x.SupplierID != supplierId))
			{
				throw new ShopServiceInternalErrorException("Shop service doesn't support batch offer from different suppliers");
			}
			int num = offers.Count((StoreOffer o) => o.IsKeyPriceOffer());
			if (num > 0 && num < offers.Count<StoreOffer>())
			{
				throw new ShopServiceInternalErrorException("Shop service doesn't support batch offer with different currencies");
			}
			if (offers.Count<StoreOffer>() > this.m_maxBatchSize)
			{
				throw new ShopServiceInternalErrorException(string.Format("Shop service doesn't support batch offer with size more than {0}", this.m_maxBatchSize));
			}
			bool flag = offers.FirstOrDefault((StoreOffer o) => o.IsIngameCoin()) != null && offers.Count<StoreOffer>() == 1;
			if (offerHash != this.OffersHash && !flag)
			{
				return new PurchaseResult(TransactionStatus.HASH_MISMATCH);
			}
			PurchaseResult purchaseResult = this.m_shopSuppliers[supplierId].PurchaseOffers(user, offers, paymentCallback);
			if (purchaseResult.Status == TransactionStatus.HASH_MISMATCH)
			{
				this.ReloadOffers();
			}
			return purchaseResult;
		}

		// Token: 0x06000CE9 RID: 3305 RVA: 0x00032048 File Offset: 0x00030448
		public void LoadOffers()
		{
			long num = this.m_shopSuppliers.Aggregate(0L, (long current, KeyValuePair<int, IShopSupplier> shopSupplier) => current ^ shopSupplier.Value.GetOfferHash());
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_offerHash == num)
				{
					return;
				}
			}
			List<StoreOffer> list = new List<StoreOffer>();
			foreach (KeyValuePair<int, IShopSupplier> keyValuePair in this.m_shopSuppliers)
			{
				list.AddRange(keyValuePair.Value.GetOffers());
			}
			bool flag2 = false;
			object lock2 = this.m_lock;
			lock (lock2)
			{
				if (this.m_offerHash != num)
				{
					Log.Info<long>("Updating offers, hash {0}", num);
					this.m_offers = list;
					this.m_offerHash = num;
					flag2 = true;
				}
			}
			if (flag2)
			{
				this.OffersUpdated.SafeInvoke(this.m_offers);
			}
		}

		// Token: 0x06000CEA RID: 3306 RVA: 0x00032194 File Offset: 0x00030594
		private void ReloadOffers()
		{
			try
			{
				this.LoadOffers();
			}
			catch (ShopServiceException e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x06000CEB RID: 3307 RVA: 0x000321C8 File Offset: 0x000305C8
		private void OnConfigChanged(ConfigEventArgs e)
		{
			if (e.Name.Equals("MaxBatchSize", StringComparison.InvariantCultureIgnoreCase))
			{
				this.m_maxBatchSize = e.iValue;
			}
		}

		// Token: 0x040005EB RID: 1515
		private readonly object m_lock = new object();

		// Token: 0x040005EC RID: 1516
		private readonly IJobSchedulerService m_jobSchedulerService;

		// Token: 0x040005ED RID: 1517
		private readonly Dictionary<int, IShopSupplier> m_shopSuppliers = new Dictionary<int, IShopSupplier>();

		// Token: 0x040005EE RID: 1518
		private List<StoreOffer> m_offers = new List<StoreOffer>();

		// Token: 0x040005EF RID: 1519
		private int m_maxBatchSize;

		// Token: 0x040005F0 RID: 1520
		private long m_offerHash;
	}
}
