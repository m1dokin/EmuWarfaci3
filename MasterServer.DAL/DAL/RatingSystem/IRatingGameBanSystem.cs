using System;

namespace MasterServer.DAL.RatingSystem
{
	// Token: 0x0200008E RID: 142
	public interface IRatingGameBanSystem
	{
		// Token: 0x060001AC RID: 428
		DALResultVoid BanRatingGameForPlayer(ulong profileId, TimeSpan banTimeout);

		// Token: 0x060001AD RID: 429
		DALResultVoid UnbanRatingGameForPlayer(ulong profileId);

		// Token: 0x060001AE RID: 430
		DALResult<RatingGamePlayerBanInfo> GetPlayerBanInfo(ulong profileId);
	}
}
