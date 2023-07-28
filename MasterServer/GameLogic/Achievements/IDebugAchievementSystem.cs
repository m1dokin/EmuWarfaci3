using System;
using HK2Net;

namespace MasterServer.GameLogic.Achievements
{
	// Token: 0x0200025A RID: 602
	[Contract]
	public interface IDebugAchievementSystem
	{
		// Token: 0x06000D41 RID: 3393
		void DeleteProfileAchievements(ulong profileId);
	}
}
