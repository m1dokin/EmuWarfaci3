using System;
using HK2Net;
using MasterServer.DAL;
using MasterServer.GameLogic.GameInterface;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.Users
{
	// Token: 0x02000760 RID: 1888
	[Contract]
	internal interface IUserFactory
	{
		// Token: 0x0600273E RID: 10046
		UserInfo.User Create(ulong uid, ulong pid, string nick, string jid, string token, string ip, string buildType, string regionId, AccessLevel accessLevel, DateTime loginTime, ulong exp, int rank, int ping, SBannerInfo banner, ProfileProgressionInfo profileProgression, ClientVersion version);

		// Token: 0x0600273F RID: 10047
		UserInfo.User Create(ulong pid, string jid, string token, string ip, string buildType, string regionId, SProfileInfo profileInfo, AccessLevel accessLevel, DateTime loginTime, ProfileProgressionInfo profileProgression, ClientVersion version);
	}
}
