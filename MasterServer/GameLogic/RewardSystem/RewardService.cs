using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.PerformanceSystem;
using MasterServer.GameLogic.StatsTracking;
using MasterServer.Telemetry;
using MasterServer.Users;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005BD RID: 1469
	[Service]
	[Singleton]
	internal class RewardService : ServiceModule, IRewardService
	{
		// Token: 0x06001F80 RID: 8064 RVA: 0x0007FFC0 File Offset: 0x0007E3C0
		public RewardService(IProfileItems profileItemsService, IItemCache itemCacheService, ICatalogService catalogService, IStatsTracker statsTracker, IDALService dalService, ILogService logService, IUserRepository userRepository, IOnlineClient onlineClientService, IRewardCalculator rewardCalculator, INotificationService notificationService, ITelemetryService telemetryService, IMissionPerformanceService missionPerformanceService, List<IRewardProcessor> rewardProcessors)
		{
			this.m_profileItemsService = profileItemsService;
			this.m_itemCacheService = itemCacheService;
			this.m_catalogService = catalogService;
			this.m_statsTracker = statsTracker;
			this.m_rewardCalculator = rewardCalculator;
			this.m_notificationService = notificationService;
			this.m_telemetryService = telemetryService;
			this.m_missionPerformanceService = missionPerformanceService;
			this.m_dalService = dalService;
			this.m_logService = logService;
			this.m_userRepository = userRepository;
			this.m_onlineClientService = onlineClientService;
			this.m_rewarders = rewardProcessors;
			ConfigSection section = Resources.LBSettings.GetSection("PerformanceService");
			this.SetAwardExpirationTime(section.Get("AwardExpirationTime"));
			section.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x14000079 RID: 121
		// (add) Token: 0x06001F81 RID: 8065 RVA: 0x0008006C File Offset: 0x0007E46C
		// (remove) Token: 0x06001F82 RID: 8066 RVA: 0x000800A4 File Offset: 0x0007E4A4
		public event OnRewardsGivenDeleg OnRewardsGiven;

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06001F83 RID: 8067 RVA: 0x000800DA File Offset: 0x0007E4DA
		// (set) Token: 0x06001F84 RID: 8068 RVA: 0x000800E2 File Offset: 0x0007E4E2
		public TimeSpan AwardExpirationTime { get; private set; }

		// Token: 0x06001F85 RID: 8069 RVA: 0x000800EC File Offset: 0x0007E4EC
		public XmlElement GetUnlockedItemsXml(XmlDocument factory, ulong profileId, IEnumerable<SponsorDataUpdate.ItemIDs> unlockedItems)
		{
			Dictionary<ulong, SItem> allItems = this.m_itemCacheService.GetAllItems(true);
			XmlElement xmlElement = factory.CreateElement("unlocked_items");
			foreach (SponsorDataUpdate.ItemIDs itemIDs in unlockedItems)
			{
				SProfileItem profileItem = this.m_profileItemsService.GetProfileItem(profileId, itemIDs.profileItemId);
				if (profileItem != null)
				{
					XmlElement xml = ServerItem.GetXml(profileItem, factory, "item");
					xml.SetAttribute("profile_item_id", itemIDs.profileItemId.ToString(CultureInfo.InvariantCulture));
					xmlElement.AppendChild(xml);
				}
				else
				{
					SItem sitem = allItems[itemIDs.inventoryId];
					XmlElement xml2 = ServerItem.GetXml(sitem, factory, "item");
					xml2.SetAttribute("profile_item_id", "0");
					xml2.SetAttribute("name", sitem.Name);
					xmlElement.AppendChild(xml2);
				}
			}
			return xmlElement;
		}

		// Token: 0x06001F86 RID: 8070 RVA: 0x000801F4 File Offset: 0x0007E5F4
		public void RewardMoney(ulong userId, ulong profileId, Currency currency, ulong rewardMoney, ILogGroup logGroup, LogGroup.ProduceType rewardSource)
		{
			EStatsEvent eventId = EStatsEvent.NON_MS_EVENT;
			if (currency != Currency.GameMoney)
			{
				if (currency != Currency.CryMoney)
				{
					if (currency == Currency.CrownMoney)
					{
						eventId = EStatsEvent.CROWN_COLLECTED;
					}
				}
				else
				{
					eventId = EStatsEvent.CRYMONEY_AWARDED;
				}
			}
			else
			{
				eventId = EStatsEvent.MONEY_AWARDED;
			}
			this.m_catalogService.AddMoney(userId, currency, rewardMoney, string.Empty);
			this.m_statsTracker.ChangeStatistics(profileId, eventId, rewardMoney);
			logGroup.ShopMoneyChangedLog(userId, profileId, (long)((currency != Currency.GameMoney) ? 0UL : rewardMoney), (long)((currency != Currency.CryMoney) ? 0UL : rewardMoney), (long)((currency != Currency.CrownMoney) ? 0UL : rewardMoney), rewardSource, TransactionStatus.OK, string.Empty, string.Empty);
		}

		// Token: 0x06001F87 RID: 8071 RVA: 0x000802A0 File Offset: 0x0007E6A0
		public void GiveRewards(string sessionId, MissionContext missionContext, RewardInputData rewardData)
		{
			IEnumerable<RewardOutputData> enumerable = this.m_rewardCalculator.Calculate(rewardData, sessionId);
			List<RewardUpdateData> list = new List<RewardUpdateData>();
			this.UpdatePlayersPerformance(missionContext, rewardData);
			foreach (RewardOutputData rewardOutputData in enumerable)
			{
				using (ILogGroup logGroup = this.m_logService.CreateGroup())
				{
					SProfileInfo pi = this.m_dalService.ProfileSystem.GetProfileInfo(rewardOutputData.profileId);
					this.m_statsTracker.ChangeStatistics(rewardOutputData.profileId, EStatsEvent.SESSION_END, rewardOutputData.outcome);
					RewardUpdateData rewardUpdateData = new RewardUpdateData();
					RewardOutputData rewardOutputData2 = rewardOutputData;
					rewardOutputData2.userId = pi.UserID;
					IEnumerator enumerator2 = Enum.GetValues(typeof(RewardProcessorState)).GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							RewardProcessorState state = (RewardProcessorState)enumerator2.Current;
							rewardOutputData2 = this.m_rewarders.Aggregate(rewardOutputData2, (RewardOutputData current, IRewardProcessor rewarder) => rewarder.ProcessRewardData(pi.UserID, state, missionContext, current, logGroup));
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator2 as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
					this.RewardMoney(pi.UserID, rewardOutputData2.profileId, Currency.GameMoney, (ulong)rewardOutputData2.gainedMoney, logGroup, LogGroup.ProduceType.Reward);
					ulong accumulatedReward = rewardOutputData2.crownReward.GetAccumulatedReward();
					if (accumulatedReward > 0UL)
					{
						this.RewardMoney(pi.UserID, rewardOutputData2.profileId, Currency.CrownMoney, accumulatedReward, logGroup, LogGroup.ProduceType.CrownReward);
						IEnumerator enumerator3 = rewardOutputData2.crownReward.GetEnumerator();
						try
						{
							while (enumerator3.MoveNext())
							{
								object obj = enumerator3.Current;
								ProfileStatCrownReward profileStatCrownReward = (ProfileStatCrownReward)obj;
								logGroup.CrownMoneyGivenLog(pi.UserID, rewardOutputData2.profileId, missionContext.missionType.Name, (CrownRewardThreshold.PerformanceCategory)profileStatCrownReward.Stat, profileStatCrownReward.Reward, DateTime.UtcNow);
							}
						}
						finally
						{
							IDisposable disposable2;
							if ((disposable2 = (enumerator3 as IDisposable)) != null)
							{
								disposable2.Dispose();
							}
						}
					}
					rewardUpdateData.nickname = pi.Nickname;
					rewardUpdateData.rewards = rewardOutputData2;
					rewardUpdateData.mission = new Guid(rewardData.missionId);
					rewardUpdateData.status = ((rewardOutputData.outcome != SessionOutcome.Won) ? MissionStatus.Failed : MissionStatus.Finished);
					list.Add(rewardUpdateData);
				}
			}
			this.FireOnRewardsGivenEvent(sessionId, list);
			this.NotifyUsers(list);
		}

		// Token: 0x06001F88 RID: 8072 RVA: 0x000805F0 File Offset: 0x0007E9F0
		private void UpdatePlayersPerformance(MissionContext missionContext, RewardInputData inputData)
		{
			if (!inputData.teams.Any<KeyValuePair<byte, RewardInputData.Team>>())
			{
				return;
			}
			int playerTeamId = (int)inputData.teams.Keys.FirstOrDefault<byte>();
			SessionOutcome outcome = RewardCalculatorHelper.GetOutcome(missionContext, inputData.winnerTeamId, playerTeamId);
			if (outcome == SessionOutcome.Stopped)
			{
				return;
			}
			PerformanceUpdate upd;
			upd.MissionID = new Guid(inputData.missionId);
			upd.Status = ((outcome != SessionOutcome.Won) ? MissionStatus.Failed : MissionStatus.Finished);
			upd.Performances = new List<PerformanceInfo>();
			foreach (KeyValuePair<uint, uint> keyValuePair in inputData.playersPerformances)
			{
				if (RewardService.PERFORMANCE_LB_STATS.Contains(keyValuePair.Key) && keyValuePair.Value != 0U)
				{
					upd.Performances.Add(new PerformanceInfo
					{
						Stat = keyValuePair.Key,
						Performance = keyValuePair.Value
					});
				}
			}
			upd.ProfilesIds = new List<ulong>();
			foreach (RewardInputData.Team team in inputData.teams.Values)
			{
				foreach (RewardInputData.Team.Player player in team.playerScores)
				{
					upd.ProfilesIds.Add(player.profileId);
				}
			}
			this.m_missionPerformanceService.UpdatePlayersPerformance(upd);
		}

		// Token: 0x06001F89 RID: 8073 RVA: 0x000807C4 File Offset: 0x0007EBC4
		private void OnConfigChanged(ConfigEventArgs e)
		{
			if (string.Equals(e.Name, "AwardExpirationTime", StringComparison.CurrentCultureIgnoreCase))
			{
				this.SetAwardExpirationTime(e.sValue);
			}
		}

		// Token: 0x06001F8A RID: 8074 RVA: 0x000807E8 File Offset: 0x0007EBE8
		private void SetAwardExpirationTime(string expTime)
		{
			string[] array = expTime.Split(new char[]
			{
				':'
			});
			int days = int.Parse(array[0]);
			int hours = int.Parse(array[1]);
			int minutes = int.Parse(array[2]);
			this.AwardExpirationTime = new TimeSpan(days, hours, minutes, 0);
		}

		// Token: 0x06001F8B RID: 8075 RVA: 0x00080834 File Offset: 0x0007EC34
		private void NotifyUsers(List<RewardUpdateData> updateData)
		{
			if (updateData != null && updateData.Any<RewardUpdateData>())
			{
				List<string> list = new List<string>();
				foreach (RewardUpdateData rewardUpdateData in updateData)
				{
					UserInfo.User user = this.m_userRepository.GetUser(rewardUpdateData.rewards.profileId);
					if (user != null)
					{
						list.Add(user.OnlineID);
						if (rewardUpdateData.rewards.gainedSponsorPoints > 0U)
						{
							QueryManager.RequestSt("sponsor_info_updated", user.OnlineID, new object[]
							{
								rewardUpdateData
							});
						}
					}
				}
				if (list.Any<string>())
				{
					QueryManager.RequestSt("broadcast_session_result", "k01." + this.m_onlineClientService.XmppHost, new object[]
					{
						list,
						updateData
					});
				}
			}
		}

		// Token: 0x06001F8C RID: 8076 RVA: 0x00080928 File Offset: 0x0007ED28
		private void FireOnRewardsGivenEvent(string sessionId, List<RewardUpdateData> usersRewards)
		{
			if (this.OnRewardsGiven != null)
			{
				foreach (Delegate @delegate in this.OnRewardsGiven.GetInvocationList())
				{
					try
					{
						((OnRewardsGivenDeleg)@delegate)(sessionId, usersRewards);
					}
					catch (Exception e)
					{
						Log.Error(e);
					}
				}
			}
		}

		// Token: 0x04000F49 RID: 3913
		private static readonly uint[] PERFORMANCE_LB_STATS = new uint[]
		{
			0U,
			1U,
			2U,
			3U,
			4U,
			5U
		};

		// Token: 0x04000F4A RID: 3914
		private readonly IProfileItems m_profileItemsService;

		// Token: 0x04000F4B RID: 3915
		private readonly IItemCache m_itemCacheService;

		// Token: 0x04000F4C RID: 3916
		private readonly ICatalogService m_catalogService;

		// Token: 0x04000F4D RID: 3917
		private readonly IStatsTracker m_statsTracker;

		// Token: 0x04000F4E RID: 3918
		private readonly IRewardCalculator m_rewardCalculator;

		// Token: 0x04000F4F RID: 3919
		private readonly IDALService m_dalService;

		// Token: 0x04000F50 RID: 3920
		private readonly ILogService m_logService;

		// Token: 0x04000F51 RID: 3921
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000F52 RID: 3922
		private readonly IOnlineClient m_onlineClientService;

		// Token: 0x04000F53 RID: 3923
		private readonly INotificationService m_notificationService;

		// Token: 0x04000F54 RID: 3924
		private readonly ITelemetryService m_telemetryService;

		// Token: 0x04000F55 RID: 3925
		private readonly IMissionPerformanceService m_missionPerformanceService;

		// Token: 0x04000F57 RID: 3927
		private readonly List<IRewardProcessor> m_rewarders;
	}
}
