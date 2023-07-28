using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Core;

namespace MasterServer.GameLogic.VoucherSystem.VoucherProviders
{
	// Token: 0x0200044C RID: 1100
	[Service]
	[Singleton]
	internal class RestVoucherProvider : VoucherProvider
	{
		// Token: 0x0600175E RID: 5982 RVA: 0x00060ECC File Offset: 0x0005F2CC
		public RestVoucherProvider(ILogService logService, IVoucherCommunicationClient communicationClient) : base(logService)
		{
			this.m_communicationClient = communicationClient;
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x0600175F RID: 5983 RVA: 0x00060EDC File Offset: 0x0005F2DC
		protected override string Type
		{
			get
			{
				return "dlc";
			}
		}

		// Token: 0x06001760 RID: 5984 RVA: 0x00060EE3 File Offset: 0x0005F2E3
		public override Task<IEnumerable<Voucher>> GetNewVouchers(ulong userId)
		{
			return this.m_communicationClient.GetNewVouchers(userId);
		}

		// Token: 0x06001761 RID: 5985 RVA: 0x00060EF1 File Offset: 0x0005F2F1
		public override IEnumerable<Voucher> GetAllVouchersForUser(ulong userId)
		{
			return this.m_communicationClient.GetAllVouchers(userId);
		}

		// Token: 0x06001762 RID: 5986 RVA: 0x00060EFF File Offset: 0x0005F2FF
		protected override void ReportVoucherImpl(Voucher voucher)
		{
			this.m_communicationClient.UpdateVoucher(voucher);
		}

		// Token: 0x04000B40 RID: 2880
		private readonly IVoucherCommunicationClient m_communicationClient;
	}
}
