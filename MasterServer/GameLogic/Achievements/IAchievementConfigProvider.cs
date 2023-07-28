using System;
using HK2Net;

namespace MasterServer.GameLogic.Achievements
{
	// Token: 0x0200005A RID: 90
	[Contract]
	public interface IAchievementConfigProvider
	{
		// Token: 0x06000163 RID: 355
		AchievementConfig GetConfing();
	}
}
