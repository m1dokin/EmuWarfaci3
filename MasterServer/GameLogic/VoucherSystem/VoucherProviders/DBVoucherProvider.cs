using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Core;
using MasterServer.Database;
using MasterServer.GameLogic.VoucherSystem.Exceptions;

namespace MasterServer.GameLogic.VoucherSystem.VoucherProviders
{
	// Token: 0x02000473 RID: 1139
	[Service]
	[Singleton]
	internal class DBVoucherProvider : VoucherProvider
	{
		// Token: 0x060017F5 RID: 6133 RVA: 0x00063236 File Offset: 0x00061636
		public DBVoucherProvider(IDALService dalService, ILogService logService) : base(logService)
		{
			this.m_dalService = dalService;
		}

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x060017F6 RID: 6134 RVA: 0x00063246 File Offset: 0x00061646
		protected override string Type
		{
			get
			{
				return "cti";
			}
		}

		// Token: 0x060017F7 RID: 6135 RVA: 0x00063250 File Offset: 0x00061650
		public override Task<IEnumerable<Voucher>> GetNewVouchers(ulong userId)
		{
			IEnumerable<Voucher> newVouchers = this.m_dalService.VoucherSystem.GetNewVouchers(userId);
			return Task.FromResult<IEnumerable<Voucher>>(newVouchers);
		}

		// Token: 0x060017F8 RID: 6136 RVA: 0x00063278 File Offset: 0x00061678
		public override IEnumerable<Voucher> GetAllVouchersForUser(ulong userId)
		{
			return this.m_dalService.VoucherSystem.GetAllVouchers(userId);
		}

		// Token: 0x060017F9 RID: 6137 RVA: 0x00063298 File Offset: 0x00061698
		protected override void ReportVoucherImpl(Voucher voucher)
		{
			this.ReportVoucherStatus(voucher);
		}

		// Token: 0x060017FA RID: 6138 RVA: 0x000632A4 File Offset: 0x000616A4
		private void ReportVoucherStatus(Voucher voucher)
		{
			Voucher voucher2 = this.m_dalService.VoucherSystem.UpdateVoucher(voucher);
			if (voucher2.UserId != voucher.UserId)
			{
				Log.Error(new VoucherUpdateException(string.Format("Unexpected user {0}, should be {1}, for voucher {2} ", voucher2.UserId, voucher.UserId, voucher)));
			}
			if (voucher2.UserId == 0UL)
			{
				Log.Error(new VoucherUpdateException(string.Format("Can't find voucher for update {0}", voucher)));
			}
		}

		// Token: 0x04000B8E RID: 2958
		private readonly IDALService m_dalService;
	}
}
