using System;
using System.Text;

namespace MasterServer.DAL
{
	// Token: 0x02000044 RID: 68
	public class MissionSerializer : IDBSerializer<SMission>
	{
		// Token: 0x060000A9 RID: 169 RVA: 0x00003BC0 File Offset: 0x00001FC0
		public void Deserialize(IDataReaderEx reader, out SMission ret)
		{
			ret = default(SMission);
			ret.ID = (Guid)reader["id"];
			ret.Generation = int.Parse(reader["generation"].ToString());
			ret.Data = Encoding.ASCII.GetString((byte[])reader["data"]);
		}
	}
}
