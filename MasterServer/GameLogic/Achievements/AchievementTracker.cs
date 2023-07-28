using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.StatsTracking;

namespace MasterServer.GameLogic.Achievements
{
	// Token: 0x02000257 RID: 599
	public class AchievementTracker
	{
		// Token: 0x06000D36 RID: 3382 RVA: 0x000341F8 File Offset: 0x000325F8
		public void Init()
		{
			IStatsTracker service = ServicesManager.GetService<IStatsTracker>();
			service.OnStatisticsChanged += this.OnStatisticsChanged;
			service.OnStatisticsReset += this.OnStatisticsReset;
		}

		// Token: 0x06000D37 RID: 3383 RVA: 0x00034230 File Offset: 0x00032630
		public void Clear()
		{
			IStatsTracker service = ServicesManager.GetService<IStatsTracker>();
			service.OnStatisticsChanged -= this.OnStatisticsChanged;
			service.OnStatisticsReset -= this.OnStatisticsReset;
		}

		// Token: 0x06000D38 RID: 3384 RVA: 0x00034268 File Offset: 0x00032668
		private void DoStatisticsReset(ulong profileId, EStatsEvent eventId, List<IStatsFilter> filters, int value, bool addToCurrentProgress)
		{
			IAchievementSystem service = ServicesManager.GetService<IAchievementSystem>();
			Dictionary<uint, AchievementDescription> allAchievementDescs = service.GetAllAchievementDescs();
			List<AchievementUpdateChunk> list = new List<AchievementUpdateChunk>();
			foreach (AchievementDescription achievementDescription in allAchievementDescs.Values)
			{
				if (achievementDescription.MasterServerSide && achievementDescription.Kind == eventId && StatsTracker.IsFiltersMatch(filters, achievementDescription.Filters))
				{
					AchievementUpdateChunk item = new AchievementUpdateChunk(achievementDescription.Id, value, 0UL);
					bool flag = (!addToCurrentProgress) ? service.SetAchievementProgress(profileId, achievementDescription, ref item) : service.UpdateAchievementProgress(profileId, achievementDescription, ref item);
					if (flag)
					{
						list.Add(item);
					}
				}
			}
			if (list.Count > 0)
			{
				INotificationService service2 = ServicesManager.GetService<INotificationService>();
				IRewardService service3 = ServicesManager.GetService<IRewardService>();
				service2.AddNotifications<AchievementUpdateChunk>(profileId, ENotificationType.Achievement, list, service3.AwardExpirationTime, EDeliveryType.SendNowOrLater, EConfirmationType.None);
			}
		}

		// Token: 0x06000D39 RID: 3385 RVA: 0x0003436C File Offset: 0x0003276C
		private void OnStatisticsChanged(ulong profileId, EStatsEvent eventId, List<IStatsFilter> filters, int delta)
		{
			this.DoStatisticsReset(profileId, eventId, filters, delta, true);
		}

		// Token: 0x06000D3A RID: 3386 RVA: 0x0003437A File Offset: 0x0003277A
		private void OnStatisticsReset(ulong profileId, EStatsEvent eventId, List<IStatsFilter> filters, int value)
		{
			this.DoStatisticsReset(profileId, eventId, filters, value, false);
		}
	}
}
