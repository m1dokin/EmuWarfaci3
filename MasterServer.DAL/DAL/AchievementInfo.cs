using System;

namespace MasterServer.DAL
{
	// Token: 0x02000008 RID: 8
	[Serializable]
	public struct AchievementInfo
	{
		// Token: 0x06000012 RID: 18 RVA: 0x0000226C File Offset: 0x0000066C
		public override string ToString()
		{
			return string.Format("Id: {0}, Progress: {1}, CompletionTime: {2}", this.ID, this.Progress, this.CompletionTime);
		}

		// Token: 0x04000010 RID: 16
		public int ID;

		// Token: 0x04000011 RID: 17
		public int Progress;

		// Token: 0x04000012 RID: 18
		public ulong CompletionTime;
	}
}
