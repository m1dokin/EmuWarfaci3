using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.RatingSystem.ConsoleCommands
{
	// Token: 0x020000C4 RID: 196
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "update_season_for_player", Help = "Sets player to current season, and resets rating points")]
	internal class UpdateSeasonForPlayerCmd : ConsoleCommand<UpdateSeasonForPlayerCmdParams>
	{
		// Token: 0x0600032C RID: 812 RVA: 0x0000EDCC File Offset: 0x0000D1CC
		public UpdateSeasonForPlayerCmd(IRatingSeasonService ratingSeasonService)
		{
			this.m_ratingSeasonService = ratingSeasonService;
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0000EDDC File Offset: 0x0000D1DC
		protected override void Execute(UpdateSeasonForPlayerCmdParams param)
		{
			this.m_ratingSeasonService.UpdateSeasonForPlayer(param.ProfileId);
			Rating playerRating = this.m_ratingSeasonService.GetPlayerRating(param.ProfileId);
			Log.Info<ulong, uint>("Success. Profile {0} has {1} rating points in new season", param.ProfileId, playerRating.Points);
		}

		// Token: 0x0400015A RID: 346
		private readonly IRatingSeasonService m_ratingSeasonService;
	}
}
