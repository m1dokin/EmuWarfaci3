using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.RatingSystem.ConsoleCommands
{
	// Token: 0x0200040E RID: 1038
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "get_pvp_rating", Help = "Gets PvP rating for given profile id")]
	internal class GetPvpRatingCmd : ConsoleCommand<GetPvpRatingCmdParams>
	{
		// Token: 0x06001668 RID: 5736 RVA: 0x0005E31A File Offset: 0x0005C71A
		public GetPvpRatingCmd(IRatingService ratingService)
		{
			this.m_ratingService = ratingService;
		}

		// Token: 0x06001669 RID: 5737 RVA: 0x0005E32C File Offset: 0x0005C72C
		protected override void Execute(GetPvpRatingCmdParams param)
		{
			Rating rating = this.m_ratingService.GetRating(param.ProfileId);
			Log.Info<ulong, Rating>("Profile {0} has {1}", param.ProfileId, rating);
		}

		// Token: 0x04000AE1 RID: 2785
		private readonly IRatingService m_ratingService;
	}
}
