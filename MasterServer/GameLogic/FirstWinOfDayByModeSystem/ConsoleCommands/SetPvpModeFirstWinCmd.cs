using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem.ConsoleCommands
{
	// Token: 0x02000077 RID: 119
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "set_pvp_mode_first_win", Help = "Sets player first win progress for particular game mode")]
	internal class SetPvpModeFirstWinCmd : ConsoleCommand<SetPvpModeFirstWinCmdParams>
	{
		// Token: 0x060001CB RID: 459 RVA: 0x0000B34B File Offset: 0x0000974B
		public SetPvpModeFirstWinCmd(IFirstWinOfDayByModeService firstWinOfDayByModeService)
		{
			this.m_firstWinOfDayByModeService = firstWinOfDayByModeService;
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000B35A File Offset: 0x0000975A
		protected override void Execute(SetPvpModeFirstWinCmdParams param)
		{
			this.m_firstWinOfDayByModeService.SetPvpModeFirstWin(param.ProfileId, param.GameMode);
			Log.Info<string, ulong>("Pvp mode({0}) win was set successfully for profile {1}", param.GameMode, param.ProfileId);
		}

		// Token: 0x040000D7 RID: 215
		private readonly IFirstWinOfDayByModeService m_firstWinOfDayByModeService;
	}
}
