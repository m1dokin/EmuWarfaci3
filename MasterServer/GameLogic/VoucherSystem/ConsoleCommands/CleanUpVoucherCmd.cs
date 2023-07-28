using System;
using MasterServer.Core;
using MasterServer.GameLogic.VoucherSystem.VoucherSynchronization;

namespace MasterServer.GameLogic.VoucherSystem.ConsoleCommands
{
	// Token: 0x02000443 RID: 1091
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "cleanup_vouchers", Help = "remove  all vouchers for user")]
	internal class CleanUpVoucherCmd : ConsoleCommand<CleanUpVoucherParams>
	{
		// Token: 0x06001749 RID: 5961 RVA: 0x00060CE8 File Offset: 0x0005F0E8
		public CleanUpVoucherCmd(IDebugVoucherSynchronizer voucherSynchronizer)
		{
			this.m_voucherSynchronizer = voucherSynchronizer;
		}

		// Token: 0x0600174A RID: 5962 RVA: 0x00060CF7 File Offset: 0x0005F0F7
		protected override void Execute(CleanUpVoucherParams param)
		{
			this.m_voucherSynchronizer.CleanUpVouchers(param.UserId);
		}

		// Token: 0x04000B3B RID: 2875
		private readonly IDebugVoucherSynchronizer m_voucherSynchronizer;
	}
}
