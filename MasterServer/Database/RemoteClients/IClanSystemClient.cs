using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001F3 RID: 499
	internal interface IClanSystemClient
	{
		// Token: 0x060009C3 RID: 2499
		ClanMember GetMemberInfo(ulong profileId);

		// Token: 0x060009C4 RID: 2500
		IEnumerable<SClanKick> GetClanKicks(ulong profileId);

		// Token: 0x060009C5 RID: 2501
		ClanInfo GetClanInfo(ulong clanId);

		// Token: 0x060009C6 RID: 2502
		ClanInfo GetClanInfoByName(string clanName);

		// Token: 0x060009C7 RID: 2503
		IEnumerable<ClanInfo> GetClanTop(int limit);

		// Token: 0x060009C8 RID: 2504
		IEnumerable<ulong> GetClansForLeaderboardPrediction();

		// Token: 0x060009C9 RID: 2505
		ulong CreateClan(ulong profileId, string clanName, string description);

		// Token: 0x060009CA RID: 2506
		void RemoveClan(ulong clanId);

		// Token: 0x060009CB RID: 2507
		void SetClanInfo(ulong clanId, string description);

		// Token: 0x060009CC RID: 2508
		ulong AddClanPoints(ulong clanId, ulong targetId, ulong clanPoints);

		// Token: 0x060009CD RID: 2509
		IEnumerable<ClanMember> GetClanMembers(ulong clanId);

		// Token: 0x060009CE RID: 2510
		EAddMemberResult AddClanMember(ulong clanId, ulong profileId, uint limit);

		// Token: 0x060009CF RID: 2511
		void KickClanMember(ulong clanId, ulong profileId);

		// Token: 0x060009D0 RID: 2512
		ulong RemoveClanMember(ulong clanId, ulong profileId);

		// Token: 0x060009D1 RID: 2513
		void SetUserClanRole(ulong clanId, ulong masterId, ulong profileId, uint role);

		// Token: 0x060009D2 RID: 2514
		uint FixupClans();

		// Token: 0x060009D3 RID: 2515
		bool FixupClan(ulong clanId);

		// Token: 0x060009D4 RID: 2516
		void FixupClansMasters();

		// Token: 0x060009D5 RID: 2517
		void DebugGenerateClans(uint count);

		// Token: 0x060009D6 RID: 2518
		void FlushClanCacheForMember(ulong profileId);

		// Token: 0x060009D7 RID: 2519
		void DebugDeleteAllClans();
	}
}
