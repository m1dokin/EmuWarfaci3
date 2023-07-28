using System;

namespace MasterServer.DAL
{
	// Token: 0x02000041 RID: 65
	public class VersionStampSerializer : IDBSerializer<SVersionStamp>
	{
		// Token: 0x0600009C RID: 156 RVA: 0x00003B81 File Offset: 0x00001F81
		public void Deserialize(IDataReaderEx reader, out SVersionStamp ret)
		{
			ret = default(SVersionStamp);
			ret.DataGroup = reader["data_group"].ToString();
			ret.Hash = reader["hash"].ToString();
		}
	}
}
