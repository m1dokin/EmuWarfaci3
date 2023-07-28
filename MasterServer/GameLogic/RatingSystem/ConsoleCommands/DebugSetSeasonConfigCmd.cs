using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.RatingSystem.ConsoleCommands
{
	// Token: 0x02000092 RID: 146
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "debug_set_season_config", Help = "Reconfigures season")]
	internal class DebugSetSeasonConfigCmd : ConsoleCommand<DebugSetSeasonConfigCmdParams>
	{
		// Token: 0x06000238 RID: 568 RVA: 0x0000C205 File Offset: 0x0000A605
		public DebugSetSeasonConfigCmd(IDebugRatingSeasonService ratingSeasonService)
		{
			this.m_ratingSeasonService = ratingSeasonService;
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000C214 File Offset: 0x0000A614
		protected override void Execute(DebugSetSeasonConfigCmdParams param)
		{
			this.m_ratingSeasonService.SetupSeason(param.SeasonId, param.AnnouncementEndDate, param.GamesEndDate, param.SeasonActive);
			Log.Info("RatingSeason config was changed successfully");
		}

		// Token: 0x040000FD RID: 253
		private readonly IDebugRatingSeasonService m_ratingSeasonService;
	}
}
