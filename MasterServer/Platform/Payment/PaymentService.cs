using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Platform.Payment.Exceptions;
using MasterServer.Telemetry.Metrics;
using Util.Common;

namespace MasterServer.Platform.Payment
{
	// Token: 0x0200069D RID: 1693
	internal abstract class PaymentService : ServiceModule, IPaymentService
	{
		// Token: 0x06002377 RID: 9079 RVA: 0x000744C2 File Offset: 0x000728C2
		protected PaymentService(IPaymentMetricsTracker paymentMetricsTracker, ILogService logService)
		{
			this.m_paymentMetricsTracker = paymentMetricsTracker;
			this.m_logService = logService;
		}

		// Token: 0x06002378 RID: 9080 RVA: 0x000744F4 File Offset: 0x000728F4
		public override void Init()
		{
			base.Init();
			ConfigSection config = this.GetConfig();
			config.OnConfigChanged += this.OnConfigUpdated;
			bool active;
			config.Get("active", out active);
			this.m_active = active;
			int num;
			config.Get("timeout_ms", out num);
			this.m_timeout = TimeSpan.FromMilliseconds((double)num);
		}

		// Token: 0x06002379 RID: 9081 RVA: 0x00074550 File Offset: 0x00072950
		public override void Stop()
		{
			base.Stop();
			ConfigSection config = this.GetConfig();
			config.OnConfigChanged -= this.OnConfigUpdated;
		}

		// Token: 0x0600237A RID: 9082 RVA: 0x00074580 File Offset: 0x00072980
		public Task<ulong> GetMoneyAsync(ulong userId)
		{
			if (!this.m_active)
			{
				return TaskHelpers.Completed<ulong>(0UL);
			}
			this.m_paymentMetricsTracker.ReportGetMoneyRequest();
			DateTime startTime = DateTime.Now;
			Task<ulong> moneyImpl = this.GetMoneyImpl(userId);
			return moneyImpl.ContinueWith<ulong>(delegate(Task<ulong> task)
			{
				this.OnGetMoneyFinished(task, userId, startTime);
				return task.Result;
			}, TaskContinuationOptions.ExecuteSynchronously);
		}

		// Token: 0x0600237B RID: 9083 RVA: 0x000745F0 File Offset: 0x000729F0
		public ulong GetMoney(ulong userId)
		{
			Task<ulong> moneyAsync = this.GetMoneyAsync(userId);
			bool flag;
			try
			{
				flag = moneyAsync.Wait(this.m_timeout);
			}
			catch (AggregateException ex)
			{
				if (ex.InnerExceptions.Any<Exception>())
				{
					IEnumerable<Exception> src = from e in ex.Flatten().InnerExceptions
					where !(e is PaymentServiceException)
					select e;
					if (PaymentService.<>f__mg$cache0 == null)
					{
						PaymentService.<>f__mg$cache0 = new Action<Exception>(Log.Error);
					}
					src.ForEach(PaymentService.<>f__mg$cache0);
					PaymentServiceException inner = ex.FindException<PaymentServiceException>();
					throw new PaymentServiceException(inner);
				}
				throw new PaymentServiceException(ex);
			}
			if (!flag)
			{
				throw new PaymentServiceTimeoutException(this.m_timeout);
			}
			return moneyAsync.Result;
		}

		// Token: 0x0600237C RID: 9084 RVA: 0x000746B4 File Offset: 0x00072AB4
		public PaymentResult SpendMoney(ulong userId, ulong amount, SpendMoneyReason spendMoneyReason)
		{
			if (!this.m_active)
			{
				throw new ApplicationException("Can't spend money, payment system is not active.");
			}
			this.m_paymentMetricsTracker.ReportSpendMoneyRequest();
			this.m_logService.Event.PaymentSpendMoneyRequestLog(userId, (long)(-(long)amount));
			DateTime now = DateTime.Now;
			PaymentResult result;
			try
			{
				Task<PaymentResult> task = this.SpendMoneyImpl(userId, amount, spendMoneyReason);
				bool flag;
				try
				{
					flag = task.Wait(this.m_timeout);
				}
				catch (AggregateException ex)
				{
					if (ex.InnerExceptions.Any<Exception>())
					{
						IEnumerable<Exception> src = from e in ex.Flatten().InnerExceptions
						where !(e is PaymentServiceException)
						select e;
						if (PaymentService.<>f__mg$cache1 == null)
						{
							PaymentService.<>f__mg$cache1 = new Action<Exception>(Log.Error);
						}
						src.ForEach(PaymentService.<>f__mg$cache1);
						PaymentServiceException inner = ex.FindException<PaymentServiceException>();
						throw new PaymentServiceException(inner);
					}
					throw new PaymentServiceException(ex);
				}
				if (!flag)
				{
					throw new PaymentServiceTimeoutException(this.m_timeout);
				}
				TimeSpan timeSpan = DateTime.Now - now;
				this.m_logService.Event.PaymentMoneySpentLog(userId, (long)(-(long)amount), task.Result);
				this.m_paymentMetricsTracker.ReportSpendMoneyRequestTime(timeSpan);
				Log.Info<ulong, TimeSpan>("[Payment: spend money request for {0} took {1}]", userId, timeSpan);
				result = task.Result;
			}
			catch
			{
				TimeSpan p = DateTime.Now - now;
				this.m_logService.Event.PaymentSpendMoneyFailedLog(userId, (long)(-(long)amount));
				this.m_paymentMetricsTracker.ReportSpendMoneyRequestFailed();
				Log.Info<ulong, TimeSpan>("[Payment: spend money request for {0} failed in {1}]", userId, p);
				throw;
			}
			return result;
		}

		// Token: 0x0600237D RID: 9085 RVA: 0x00074848 File Offset: 0x00072C48
		public PaymentResult SpendMoney(ulong userId, StoreOffer offer)
		{
			if (offer.Prices.First((PriceTag p) => p.Currency == Currency.CryMoney).Price == 0UL)
			{
				return PaymentResult.Ok;
			}
			if (!this.m_active)
			{
				throw new ApplicationException("Can't spend money, payment system is not active.");
			}
			this.m_paymentMetricsTracker.ReportSpendMoneyRequest();
			this.m_logService.Event.PaymentSpendMoneyOfferRequestLog(userId, offer.StoreID, (long)(-(long)offer.Prices.First((PriceTag p) => p.Currency == Currency.CryMoney).Price));
			DateTime now = DateTime.Now;
			PaymentResult result;
			try
			{
				Task<PaymentResult> task = this.SpendMoneyImpl(userId, offer);
				bool flag;
				try
				{
					flag = task.Wait(this.m_timeout);
				}
				catch (AggregateException ex)
				{
					if (ex.InnerExceptions.Any<Exception>())
					{
						IEnumerable<Exception> src = from e in ex.Flatten().InnerExceptions
						where !(e is PaymentServiceException)
						select e;
						if (PaymentService.<>f__mg$cache2 == null)
						{
							PaymentService.<>f__mg$cache2 = new Action<Exception>(Log.Error);
						}
						src.ForEach(PaymentService.<>f__mg$cache2);
						PaymentServiceException inner = ex.FindException<PaymentServiceException>();
						throw new PaymentServiceException(inner);
					}
					throw new PaymentServiceException(ex);
				}
				if (!flag)
				{
					throw new PaymentServiceTimeoutException(this.m_timeout);
				}
				TimeSpan timeSpan = DateTime.Now - now;
				this.m_logService.Event.PaymentMoneyOfferSpentLog(userId, offer.StoreID, (long)(-(long)offer.Prices.First((PriceTag p) => p.Currency == Currency.CryMoney).Price), task.Result, offer.Status, offer.Discount);
				this.m_paymentMetricsTracker.ReportSpendMoneyRequestTime(timeSpan);
				Log.Info<ulong, TimeSpan>("[Payment: spend money request for {0} took {1}]", userId, timeSpan);
				result = task.Result;
			}
			catch
			{
				TimeSpan p2 = DateTime.Now - now;
				this.m_logService.Event.PaymentSpendMoneyOfferFailedLog(userId, offer.StoreID, (long)(-(long)offer.Prices.First((PriceTag p) => p.Currency == Currency.CryMoney).Price));
				this.m_paymentMetricsTracker.ReportSpendMoneyRequestFailed();
				Log.Info<ulong, TimeSpan>("[Payment: spend money request for {0} failed in {1}]", userId, p2);
				throw;
			}
			return result;
		}

		// Token: 0x0600237E RID: 9086
		protected abstract ConfigSection GetConfig();

		// Token: 0x0600237F RID: 9087
		protected abstract Task<ulong> GetMoneyImpl(ulong userId);

		// Token: 0x06002380 RID: 9088
		protected abstract Task<PaymentResult> SpendMoneyImpl(ulong userId, ulong amount, SpendMoneyReason spendMoneyReason);

		// Token: 0x06002381 RID: 9089
		protected abstract Task<PaymentResult> SpendMoneyImpl(ulong userId, StoreOffer offer);

		// Token: 0x06002382 RID: 9090 RVA: 0x00074AE0 File Offset: 0x00072EE0
		protected virtual void OnConfigUpdated(ConfigEventArgs args)
		{
			if (string.Equals(args.Name, "active", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_active = (args.sValue == "1");
			}
			else if (string.Equals(args.Name, "timeout_ms", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_timeout = TimeSpan.FromMilliseconds((double)args.iValue);
			}
		}

		// Token: 0x06002383 RID: 9091 RVA: 0x00074B48 File Offset: 0x00072F48
		private void OnGetMoneyFinished(Task task, ulong userId, DateTime startTime)
		{
			TimeSpan timeSpan = DateTime.Now - startTime;
			if (task.IsFaulted)
			{
				AggregateException exception = task.Exception;
				this.m_paymentMetricsTracker.ReportGetMoneyRequestFailed();
				Log.Info<ulong, TimeSpan>("[Payment: get money request for {0} failed in {1}]", userId, timeSpan);
			}
			else
			{
				Log.Info<ulong, TimeSpan>("[Payment: get money request for {0} took {1}]", userId, timeSpan);
			}
			this.m_paymentMetricsTracker.ReportGetMoneyRequestTime(timeSpan);
		}

		// Token: 0x040011E3 RID: 4579
		private readonly IPaymentMetricsTracker m_paymentMetricsTracker;

		// Token: 0x040011E4 RID: 4580
		private readonly ILogService m_logService;

		// Token: 0x040011E5 RID: 4581
		private bool m_active = true;

		// Token: 0x040011E6 RID: 4582
		private TimeSpan m_timeout = TimeSpan.FromSeconds(5.0);

		// Token: 0x040011E7 RID: 4583
		[CompilerGenerated]
		private static Action<Exception> <>f__mg$cache0;

		// Token: 0x040011E9 RID: 4585
		[CompilerGenerated]
		private static Action<Exception> <>f__mg$cache1;

		// Token: 0x040011EB RID: 4587
		[CompilerGenerated]
		private static Action<Exception> <>f__mg$cache2;
	}
}
