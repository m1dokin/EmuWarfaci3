using System;

namespace MasterServer.DAL
{
	// Token: 0x02000039 RID: 57
	public class SoftShufflePoolSerializer : IDBSerializer<SoftShufflePoolData>
	{
		// Token: 0x06000090 RID: 144 RVA: 0x000035F4 File Offset: 0x000019F4
		public void Deserialize(IDataReaderEx reader, out SoftShufflePoolData ret)
		{
			ret = new SoftShufflePoolData();
			ret.m_key = reader["pool_key"].ToString();
			ret.m_softShuffleIdx = int.Parse(reader["shuffle_idx"].ToString());
			ret.m_marker = int.Parse(reader["marker_pos"].ToString());
		}
	}
}
