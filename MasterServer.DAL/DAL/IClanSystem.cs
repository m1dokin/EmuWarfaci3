using System;

namespace MasterServer.DAL
{
	// Token: 0x02000014 RID: 20
	public interface IClanSystem
	{
		// Token: 0x0600002C RID: 44
		DALResultMulti<ClanMember> GetClanMembers(ulong clan_id);

		// Token: 0x0600002D RID: 45
		DALResult<EAddMemberResult> AddClanMember(ulong clan_id, ulong profile_id, uint limit);

		// Token: 0x0600002E RID: 46
		DALResult<ulong> RemoveClanMember(ulong clan_id, ulong profile_id);

		// Token: 0x0600002F RID: 47
		DALResultVoid KickClanMember(ulong clan_id, ulong profile_id);

		// Token: 0x06000030 RID: 48
		DALResultMulti<SClanKick> GetClanKicks(ulong profile_id);

		// Token: 0x06000031 RID: 49
		DALResultMulti<ClanInfo> GetClanTop(int limit);

		// Token: 0x06000032 RID: 50
		DALResultMulti<ulong> GetClansForLeaderboardPrediction();

		// Token: 0x06000033 RID: 51
		DALResult<ClanInfo> GetClanInfo(ulong clan_id);

		// Token: 0x06000034 RID: 52
		DALResult<ulong> GetClanIdByName(string clanName);

		// Token: 0x06000035 RID: 53
		DALResult<ClanMember> GetMemberInfo(ulong profile_id);

		// Token: 0x06000036 RID: 54
		DALResult<ulong> CreateClan(ulong master_pid, string clan_name, string description);

		// Token: 0x06000037 RID: 55
		DALResultVoid RemoveClan(ulong clan_id);

		// Token: 0x06000038 RID: 56
		DALResultVoid SetClanInfo(ulong clan_id, string description);

		// Token: 0x06000039 RID: 57
		DALResult<ulong> AddClanPoints(ulong clan_id, ulong profile_id, ulong clan_points);

		// Token: 0x0600003A RID: 58
		DALResultVoid SetUserClanRole(ulong clan_id, ulong master_id, ulong profile_id, uint role);

		// Token: 0x0600003B RID: 59
		DALResult<uint> FixupClans();

		// Token: 0x0600003C RID: 60
		DALResult<bool> FixupClan(ulong clan_id);

		// Token: 0x0600003D RID: 61
		DALResultVoid FixupClansMasters();

		// Token: 0x0600003E RID: 62
		DALResultVoid DebugGenerateClans(uint count);

		// Token: 0x0600003F RID: 63
		DALResultVoid DebugDeleteAllClans();
	}
}
