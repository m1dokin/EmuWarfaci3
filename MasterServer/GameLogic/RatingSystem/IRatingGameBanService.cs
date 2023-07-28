using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL.RatingSystem;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x020000C2 RID: 194
	[Contract]
	public interface IRatingGameBanService
	{
		// Token: 0x0600031C RID: 796
		void BanRatingGameForPlayers(IEnumerable<ulong> profileIds);

		// Token: 0x0600031D RID: 797
		void BanRatingGameForPlayers(IEnumerable<ulong> profileIds, TimeSpan banTimeout, string msg = "");

		// Token: 0x0600031E RID: 798
		void UnbanRatingGameForPlayers(IEnumerable<ulong> profileIds);

		// Token: 0x0600031F RID: 799
		bool IsPlayerBanned(ulong profileId);

		// Token: 0x06000320 RID: 800
		RatingGamePlayerBanInfo GetPlayerBanInfo(ulong profileId);
	}
}
