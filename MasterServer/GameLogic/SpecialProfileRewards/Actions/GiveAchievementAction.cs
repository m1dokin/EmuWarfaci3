using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.GameLogic.Achievements;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameLogic.SpecialProfileRewards.Actions
{
	// Token: 0x020005C0 RID: 1472
	[SpecialRewardAction("achievement")]
	internal class GiveAchievementAction : SpecialRewardAction
	{
		// Token: 0x06001F90 RID: 8080 RVA: 0x00080A1C File Offset: 0x0007EE1C
		public GiveAchievementAction(ConfigSection config, IAchievementSystem achievementSystem, IRewardService rewardService) : base(config)
		{
			this.m_achievementSystem = achievementSystem;
			this.m_rewardService = rewardService;
			config.Get("id", out this.m_achievementId);
			config.Get("progress", out this.m_progress);
		}

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06001F91 RID: 8081 RVA: 0x00080A55 File Offset: 0x0007EE55
		public override string PrizeName
		{
			get
			{
				return string.Format("achievement: {0}", this.m_achievementId);
			}
		}

		// Token: 0x06001F92 RID: 8082 RVA: 0x00080A6C File Offset: 0x0007EE6C
		public override SNotification Activate(ulong profileId, ILogGroup logGroup, XmlElement userData)
		{
			Dictionary<uint, AchievementDescription> allAchievementDescs = this.m_achievementSystem.GetAllAchievementDescs();
			AchievementDescription achievementDescription;
			if (!allAchievementDescs.TryGetValue(this.m_achievementId, out achievementDescription))
			{
				throw new ApplicationException(string.Format("Achievement {0} not found", this.m_achievementId));
			}
			SNotification result = null;
			AchievementUpdateChunk data = new AchievementUpdateChunk(achievementDescription.Id, this.m_progress, 0UL);
			if (this.m_achievementSystem.UpdateAchievementProgress(profileId, achievementDescription, ref data))
			{
				result = base.CreateNotification<AchievementUpdateChunk>(ENotificationType.Achievement, data, this.m_rewardService.AwardExpirationTime, EConfirmationType.None);
			}
			return result;
		}

		// Token: 0x06001F93 RID: 8083 RVA: 0x00080AF4 File Offset: 0x0007EEF4
		public override string ToString()
		{
			return string.Format("achievement id:{0} progress:{1}", this.m_achievementId, this.m_progress);
		}

		// Token: 0x04000F5B RID: 3931
		private readonly uint m_achievementId;

		// Token: 0x04000F5C RID: 3932
		private readonly int m_progress;

		// Token: 0x04000F5D RID: 3933
		private readonly IAchievementSystem m_achievementSystem;

		// Token: 0x04000F5E RID: 3934
		private readonly IRewardService m_rewardService;
	}
}
