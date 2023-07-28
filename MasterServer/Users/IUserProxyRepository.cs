using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.Users
{
	// Token: 0x02000802 RID: 2050
	[Contract]
	internal interface IUserProxyRepository
	{
		// Token: 0x06002A08 RID: 10760
		UserInfo.User GetUserOrProxyByUserId(ulong userId);

		// Token: 0x06002A09 RID: 10761
		UserInfo.User GetUserOrProxyByUserId(ulong userId, bool full);

		// Token: 0x06002A0A RID: 10762
		UserInfo.User GetUserOrProxyByProfileId(ulong profileId);

		// Token: 0x06002A0B RID: 10763
		IEnumerable<UserInfo.User> GetUserOrProxyByProfileId(IEnumerable<ulong> profileIds);

		// Token: 0x06002A0C RID: 10764
		UserInfo.User GetUserOrProxyByProfileId(ulong profileId, bool full);

		// Token: 0x06002A0D RID: 10765
		IEnumerable<UserInfo.User> GetUserOrProxyByProfileId(IEnumerable<ulong> profileIds, bool full);

		// Token: 0x06002A0E RID: 10766
		UserInfo.User GetUserOrProxyByNickname(string nickname);

		// Token: 0x06002A0F RID: 10767
		UserInfo.User GetUserOrProxyByNickname(string nickname, bool full);
	}
}
