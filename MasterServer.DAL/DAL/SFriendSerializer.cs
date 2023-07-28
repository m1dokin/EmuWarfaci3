using System;

namespace MasterServer.DAL
{
	// Token: 0x02000081 RID: 129
	public class SFriendSerializer : IDBSerializer<SFriend>
	{
		// Token: 0x06000189 RID: 393 RVA: 0x00004EBC File Offset: 0x000032BC
		public void Deserialize(IDataReaderEx reader, out SFriend ret)
		{
			ret = default(SFriend);
			ret.ProfileID = ulong.Parse(reader["profile_id"].ToString());
			ret.Nickname = reader["nickname"].ToString();
		}
	}
}
