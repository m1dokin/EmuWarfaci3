using System;
using System.Collections.Generic;
using System.Text;
using MasterServer.Core;
using MasterServer.GameLogic.VoucherSystem.VoucherSynchronization;

namespace MasterServer.GameLogic.VoucherSystem.ConsoleCommands
{
	// Token: 0x02000446 RID: 1094
	[ConsoleCmdAttributes(CmdName = "synchronize_corrupted_voucher", Help = "synchronize corrupted voucher")]
	internal class SynchronizeCorruptedVoucherCmd : ConsoleCommand<SynchronizeCorruptedVoucherParams>
	{
		// Token: 0x06001751 RID: 5969 RVA: 0x00060D3C File Offset: 0x0005F13C
		public SynchronizeCorruptedVoucherCmd(IVoucherSynchronizer voucherSynchronizer)
		{
			this.m_voucherSynchronizer = voucherSynchronizer;
		}

		// Token: 0x06001752 RID: 5970 RVA: 0x00060D4C File Offset: 0x0005F14C
		protected override void Execute(SynchronizeCorruptedVoucherParams param)
		{
			IEnumerable<Voucher> enumerable = this.m_voucherSynchronizer.SynchronizeCorrupted(param.IndexValue, 1);
			StringBuilder stringBuilder = new StringBuilder("Synchronized vouchers:\n");
			foreach (Voucher voucher in enumerable)
			{
				stringBuilder.AppendLine(voucher.ToString());
			}
			Log.Info(stringBuilder.ToString());
		}

		// Token: 0x04000B3E RID: 2878
		private readonly IVoucherSynchronizer m_voucherSynchronizer;
	}
}
