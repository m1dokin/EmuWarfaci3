using System;
using HK2Net;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x02000002 RID: 2
	[Contract]
	public interface IClanMemberNicknameSynchronizer
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000001 RID: 1
		// (remove) Token: 0x06000002 RID: 2
		event Action<ClanMember> ClanMemberRenamed;
	}
}
