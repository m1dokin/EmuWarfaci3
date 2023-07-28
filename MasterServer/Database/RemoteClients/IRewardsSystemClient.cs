using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000211 RID: 529
	internal interface IRewardsSystemClient
	{
		// Token: 0x06000B7E RID: 2942
		IEnumerable<SSponsorPoints> GetSponsorPoints(ulong profileId);

		// Token: 0x06000B7F RID: 2943
		void SetSponsorPoints(ulong profileId, uint sponsorId, ulong sponsorPts);

		// Token: 0x06000B80 RID: 2944
		bool SetSponsorInfo(ulong profileId, uint sponsorId, ulong oldSpPts, SRankInfo new_sp);

		// Token: 0x06000B81 RID: 2945
		void SetNextUnlockItem(ulong profileId, uint sponsorId, ulong nextUnlockItemId);
	}
}
