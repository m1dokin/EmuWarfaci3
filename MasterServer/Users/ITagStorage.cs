using System;
using HK2Net;

namespace MasterServer.Users
{
	// Token: 0x0200073F RID: 1855
	[Contract]
	public interface ITagStorage
	{
		// Token: 0x0600263E RID: 9790
		string GetPersistentUserTags(ulong user_id);

		// Token: 0x0600263F RID: 9791
		void SetPersistentUserTags(ulong user_id, string tags);

		// Token: 0x06002640 RID: 9792
		void RemovePersistentUserTags(ulong user_id);
	}
}
