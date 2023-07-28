using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameLogic.Achievements
{
	// Token: 0x02000256 RID: 598
	[ConsoleCmdAttributes(CmdName = "achievements_update", ArgsSize = 3, Help = "Updates achievement progress: <pid> <ach> <delta>")]
	internal class AchievementsUpdateCmd : IConsoleCmd
	{
		// Token: 0x06000D33 RID: 3379 RVA: 0x000340E8 File Offset: 0x000324E8
		public AchievementsUpdateCmd(IAchievementSystem achievementSystem, INotificationService notificationService, IRewardService rewardService)
		{
			this.m_achievementSystem = achievementSystem;
			this.m_notificationService = notificationService;
			this.m_rewardService = rewardService;
		}

		// Token: 0x06000D34 RID: 3380 RVA: 0x00034108 File Offset: 0x00032508
		public void ExecuteCmd(string[] args)
		{
			ulong profileId = ulong.Parse(args[1]);
			uint num = uint.Parse(args[2]);
			int pr = int.Parse(args[3]);
			Dictionary<uint, AchievementDescription> allAchievementDescs = this.m_achievementSystem.GetAllAchievementDescs();
			if (!allAchievementDescs.ContainsKey(num))
			{
				Log.Error<uint>("Achievement description for ID={0} was not found in config (achievements_data.xml)", num);
				return;
			}
			AchievementDescription achievementDescription = allAchievementDescs[num];
			AchievementUpdateChunk data = new AchievementUpdateChunk(achievementDescription.Id, pr, 0UL);
			if (this.m_achievementSystem.UpdateAchievementProgress(profileId, achievementDescription, ref data))
			{
				this.m_notificationService.AddNotification<AchievementUpdateChunk>(profileId, ENotificationType.Achievement, data, this.m_rewardService.AwardExpirationTime, EDeliveryType.SendNowOrLater, EConfirmationType.None).Wait();
			}
			Log.Info("Updated id:{0} progress:{1}/{2} completion_time:{3}", new object[]
			{
				data.achievementId,
				data.progress,
				achievementDescription.Amount,
				data.completionTime
			});
		}

		// Token: 0x04000611 RID: 1553
		private readonly IAchievementSystem m_achievementSystem;

		// Token: 0x04000612 RID: 1554
		private readonly INotificationService m_notificationService;

		// Token: 0x04000613 RID: 1555
		private readonly IRewardService m_rewardService;
	}
}
