using System;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.DAL;
using MasterServer.Platform.Payment.Exceptions;
using MasterServer.Telemetry.Metrics;
using Util.Common;

namespace MasterServer.Platform.Payment
{
	// Token: 0x02000691 RID: 1681
	[Service]
	[Singleton]
	internal class EmulatorPaymentService : PaymentService, IDebugPaymentService
	{
		// Token: 0x0600234E RID: 9038 RVA: 0x0009574A File Offset: 0x00093B4A
		public EmulatorPaymentService(IPaymentMetricsTracker metricsService, IWalletService walletService, ILogService logService) : base(metricsService, logService)
		{
			this.m_walletService = walletService;
		}

		// Token: 0x0600234F RID: 9039 RVA: 0x0009575C File Offset: 0x00093B5C
		public void SetMoney(ulong userId, ulong amount)
		{
			try
			{
				this.m_walletService.SetMoney(userId, amount);
			}
			catch (AggregateException inner)
			{
				throw new PaymentServiceException(inner);
			}
		}

		// Token: 0x06002350 RID: 9040 RVA: 0x00095794 File Offset: 0x00093B94
		public ulong AddMoney(ulong userId, ulong amount)
		{
			ulong result;
			try
			{
				result = this.m_walletService.AddMoney(userId, amount);
			}
			catch (AggregateException inner)
			{
				throw new PaymentServiceException(inner);
			}
			return result;
		}

		// Token: 0x06002351 RID: 9041 RVA: 0x000957D0 File Offset: 0x00093BD0
		public PaymentResult SpendMoney(ulong userId, ulong amount)
		{
			return base.SpendMoney(userId, amount, SpendMoneyReason.Debug);
		}

		// Token: 0x06002352 RID: 9042 RVA: 0x000957DB File Offset: 0x00093BDB
		public void SetRequestFailedState(bool state)
		{
			this.m_walletService.SetRequestFailedState(state);
		}

		// Token: 0x06002353 RID: 9043 RVA: 0x000957E9 File Offset: 0x00093BE9
		public bool GetRequestFailedState()
		{
			return this.m_walletService.GetRequestFailedState();
		}

		// Token: 0x06002354 RID: 9044 RVA: 0x000957F6 File Offset: 0x00093BF6
		public void SetRequestTimeout(TimeSpan timeout)
		{
			this.m_walletService.SetRequestTimeout(timeout);
		}

		// Token: 0x06002355 RID: 9045 RVA: 0x00095804 File Offset: 0x00093C04
		public TimeSpan GetRequestTimeout()
		{
			return this.m_walletService.GetRequestTimeout();
		}

		// Token: 0x06002356 RID: 9046 RVA: 0x00095811 File Offset: 0x00093C11
		protected override ConfigSection GetConfig()
		{
			return Resources.ModuleSettings.GetSection("Payment");
		}

		// Token: 0x06002357 RID: 9047 RVA: 0x00095824 File Offset: 0x00093C24
		protected override Task<ulong> GetMoneyImpl(ulong userId)
		{
			ulong money;
			try
			{
				money = this.m_walletService.GetMoney(userId);
			}
			catch (Exception exception)
			{
				return TaskHelpers.Failed<ulong>(exception);
			}
			return TaskHelpers.Completed<ulong>(money);
		}

		// Token: 0x06002358 RID: 9048 RVA: 0x00095868 File Offset: 0x00093C68
		protected override Task<PaymentResult> SpendMoneyImpl(ulong userId, StoreOffer offer)
		{
			ulong priceByCurrency = offer.GetPriceByCurrency(Currency.CryMoney);
			return this.SpendMoneyImpl(userId, priceByCurrency, SpendMoneyReason.None);
		}

		// Token: 0x06002359 RID: 9049 RVA: 0x00095888 File Offset: 0x00093C88
		protected override Task<PaymentResult> SpendMoneyImpl(ulong userId, ulong amount, SpendMoneyReason spendMoneyReason)
		{
			return Task.Factory.StartNew<PaymentResult>(() => this.SpendMoneyFunc(userId, amount));
		}

		// Token: 0x0600235A RID: 9050 RVA: 0x000958C6 File Offset: 0x00093CC6
		private PaymentResult SpendMoneyFunc(ulong userId, ulong amount)
		{
			return this.m_walletService.SpendMoney(userId, amount);
		}

		// Token: 0x040011DF RID: 4575
		private readonly IWalletService m_walletService;
	}
}
