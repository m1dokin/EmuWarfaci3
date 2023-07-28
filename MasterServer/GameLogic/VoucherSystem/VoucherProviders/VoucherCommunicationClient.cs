using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MasterServer.Core;
using MasterServer.Telemetry.Metrics;

namespace MasterServer.GameLogic.VoucherSystem.VoucherProviders
{
	// Token: 0x02000476 RID: 1142
	internal abstract class VoucherCommunicationClient : IPageVoucherCommunicationClient, IVoucherCommunicationClient, IDisposable
	{
		// Token: 0x06001802 RID: 6146 RVA: 0x0006334C File Offset: 0x0006174C
		protected VoucherCommunicationClient(IVoucherMetricsTracker metricsTracker)
		{
			this.m_metricsTracker = metricsTracker;
		}

		// Token: 0x06001803 RID: 6147
		public abstract void Dispose();

		// Token: 0x06001804 RID: 6148 RVA: 0x0006335C File Offset: 0x0006175C
		public Task<IEnumerable<Voucher>> GetVouchers(ulong pageIndex, int pageSize)
		{
			return this.RequestAsync<IEnumerable<Voucher>>(() => this.GetVouchersAsyncImpl(pageIndex, pageSize), VoucherRequest.GetVouchersPage);
		}

		// Token: 0x06001805 RID: 6149 RVA: 0x0006339C File Offset: 0x0006179C
		public Task<IEnumerable<Voucher>> GetNewVouchers(ulong userId)
		{
			return this.RequestAsync<IEnumerable<Voucher>>(() => this.GetNewVouchersAsyncImpl(userId), VoucherRequest.GetVouchersPerUser);
		}

		// Token: 0x06001806 RID: 6150 RVA: 0x000633D4 File Offset: 0x000617D4
		public void UpdateVoucher(Voucher voucher)
		{
			this.Request<bool>(delegate
			{
				this.UpdateVoucherImpl(voucher);
				return true;
			}, VoucherRequest.ConfirmVoucher);
		}

		// Token: 0x06001807 RID: 6151 RVA: 0x00063410 File Offset: 0x00061810
		public IEnumerable<Voucher> GetAllVouchers(ulong userId)
		{
			return this.Request<IEnumerable<Voucher>>(() => this.GetAllVouchersImp(userId), VoucherRequest.GetVouchersPerUserAll);
		}

		// Token: 0x06001808 RID: 6152
		protected abstract IEnumerable<Voucher> GetAllVouchersImp(ulong userId);

		// Token: 0x06001809 RID: 6153
		protected abstract Task<IEnumerable<Voucher>> GetVouchersAsyncImpl(ulong pageIndex, int pageSize);

		// Token: 0x0600180A RID: 6154
		protected abstract Task<IEnumerable<Voucher>> GetNewVouchersAsyncImpl(ulong userId);

		// Token: 0x0600180B RID: 6155
		protected abstract void UpdateVoucherImpl(Voucher voucher);

		// Token: 0x0600180C RID: 6156 RVA: 0x00063448 File Offset: 0x00061848
		private Task<T> RequestAsync<T>(Func<Task<T>> requestFunc, VoucherRequest request)
		{
			this.m_metricsTracker.ReportRequest(request);
			Stopwatch timer = Stopwatch.StartNew();
			TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
			requestFunc().ContinueWith(delegate(Task<T> t)
			{
				timer.Stop();
				if (t.IsFaulted)
				{
					this.m_metricsTracker.ReportRequestFailed(request);
					tcs.SetException(new VoucherProviderInternalErrorException(t.Exception.Flatten()));
				}
				else
				{
					this.m_metricsTracker.ReportRequestTime(request, timer.Elapsed);
					tcs.SetResult(t.Result);
				}
			});
			return tcs.Task;
		}

		// Token: 0x0600180D RID: 6157 RVA: 0x000634B4 File Offset: 0x000618B4
		private T Request<T>(Func<T> requestFunc, VoucherRequest request)
		{
			this.m_metricsTracker.ReportRequest(request);
			Stopwatch stopwatch = Stopwatch.StartNew();
			T result;
			try
			{
				T t = requestFunc();
				stopwatch.Stop();
				Log.Info<string, TimeSpan>("[Voucher: request {0} took {1}]", request.Name, stopwatch.Elapsed);
				result = t;
			}
			catch (AggregateException ex)
			{
				stopwatch.Stop();
				Log.Info<string, TimeSpan>("[Voucher: request {0} failed in {1}] agg", request.Name, stopwatch.Elapsed);
				this.m_metricsTracker.ReportRequestFailed(request);
				throw new VoucherProviderInternalErrorException(ex.Flatten());
			}
			catch (Exception ex2)
			{
				stopwatch.Stop();
				Log.Info<string, TimeSpan>("[Voucher: request {0} failed in {1}]", request.Name, stopwatch.Elapsed);
				this.m_metricsTracker.ReportRequestFailed(request);
				if (ex2.InnerException != null)
				{
					throw new VoucherProviderInternalErrorException(ex2.InnerException);
				}
				throw new VoucherProviderInternalErrorException(ex2);
			}
			finally
			{
				this.m_metricsTracker.ReportRequestTime(request, stopwatch.Elapsed);
			}
			return result;
		}

		// Token: 0x04000B8F RID: 2959
		private readonly IVoucherMetricsTracker m_metricsTracker;
	}
}
