using System;
using MasterServer.Core;
using MasterServer.GameLogic.VoucherSystem.VoucherSynchronization;

namespace MasterServer.GameLogic.VoucherSystem.ConsoleCommands
{
	// Token: 0x0200046C RID: 1132
	[ConsoleCmdAttributes(CmdName = "get_voucher_current_index", Help = "Return current value of voucher index")]
	internal class GetVoucherCurrentIndexCmd : IConsoleCmd
	{
		// Token: 0x060017EA RID: 6122 RVA: 0x0006315E File Offset: 0x0006155E
		public GetVoucherCurrentIndexCmd(IDebugVoucherSynchronizer synchronizer)
		{
			this.m_synchronizer = synchronizer;
		}

		// Token: 0x060017EB RID: 6123 RVA: 0x00063170 File Offset: 0x00061570
		public void ExecuteCmd(string[] args)
		{
			ulong currentIndex = this.m_synchronizer.GetCurrentIndex();
			Log.Info(string.Format("Voucher index is {0}", currentIndex));
		}

		// Token: 0x04000B8B RID: 2955
		private readonly IDebugVoucherSynchronizer m_synchronizer;
	}
}
