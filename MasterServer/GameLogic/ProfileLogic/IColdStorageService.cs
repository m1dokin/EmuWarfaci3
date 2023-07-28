using System;
using HK2Net;
using MasterServer.DAL;
using MasterServer.DAL.Utils;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000548 RID: 1352
	[Contract]
	public interface IColdStorageService
	{
		// Token: 0x06001D2F RID: 7471
		bool? IsProfileCold(ulong profile_id);

		// Token: 0x06001D30 RID: 7472
		TouchProfileResult TouchProfile(ulong profile_id, DBVersion current_version);

		// Token: 0x06001D31 RID: 7473
		bool ArchiveProfile(ulong profile_id, TimeSpan threshold);

		// Token: 0x06001D32 RID: 7474
		void ArchiveProfiles(TimeSpan threshold, int batch_size);

		// Token: 0x06001D33 RID: 7475
		void DebugRestoreAllProfiles(int batch_size);

		// Token: 0x06001D34 RID: 7476
		DateTime DebugUpdateLastSeenDate(ulong profileId, DateTime lastSeenDate);
	}
}
