using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.VoucherSystem.VoucherSynchronization
{
	// Token: 0x0200044D RID: 1101
	[ConsoleCmdAttributes(CmdName = "synchronize_vouchers", Help = "Trigger voucher synchronization")]
	internal class SynchronizeVouchersCommand : IConsoleCmd
	{
		// Token: 0x06001763 RID: 5987 RVA: 0x00060F0D File Offset: 0x0005F30D
		public SynchronizeVouchersCommand(IVoucherSynchronizer synchronizer)
		{
			this.m_synchronizer = synchronizer;
		}

		// Token: 0x06001764 RID: 5988 RVA: 0x00060F1C File Offset: 0x0005F31C
		public void ExecuteCmd(string[] args)
		{
			this.m_synchronizer.Synchronize();
		}

		// Token: 0x04000B41 RID: 2881
		private readonly IVoucherSynchronizer m_synchronizer;
	}
}
