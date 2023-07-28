using System;

namespace MasterServer.DAL
{
	// Token: 0x0200000A RID: 10
	public class AchievementInfoSerializer : IDBSerializer<AchievementInfo>
	{
		// Token: 0x06000014 RID: 20 RVA: 0x000022A4 File Offset: 0x000006A4
		public void Deserialize(IDataReaderEx reader, out AchievementInfo ret)
		{
			ret = default(AchievementInfo);
			ret.ID = int.Parse(reader["achievement_id"].ToString());
			ret.Progress = int.Parse(reader["progress"].ToString());
			ret.CompletionTime = ulong.Parse(reader["completion_time"].ToString());
		}
	}
}
