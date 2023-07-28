using System;

namespace MasterServer.DAL
{
	// Token: 0x02000085 RID: 133
	public class SAuthProfileSerializer : IDBSerializer<SAuthProfile>
	{
		// Token: 0x0600018E RID: 398 RVA: 0x00004F7B File Offset: 0x0000337B
		public void Deserialize(IDataReaderEx reader, out SAuthProfile ret)
		{
			ret = default(SAuthProfile);
			ret.ProfileID = ulong.Parse(reader["id"].ToString());
			ret.Nickname = reader["nickname"].ToString();
		}
	}
}
