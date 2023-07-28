using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using MasterServer.Core;
using MasterServer.DAL.VoucherSystem;
using MasterServer.GameLogic.VoucherSystem.Serializers;

namespace MasterServer.GameLogic.VoucherSystem.VoucherProviders
{
	// Token: 0x02000477 RID: 1143
	internal abstract class VoucherProvider : IVoucherProvider
	{
		// Token: 0x0600180E RID: 6158 RVA: 0x00060E22 File Offset: 0x0005F222
		protected VoucherProvider(ILogService logService)
		{
			this.m_logService = logService;
		}

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x0600180F RID: 6159
		protected abstract string Type { get; }

		// Token: 0x06001810 RID: 6160
		public abstract Task<IEnumerable<Voucher>> GetNewVouchers(ulong userId);

		// Token: 0x06001811 RID: 6161
		public abstract IEnumerable<Voucher> GetAllVouchersForUser(ulong userId);

		// Token: 0x06001812 RID: 6162 RVA: 0x00060E34 File Offset: 0x0005F234
		public void ReportVoucher(Voucher voucher)
		{
			try
			{
				this.ReportVoucherImpl(voucher);
			}
			catch (Exception)
			{
				voucher.Status = VoucherStatus.Corrupted;
				throw;
			}
			finally
			{
				this.ReportVoucherConsumed(voucher);
			}
		}

		// Token: 0x06001813 RID: 6163
		protected abstract void ReportVoucherImpl(Voucher voucher);

		// Token: 0x06001814 RID: 6164 RVA: 0x00060E7C File Offset: 0x0005F27C
		private void ReportVoucherConsumed(Voucher voucher)
		{
			XmlDocument xmlDocument = new XmlDocument();
			VoucherXmlSerializer voucherXmlSerializer = new VoucherXmlSerializer(xmlDocument);
			XmlElement xmlElement = voucherXmlSerializer.Serialize(voucher);
			this.m_logService.Event.VoucherConsumed(voucher.UserId, voucher.Id, this.Type, voucher.Status, xmlElement.OuterXml);
		}

		// Token: 0x04000B90 RID: 2960
		private readonly ILogService m_logService;
	}
}
