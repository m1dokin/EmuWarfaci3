using System;

namespace MasterServer.DAL
{
	// Token: 0x0200003F RID: 63
	public class ServerEntitySerializer : IDBSerializer<SServerEntity>
	{
		// Token: 0x0600009A RID: 154 RVA: 0x00003A64 File Offset: 0x00001E64
		public void Deserialize(IDataReaderEx reader, out SServerEntity ret)
		{
			ret = default(SServerEntity);
			ret.ServerId = reader["server"].ToString();
			ret.Hostname = reader["host"].ToString();
			ret.Port = int.Parse(reader["port"].ToString());
			ret.Node = reader["node"].ToString();
			ret.OnlineId = reader["online"].ToString();
			ret.Status = int.Parse(reader["status"].ToString());
			ret.MissionKey = reader["mission"].ToString();
			ret.Mode = reader["mode"].ToString();
			ret.PerformanceIndex = float.Parse(reader["performance"].ToString());
			ret.BuildType = reader["build_type"].ToString();
			ret.MasterServerId = reader["msid"].ToString();
		}
	}
}
