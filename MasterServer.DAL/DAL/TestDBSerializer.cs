using System;

namespace MasterServer.DAL
{
	// Token: 0x02000067 RID: 103
	public class TestDBSerializer : IDBSerializer<string>
	{
		// Token: 0x060000FB RID: 251 RVA: 0x000046FC File Offset: 0x00002AFC
		public void Deserialize(IDataReaderEx reader, out string ret)
		{
			ret = reader["version"].ToString();
		}
	}
}
