using System;
using System.Collections.Generic;
using System.Diagnostics;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog.Exceptions;
using MasterServer.Telemetry.Metrics;
using MasterServer.Users;

namespace MasterServer.ElectronicCatalog.ShopSupplier
{
	// Token: 0x02000251 RID: 593
	internal abstract class ShopSupplier : IShopSupplier, IDisposable
	{
		// Token: 0x06000D0F RID: 3343 RVA: 0x00032E8E File Offset: 0x0003128E
		protected ShopSupplier(IShopSupplierMetricsTracker metricsTracker)
		{
			this.m_metricsTracker = metricsTracker;
		}

		// Token: 0x06000D10 RID: 3344 RVA: 0x00032E9D File Offset: 0x0003129D
		public virtual void Dispose()
		{
		}

		// Token: 0x06000D11 RID: 3345 RVA: 0x00032EA0 File Offset: 0x000312A0
		private T Request<T>(Func<T> requestFunc, ShopSupplierRequest request)
		{
			this.m_metricsTracker.ReportRequest(request, this.SupplierId);
			Stopwatch stopwatch = Stopwatch.StartNew();
			T result;
			try
			{
				T t = requestFunc();
				stopwatch.Stop();
				Log.Info<string, int, TimeSpan>("[Shop supplier: {0} request in supplier {1} took {2}]", request.Name, this.SupplierId, stopwatch.Elapsed);
				result = t;
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				this.m_metricsTracker.ReportRequestFailed(request, this.SupplierId);
				Log.Info<string, int, TimeSpan>("[Shop supplier: {0} request in supplier {1} failed in {2}]", request.Name, this.SupplierId, stopwatch.Elapsed);
				if (ex.InnerException != null)
				{
					throw new ShopServiceInternalErrorException(ex.InnerException);
				}
				throw new ShopServiceInternalErrorException(ex);
			}
			finally
			{
				this.m_metricsTracker.ReportRequestTime(request, this.SupplierId, stopwatch.Elapsed);
			}
			return result;
		}

		// Token: 0x06000D12 RID: 3346 RVA: 0x00032F80 File Offset: 0x00031380
		public IEnumerable<StoreOffer> GetOffers()
		{
			return this.Request<IEnumerable<StoreOffer>>(new Func<IEnumerable<StoreOffer>>(this.GetOffersImpl), ShopSupplierRequest.GetOffers);
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x00032FA8 File Offset: 0x000313A8
		public PurchaseResult PurchaseOffers(UserInfo.User user, IEnumerable<StoreOffer> offers, IPaymentCallback paymentCallback)
		{
			return this.Request<PurchaseResult>(() => this.PurchaseOffersImpl(user, offers, paymentCallback), ShopSupplierRequest.PurchaseOffer);
		}

		// Token: 0x06000D14 RID: 3348 RVA: 0x00032FEE File Offset: 0x000313EE
		public long GetOfferHash()
		{
			return this.Request<long>(() => this.GetOfferHashImpl(true), ShopSupplierRequest.OfferHash);
		}

		// Token: 0x06000D15 RID: 3349
		protected abstract IEnumerable<StoreOffer> GetOffersImpl();

		// Token: 0x06000D16 RID: 3350
		protected abstract PurchaseResult PurchaseOffersImpl(UserInfo.User user, IEnumerable<StoreOffer> offers, IPaymentCallback paymentCallback);

		// Token: 0x06000D17 RID: 3351
		protected abstract long GetOfferHashImpl(bool force);

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x06000D18 RID: 3352
		public abstract int SupplierId { get; }

		// Token: 0x040005FB RID: 1531
		private readonly IShopSupplierMetricsTracker m_metricsTracker;
	}
}
