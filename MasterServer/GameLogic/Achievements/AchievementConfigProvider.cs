using System;
using HK2Net;

namespace MasterServer.GameLogic.Achievements
{
	// Token: 0x02000058 RID: 88
	[Service]
	[Singleton]
	internal class AchievementConfigProvider : IAchievementConfigProvider
	{
		// Token: 0x06000153 RID: 339 RVA: 0x00009E03 File Offset: 0x00008203
		public AchievementConfigProvider()
		{
			this.m_achievementConfig = new AchievementConfig();
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00009E16 File Offset: 0x00008216
		public AchievementConfig GetConfing()
		{
			return this.m_achievementConfig;
		}

		// Token: 0x040000A0 RID: 160
		private readonly AchievementConfig m_achievementConfig;
	}
}
