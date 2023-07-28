using System;

namespace MasterServer.DAL
{
	// Token: 0x02000037 RID: 55
	public class SoftShuffleElementSerializer : IDBSerializer<SoftShufflePoolElement>
	{
		// Token: 0x0600008D RID: 141 RVA: 0x00003570 File Offset: 0x00001970
		public void Deserialize(IDataReaderEx reader, out SoftShufflePoolElement ret)
		{
			ret = new SoftShufflePoolElement(string.Empty);
			ret.Key = reader["element_key"].ToString();
			ret.Pos = int.Parse(reader["element_pos"].ToString());
			ret.UsageCount = int.Parse(reader["element_usage_count"].ToString());
		}
	}
}
