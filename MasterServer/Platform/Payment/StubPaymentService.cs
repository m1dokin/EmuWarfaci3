using System;
using System.Threading;
using System.Threading.Tasks;
using Enyim.Caching.Memcached;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.Platform.Payment.Exceptions;
using MasterServer.Telemetry.Metrics;

namespace MasterServer.Platform.Payment
{
	// Token: 0x0200069E RID: 1694
	[Service]
	[Singleton]
	internal class StubPaymentService : PaymentService, IDebugPaymentService
	{
		// Token: 0x0600238B RID: 9099 RVA: 0x00095BB2 File Offset: 0x00093FB2
		public StubPaymentService(IMemcachedService memcachedService, IPaymentMetricsTracker paymentMetricsTracker, ILogService logService) : base(paymentMetricsTracker, logService)
		{
			this.m_memcachedService = memcachedService;
		}

		// Token: 0x0600238C RID: 9100 RVA: 0x00095BCE File Offset: 0x00093FCE
		public void SetMoney(ulong userId, ulong amount)
		{
			if (!this.m_memcachedService.Store(StoreMode.Set, this.GetCacheDomain(userId), amount))
			{
				throw new PaymentServiceInternalErrorException();
			}
		}

		// Token: 0x0600238D RID: 9101 RVA: 0x00095BF4 File Offset: 0x00093FF4
		public ulong AddMoney(ulong userId, ulong amount)
		{
			ulong moneyFromCache = this.GetMoneyFromCache(userId);
			ulong num = moneyFromCache + amount;
			if (this.m_memcachedService.Store(StoreMode.Set, this.GetCacheDomain(userId), num))
			{
				return num;
			}
			throw new PaymentServiceInternalErrorException();
		}

		// Token: 0x0600238E RID: 9102 RVA: 0x00095C32 File Offset: 0x00094032
		public PaymentResult SpendMoney(ulong userId, ulong amount)
		{
			return base.SpendMoney(userId, amount, SpendMoneyReason.Debug);
		}

		// Token: 0x0600238F RID: 9103 RVA: 0x00095C3D File Offset: 0x0009403D
		public void SetRequestFailedState(bool state)
		{
			this.m_brokenFlag = state;
		}

		// Token: 0x06002390 RID: 9104 RVA: 0x00095C46 File Offset: 0x00094046
		public bool GetRequestFailedState()
		{
			return this.m_brokenFlag;
		}

		// Token: 0x06002391 RID: 9105 RVA: 0x00095C4E File Offset: 0x0009404E
		public void SetRequestTimeout(TimeSpan timeout)
		{
			this.m_debugTimeout = timeout;
		}

		// Token: 0x06002392 RID: 9106 RVA: 0x00095C57 File Offset: 0x00094057
		public TimeSpan GetRequestTimeout()
		{
			return this.m_debugTimeout;
		}

		// Token: 0x06002393 RID: 9107 RVA: 0x00095C5F File Offset: 0x0009405F
		protected override ConfigSection GetConfig()
		{
			return Resources.ModuleSettings.GetSection("Payment");
		}

		// Token: 0x06002394 RID: 9108 RVA: 0x00095C70 File Offset: 0x00094070
		protected override Task<ulong> GetMoneyImpl(ulong userId)
		{
			return Task.Factory.StartNew<ulong>(delegate()
			{
				Thread.Sleep(this.m_debugTimeout);
				return this.GetMoneyFunc(userId);
			});
		}

		// Token: 0x06002395 RID: 9109 RVA: 0x00095CA8 File Offset: 0x000940A8
		protected override Task<PaymentResult> SpendMoneyImpl(ulong userId, StoreOffer offer)
		{
			ulong priceByCurrency = offer.GetPriceByCurrency(Currency.CryMoney);
			return this.SpendMoneyImpl(userId, priceByCurrency, SpendMoneyReason.None);
		}

		// Token: 0x06002396 RID: 9110 RVA: 0x00095CC8 File Offset: 0x000940C8
		protected override Task<PaymentResult> SpendMoneyImpl(ulong userId, ulong amount, SpendMoneyReason spendMoneyReason)
		{
			return Task.Factory.StartNew<PaymentResult>(delegate()
			{
				Thread.Sleep(this.m_debugTimeout);
				return this.SpendMoneyFunc(userId, amount);
			});
		}

		// Token: 0x06002397 RID: 9111 RVA: 0x00095D06 File Offset: 0x00094106
		private ulong GetMoneyFunc(ulong userId)
		{
			if (this.m_brokenFlag)
			{
				throw new PaymentServiceInternalErrorException();
			}
			return this.GetMoneyFromCache(userId);
		}

		// Token: 0x06002398 RID: 9112 RVA: 0x00095D20 File Offset: 0x00094120
		private PaymentResult SpendMoneyFunc(ulong userId, ulong amount)
		{
			if (this.m_brokenFlag)
			{
				throw new PaymentServiceInternalErrorException();
			}
			ulong moneyFromCache = this.GetMoneyFromCache(userId);
			if (amount > moneyFromCache)
			{
				return PaymentResult.NotEnoughMoney;
			}
			if (this.m_memcachedService.Store(StoreMode.Set, this.GetCacheDomain(userId), moneyFromCache - amount))
			{
				return PaymentResult.Ok;
			}
			throw new PaymentServiceInternalErrorException();
		}

		// Token: 0x06002399 RID: 9113 RVA: 0x00095D78 File Offset: 0x00094178
		private ulong GetMoneyFromCache(ulong userId)
		{
			cache_domain cacheDomain = this.GetCacheDomain(userId);
			if (this.m_brokenFlag)
			{
				throw new PaymentServiceInternalErrorException();
			}
			object obj;
			if (this.m_memcachedService.Get(cacheDomain, out obj))
			{
				return (ulong)obj;
			}
			return 0UL;
		}

		// Token: 0x0600239A RID: 9114 RVA: 0x00095DBA File Offset: 0x000941BA
		private cache_domain GetCacheDomain(ulong userId)
		{
			return new cache_domain(string.Format("payment.money_{0}", userId));
		}

		// Token: 0x040011F1 RID: 4593
		private readonly IMemcachedService m_memcachedService;

		// Token: 0x040011F2 RID: 4594
		private bool m_brokenFlag;

		// Token: 0x040011F3 RID: 4595
		private TimeSpan m_debugTimeout = TimeSpan.Zero;
	}
}
