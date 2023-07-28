using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.RatingSystem.ConsoleCommands
{
	// Token: 0x0200008E RID: 142
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "set_pvp_rating_win_streak", Help = "Sets PvP rating win streak for given profile id")]
	internal class SetPvpRatingWinStreakCmd : ConsoleCommand<SetPvpRatingWinStreakCmdParams>
	{
		// Token: 0x06000226 RID: 550 RVA: 0x0000C106 File Offset: 0x0000A506
		public SetPvpRatingWinStreakCmd(IDebugRatingService debugRatingService)
		{
			this.m_debugRatingService = debugRatingService;
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000C115 File Offset: 0x0000A515
		protected override void Execute(SetPvpRatingWinStreakCmdParams param)
		{
			this.m_debugRatingService.SetRatingWinStreak(param.ProfileId, param.WinStreak);
			Log.Info("Win streak was updated");
		}

		// Token: 0x040000F5 RID: 245
		private readonly IDebugRatingService m_debugRatingService;
	}
}
