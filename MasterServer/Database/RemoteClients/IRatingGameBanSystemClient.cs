using System;
using MasterServer.DAL.RatingSystem;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000044 RID: 68
	internal interface IRatingGameBanSystemClient
	{
		// Token: 0x06000115 RID: 277
		void BanRatingGameForPlayer(ulong profileId, TimeSpan unbanTime);

		// Token: 0x06000116 RID: 278
		void UnbanRatingGameForPlayer(ulong profileId);

		// Token: 0x06000117 RID: 279
		RatingGamePlayerBanInfo GetPlayerBanInfo(ulong profileId);
	}
}
