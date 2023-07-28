using System;
using System.Collections.Generic;
using MasterServer.DAL;
using MasterServer.DAL.Utils;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001F5 RID: 501
	public interface IColdStorageSystemClient
	{
		// Token: 0x060009F3 RID: 2547
		bool? IsProfileCold(ulong profile_id);

		// Token: 0x060009F4 RID: 2548
		TouchProfileResult TouchProfile(ulong profile_id, DBVersion current_schema);

		// Token: 0x060009F5 RID: 2549
		bool MoveProfileToCold(ulong profile_id, TimeSpan threshold, DBVersion current_schema);

		// Token: 0x060009F6 RID: 2550
		ColdProfileData GetColdProfileData(ulong profile_id, DBVersion current_schema);

		// Token: 0x060009F7 RID: 2551
		IEnumerable<ulong> GetUnusedProfiles(TimeSpan threshold, int limit);

		// Token: 0x060009F8 RID: 2552
		IEnumerable<ulong> GetColdProfiles(int limit);
	}
}
