using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.RatingSystem.ConsoleCommands
{
	// Token: 0x02000410 RID: 1040
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "add_pvp_rating_points", Help = "Adds PvP rating points for given profile id")]
	internal class AddPvpRatingPointsCmd : ConsoleCommand<AddPvpRatingPointsCmdParams>
	{
		// Token: 0x0600166D RID: 5741 RVA: 0x0005E375 File Offset: 0x0005C775
		public AddPvpRatingPointsCmd(IRatingService ratingService)
		{
			this.m_ratingService = ratingService;
		}

		// Token: 0x0600166E RID: 5742 RVA: 0x0005E384 File Offset: 0x0005C784
		protected override void Execute(AddPvpRatingPointsCmdParams param)
		{
			this.m_ratingService.AddRatingPoints(param.ProfileId, param.Points, 0, string.Empty);
			Rating rating = this.m_ratingService.GetRating(param.ProfileId);
			Log.Info<ulong, Rating>("Success. Profile {0} has {1}", param.ProfileId, rating);
		}

		// Token: 0x04000AE3 RID: 2787
		private readonly IRatingService m_ratingService;
	}
}
