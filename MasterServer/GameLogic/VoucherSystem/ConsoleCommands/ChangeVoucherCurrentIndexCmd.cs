using System;
using MasterServer.Core;
using MasterServer.GameLogic.VoucherSystem.VoucherSynchronization;

namespace MasterServer.GameLogic.VoucherSystem.ConsoleCommands
{
	// Token: 0x0200046D RID: 1133
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "change_voucher_current_index", Help = "Set new value to voucher current index")]
	internal class ChangeVoucherCurrentIndexCmd : ConsoleCommand<ChangeVoucherCurrentIndexParams>
	{
		// Token: 0x060017EC RID: 6124 RVA: 0x0006319E File Offset: 0x0006159E
		public ChangeVoucherCurrentIndexCmd(IDebugVoucherSynchronizer synchronizer)
		{
			this.m_synchronizer = synchronizer;
		}

		// Token: 0x060017ED RID: 6125 RVA: 0x000631AD File Offset: 0x000615AD
		protected override void Execute(ChangeVoucherCurrentIndexParams param)
		{
			Log.Info(string.Format("Setting voucher index set to {0}", param.IndexValue));
			this.m_synchronizer.SetCurrentIndex(param.IndexValue);
		}

		// Token: 0x04000B8C RID: 2956
		private readonly IDebugVoucherSynchronizer m_synchronizer;
	}
}
