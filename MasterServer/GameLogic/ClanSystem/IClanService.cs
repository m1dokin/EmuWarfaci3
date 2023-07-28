using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.DAL;
using MasterServer.GameLogic.InvitationSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x0200027A RID: 634
	[Contract]
	internal interface IClanService
	{
		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06000DA1 RID: 3489
		uint MaxClanSize { get; }

		// Token: 0x06000DA2 RID: 3490
		EClanCreationStatus CreateClan(ulong clanOwnerId, ref ulong clanID, string clanName, string description);

		// Token: 0x06000DA3 RID: 3491
		EClanCreationStatus CreateClanWithoutItem(ulong clanOwnerId, ref ulong clanID, string clanName, string description);

		// Token: 0x06000DA4 RID: 3492
		bool RemoveClan(ulong initiator_id);

		// Token: 0x06000DA5 RID: 3493
		ClanInfo GetClanInfo(ulong clan_id);

		// Token: 0x06000DA6 RID: 3494
		ClanInfo GetClanInfoByName(string clanName);

		// Token: 0x06000DA7 RID: 3495
		ClanMember GetMemberInfo(ulong profile_id);

		// Token: 0x06000DA8 RID: 3496
		ClanInfo GetClanInfoByPid(ulong profile_id);

		// Token: 0x06000DA9 RID: 3497
		double GetKickTime(ulong clan_id, ulong profile_id);

		// Token: 0x06000DAA RID: 3498
		IEnumerable<ClanMember> GetClanMembers(ulong clan_id);

		// Token: 0x06000DAB RID: 3499
		bool SetClanInfo(ulong initiator_id, string description);

		// Token: 0x06000DAC RID: 3500
		EAddMemberResult AddClanMember(ulong clan_id, ulong profile_id);

		// Token: 0x06000DAD RID: 3501
		void AddClanPoints(ulong target_id, ulong clan_points);

		// Token: 0x06000DAE RID: 3502
		bool SetClanRole(ulong initiator_id, ulong target_id, EClanRole role);

		// Token: 0x06000DAF RID: 3503
		bool KickFromClan(ulong initiator_id, ulong target_id);

		// Token: 0x06000DB0 RID: 3504
		bool LeaveClan(ulong initiator_id);

		// Token: 0x06000DB1 RID: 3505
		uint FixupClans();

		// Token: 0x06000DB2 RID: 3506
		Task<EInviteStatus> Invite(UserInfo.User initiatorId, ulong targetId, string targetNickname);

		// Token: 0x06000DB3 RID: 3507
		Task<EInviteStatus> Invite(ulong sourceId, ulong targetId);

		// Token: 0x06000DB4 RID: 3508
		void RemoteClanInfoUpdate(ulong profileId);

		// Token: 0x1400002B RID: 43
		// (add) Token: 0x06000DB5 RID: 3509
		// (remove) Token: 0x06000DB6 RID: 3510
		event ClanCreatedDelegate ClanCreated;

		// Token: 0x1400002C RID: 44
		// (add) Token: 0x06000DB7 RID: 3511
		// (remove) Token: 0x06000DB8 RID: 3512
		event ClanRemovedDelegate ClanRemoved;

		// Token: 0x1400002D RID: 45
		// (add) Token: 0x06000DB9 RID: 3513
		// (remove) Token: 0x06000DBA RID: 3514
		event ClanDescriptionUpdatedDelegate ClanDescriptionUpdated;

		// Token: 0x1400002E RID: 46
		// (add) Token: 0x06000DBB RID: 3515
		// (remove) Token: 0x06000DBC RID: 3516
		event ClanMemberListUpdatedDelegate ClanMemberListUpdated;
	}
}
