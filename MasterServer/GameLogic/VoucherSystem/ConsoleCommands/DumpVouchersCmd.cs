using System;
using System.Collections.Generic;
using MasterServer.Core;

namespace MasterServer.GameLogic.VoucherSystem.ConsoleCommands
{
	// Token: 0x02000470 RID: 1136
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "debug_dump_vouchers", Help = "Dumps all vouchers for user")]
	internal class DumpVouchersCmd : ConsoleCommand<DumpVouchersParams>
	{
		// Token: 0x060017F1 RID: 6129 RVA: 0x000631ED File Offset: 0x000615ED
		public DumpVouchersCmd(IDebugVoucherService voucherService)
		{
			this.m_voucherService = voucherService;
		}

		// Token: 0x060017F2 RID: 6130 RVA: 0x000631FC File Offset: 0x000615FC
		protected override void Execute(DumpVouchersParams param)
		{
			IEnumerable<Voucher> allVouchers = this.m_voucherService.GetAllVouchers(param.UserId);
			string format = string.Join<Voucher>(Environment.NewLine, allVouchers);
			Log.Info(format);
		}

		// Token: 0x04000B8D RID: 2957
		private readonly IDebugVoucherService m_voucherService;
	}
}
