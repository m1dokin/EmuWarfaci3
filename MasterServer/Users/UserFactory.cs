using System;
using HK2Net;
using MasterServer.DAL;
using MasterServer.GameLogic.GameInterface;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.Users
{
	// Token: 0x02000762 RID: 1890
	[Service]
	[Singleton]
	internal class UserFactory : IUserFactory
	{
		// Token: 0x06002742 RID: 10050 RVA: 0x000A5CC4 File Offset: 0x000A40C4
		public UserInfo.User Create(ulong uid, ulong pid, string nick, string jid, string token, string ip, string buildType, string regionId, AccessLevel accessLevel, DateTime loginTime, ulong exp, int rank, int ping, SBannerInfo banner, ProfileProgressionInfo profileProgression, ClientVersion version)
		{
			return new UserInfo.User(uid, pid, nick, jid, token, ip, buildType, regionId, accessLevel, loginTime, exp, rank, banner, profileProgression, version);
		}

		// Token: 0x06002743 RID: 10051 RVA: 0x000A5CF4 File Offset: 0x000A40F4
		public UserInfo.User Create(ulong pid, string jid, string token, string ip, string buildType, string regionId, SProfileInfo profileInfo, AccessLevel accessLevel, DateTime loginTime, ProfileProgressionInfo profileProgression, ClientVersion version)
		{
			return new UserInfo.User(pid, jid, token, ip, buildType, regionId, profileInfo, accessLevel, loginTime, profileProgression, version);
		}
	}
}
