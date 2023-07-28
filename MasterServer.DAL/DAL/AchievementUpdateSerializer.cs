using System;

namespace MasterServer.DAL
{
	// Token: 0x0200000B RID: 11
	public class AchievementUpdateSerializer : IDBSerializer<SAchievementUpdate>
	{
		// Token: 0x06000016 RID: 22 RVA: 0x00002314 File Offset: 0x00000714
		public void Deserialize(IDataReaderEx reader, out SAchievementUpdate ret)
		{
			ret = default(SAchievementUpdate);
			ret.Info.ID = int.Parse(reader["achievement_id"].ToString());
			ret.Info.Progress = int.Parse(reader["progress"].ToString());
			ret.Info.CompletionTime = ulong.Parse(reader["completion_time"].ToString());
			int num = int.Parse(reader["updated"].ToString());
			ret.Status = ((ret.Info.CompletionTime != 0UL) ? ((num <= 0) ? EAchevementStatus.AlreadyCompleted : EAchevementStatus.JustCompleted) : EAchevementStatus.InProgress);
		}
	}
}
