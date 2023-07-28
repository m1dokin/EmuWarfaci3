using System;
using HK2Net;
using MasterServer.GameLogic.Achievements;
using Util.Common;

namespace MasterServer.GameLogic.ItemsSystem.ItemsPurchaseHandler.MetaGameActions
{
	// Token: 0x0200031E RID: 798
	[Service]
	internal class UnlockAchievementAction : IMetaGameAction
	{
		// Token: 0x06001224 RID: 4644 RVA: 0x00048148 File Offset: 0x00046548
		public UnlockAchievementAction(IAchievementSystem achievementSystem)
		{
			this.m_achievementSystem = achievementSystem;
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06001225 RID: 4645 RVA: 0x00048157 File Offset: 0x00046557
		public string Name
		{
			get
			{
				return "on_activate.unlock_achievement";
			}
		}

		// Token: 0x06001226 RID: 4646 RVA: 0x00048160 File Offset: 0x00046560
		public void Execute(ulong profileId, string action)
		{
			uint id = Convert.ToUInt32(action);
			AchievementDescription achievementDesc = this.m_achievementSystem.GetAchievementDesc(id);
			ulong completionTime = TimeUtils.LocalTimeToUTCTimestamp(DateTime.UtcNow);
			AchievementUpdateChunk achievementUpdateChunk = new AchievementUpdateChunk(achievementDesc.Id, (int)achievementDesc.Amount, completionTime);
			this.m_achievementSystem.SetAchievementProgress(profileId, achievementDesc, ref achievementUpdateChunk);
		}

		// Token: 0x04000854 RID: 2132
		private readonly IAchievementSystem m_achievementSystem;
	}
}
