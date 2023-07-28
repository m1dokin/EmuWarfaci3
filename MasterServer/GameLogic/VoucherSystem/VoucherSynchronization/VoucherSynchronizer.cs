using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.DAL.VoucherSystem;
using MasterServer.Database;
using MasterServer.Database.RemoteClients;
using MasterServer.GameLogic.VoucherSystem.Exceptions;
using MasterServer.GameLogic.VoucherSystem.VoucherProviders;

namespace MasterServer.GameLogic.VoucherSystem.VoucherSynchronization
{
	// Token: 0x02000483 RID: 1155
	[Service]
	[Singleton]
	internal class VoucherSynchronizer : ServiceModule, IVoucherSynchronizer, IDebugVoucherSynchronizer
	{
		// Token: 0x0600184F RID: 6223 RVA: 0x00064808 File Offset: 0x00062C08
		public VoucherSynchronizer(IPageVoucherCommunicationClient pageVoucherCommunicationClient, IDALService dalService, IVoucherValidator voucherValidator)
		{
			this.m_pageVoucherClient = pageVoucherCommunicationClient;
			this.m_voucherSystemClient = dalService.VoucherSystem;
			this.m_voucherValidator = voucherValidator;
		}

		// Token: 0x06001850 RID: 6224 RVA: 0x0006482A File Offset: 0x00062C2A
		public override void Stop()
		{
			this.m_pageVoucherClient.Dispose();
			base.Stop();
		}

		// Token: 0x06001851 RID: 6225 RVA: 0x00064840 File Offset: 0x00062C40
		public void Synchronize()
		{
			ulong currentIndex = this.GetCurrentIndex();
			ConfigSection section = Resources.ModuleSettings.GetSection("Voucher");
			int pageSize;
			section.GetSection("externalvoucher").Get("synchronization_batch_size", out pageSize);
			Task task = this.m_pageVoucherClient.GetVouchers(currentIndex, pageSize).ContinueWith(delegate(Task<IEnumerable<Voucher>> t)
			{
				if (t.IsFaulted)
				{
					Log.Error(t.Exception);
					return;
				}
				List<Voucher> list = t.Result.ToList<Voucher>();
				if (list.Any<Voucher>())
				{
					foreach (Voucher voucher in list)
					{
						if (!this.m_voucherValidator.IsValid(voucher))
						{
							voucher.Status = VoucherStatus.Corrupted;
						}
						if (!this.m_voucherSystemClient.AddVoucher(voucher))
						{
							Log.Error(new VoucherAddException(voucher));
						}
					}
					currentIndex = list.Max((Voucher v) => v.Id) + 1UL;
					this.m_voucherSystemClient.SetCurrentIndex("voucher_current_index", currentIndex);
				}
			});
			task.Wait();
		}

		// Token: 0x06001852 RID: 6226 RVA: 0x000648B8 File Offset: 0x00062CB8
		public IEnumerable<Voucher> SynchronizeCorrupted(ulong startIndex, int count)
		{
			List<Voucher> list = this.m_voucherSystemClient.GetCorruptedVouchers(startIndex, count).ToList<Voucher>();
			List<Task> list2 = new List<Task>();
			using (List<Voucher>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Voucher voucher = enumerator.Current;
					VoucherSynchronizer $this = this;
					Task item = this.m_pageVoucherClient.GetVouchers(voucher.Id, 1).ContinueWith(delegate(Task<IEnumerable<Voucher>> t)
					{
						if (t.IsFaulted)
						{
							Log.Error(t.Exception);
							return;
						}
						IEnumerable<Voucher> result = t.Result;
						if (!result.Any<Voucher>())
						{
							return;
						}
						Voucher voucher = result.FirstOrDefault((Voucher v) => v.Id == voucher.Id);
						if (voucher != null && !voucher.Equals(voucher))
						{
							if (!$this.m_voucherValidator.IsValid(voucher))
							{
								voucher.Status = VoucherStatus.Corrupted;
							}
							Voucher voucher2 = $this.m_voucherSystemClient.UpdateVoucher(voucher);
							if (voucher2.UserId != voucher.UserId)
							{
								Log.Error(new VoucherUpdateException(string.Format("Unexpected user {0}, should be {1}, for voucher {2} ", voucher.UserId, voucher2.UserId, voucher)));
							}
							if (voucher2.UserId == 0UL)
							{
								Log.Error(new VoucherUpdateException(string.Format("Can't find voucher for update {0}", voucher)));
							}
						}
					});
					list2.Add(item);
				}
			}
			Task task = Task.WhenAll(list2);
			task.Wait();
			return list;
		}

		// Token: 0x06001853 RID: 6227 RVA: 0x00064978 File Offset: 0x00062D78
		public void CleanUpVouchers(ulong userId)
		{
			this.m_voucherSystemClient.CleanUpVouchers(userId);
		}

		// Token: 0x06001854 RID: 6228 RVA: 0x00064986 File Offset: 0x00062D86
		public ulong GetCurrentIndex()
		{
			return this.m_voucherSystemClient.GetCurrentIndex("voucher_current_index");
		}

		// Token: 0x06001855 RID: 6229 RVA: 0x00064998 File Offset: 0x00062D98
		public void SetCurrentIndex(ulong index)
		{
			this.m_voucherSystemClient.SetCurrentIndex("voucher_current_index", index);
		}

		// Token: 0x04000BAD RID: 2989
		private const string CURRENT_PAGE_GAME_STATE = "voucher_current_index";

		// Token: 0x04000BAE RID: 2990
		private readonly IPageVoucherCommunicationClient m_pageVoucherClient;

		// Token: 0x04000BAF RID: 2991
		private readonly IVoucherSystemClient m_voucherSystemClient;

		// Token: 0x04000BB0 RID: 2992
		private readonly IVoucherValidator m_voucherValidator;
	}
}
