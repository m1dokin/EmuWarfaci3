using System;
using HK2Net;

namespace MasterServer.Users
{
	// Token: 0x020006E5 RID: 1765
	[Contract]
	public interface ITagService
	{
		// Token: 0x06002509 RID: 9481
		UserTags GetUserTags(ulong userId);

		// Token: 0x0600250A RID: 9482
		void AddUserTags(ulong userId, UserTags tags);

		// Token: 0x0600250B RID: 9483
		void SetUserTags(ulong userId, UserTags tags);

		// Token: 0x0600250C RID: 9484
		void RemoveUserTags(ulong userId, UserTags tags);

		// Token: 0x0600250D RID: 9485
		void RemoveUserTags(ulong userId);

		// Token: 0x0600250E RID: 9486
		void SyncUserTags(ulong userId);
	}
}
