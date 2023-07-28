using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem.ConsoleCommands
{
	// Token: 0x0200007A RID: 122
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "reset_pvp_modes_first_win", Help = "Resets players first win progress for all pvp modes")]
	internal class ResetPvpModesFirstWinCmd : ConsoleCommand<ResetPvpModesFirstWinCmdParams>
	{
		// Token: 0x060001D4 RID: 468 RVA: 0x0000B3E8 File Offset: 0x000097E8
		public ResetPvpModesFirstWinCmd(IDebugFirstWinOfDayByModeService debugFirstWinOfDayByModeService)
		{
			this.m_debugFirstWinOfDayByModeService = debugFirstWinOfDayByModeService;
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000B3F7 File Offset: 0x000097F7
		protected override void Execute(ResetPvpModesFirstWinCmdParams param)
		{
			this.m_debugFirstWinOfDayByModeService.ResetPvpModesFirstWin(param.ProfileId);
			Log.Info<ulong>("Pvp modes win was reseted successfully for profile {0}", param.ProfileId);
		}

		// Token: 0x040000DB RID: 219
		private readonly IDebugFirstWinOfDayByModeService m_debugFirstWinOfDayByModeService;
	}
}
