using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.RatingSystem.ConsoleCommands
{
	// Token: 0x020000EA RID: 234
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "set_pvp_rating_points", Help = "Sets PvP rating points for given profile id")]
	internal class SetPvpRatingPointsCmd : ConsoleCommand<SetPvpRatingPointsCmdParams>
	{
		// Token: 0x060003D4 RID: 980 RVA: 0x00010CA1 File Offset: 0x0000F0A1
		public SetPvpRatingPointsCmd(IRatingService ratingService)
		{
			this.m_ratingService = ratingService;
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00010CB0 File Offset: 0x0000F0B0
		protected override void Execute(SetPvpRatingPointsCmdParams param)
		{
			this.m_ratingService.SetRatingPoints(param.ProfileId, param.Points);
			Rating rating = this.m_ratingService.GetRating(param.ProfileId);
			Log.Info<ulong, Rating>("Success. Profile {0} has {1}", param.ProfileId, rating);
		}

		// Token: 0x0400019B RID: 411
		private readonly IRatingService m_ratingService;
	}
}
