using System;
using System.Threading.Tasks;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.StatsTracking
{
	// Token: 0x020005D7 RID: 1495
	[Service]
	[Singleton]
	internal class TutorialStatsTracker : ServiceModule, ITutorialStatsTracker
	{
		// Token: 0x06001FDC RID: 8156 RVA: 0x00081A7A File Offset: 0x0007FE7A
		public TutorialStatsTracker(ILogService logService, IDALService dalService, ISessionStorage sessionStorage, IRewardMultiplierService rewardMultiplierService, IUserRepository userRepository)
		{
			this.m_logService = logService;
			this.m_dal = dalService;
			this.m_sessionStorage = sessionStorage;
			this.m_rewardMultiplierService = rewardMultiplierService;
			this.m_userRepository = userRepository;
		}

		// Token: 0x1400007F RID: 127
		// (add) Token: 0x06001FDD RID: 8157 RVA: 0x00081AB8 File Offset: 0x0007FEB8
		// (remove) Token: 0x06001FDE RID: 8158 RVA: 0x00081AF0 File Offset: 0x0007FEF0
		public event OnTutorialCompletedDeleg OnTutorialCompleted;

		// Token: 0x06001FDF RID: 8159 RVA: 0x00081B26 File Offset: 0x0007FF26
		public override void Init()
		{
			base.Init();
			this.m_userRepository.UserLoggedOut += this.OnUserLoggedOut;
		}

		// Token: 0x06001FE0 RID: 8160 RVA: 0x00081B45 File Offset: 0x0007FF45
		public override void Stop()
		{
			this.m_userRepository.UserLoggedOut -= this.OnUserLoggedOut;
			this.profilesStats.Dispose();
			base.Stop();
		}

		// Token: 0x06001FE1 RID: 8161 RVA: 0x00081B70 File Offset: 0x0007FF70
		private void OnUserLoggedOut(UserInfo.User user, ELogoutType logout_type)
		{
			TutorialStatsTracker.ProfileStats profileStats;
			if (this.profilesStats.TryGetValue(user.ProfileID, out profileStats))
			{
				this.RemoveProfileStats(user.ProfileID, profileStats.tutorialId, profileStats.tutorialStep, DateTime.UtcNow, TutorialEvent.ABORTED);
			}
		}

		// Token: 0x06001FE2 RID: 8162 RVA: 0x00081BB4 File Offset: 0x0007FFB4
		private void AddNewProfileStats(UserInfo.User user, int tutorialId, string tutorialStep, DateTime currentTime)
		{
			ProfileProgressionInfo.Tutorial idx = (ProfileProgressionInfo.Tutorial)Enum.Parse(typeof(ProfileProgressionInfo.Tutorial), string.Format("tutorial_{0}", tutorialId), true);
			bool flag = user.ProfileProgression.IsTutorialPassed(idx);
			this.profilesStats.Add(user.ProfileID, new TutorialStatsTracker.ProfileStats(currentTime, tutorialStep, tutorialId, !flag));
			this.m_rewardMultiplierService.GetResultMultiplier(user.ProfileID).ContinueWith(delegate(Task<SRewardMultiplier> task)
			{
				string session_id = string.Format("tutorial_{0}", user.ProfileID);
				SRewardMultiplier result = task.Result;
				this.m_sessionStorage.AddData(session_id, ESessionData.RewardMultiplier, result);
			});
		}

		// Token: 0x06001FE3 RID: 8163 RVA: 0x00081C5C File Offset: 0x0008005C
		private TutorialStatsTracker.ProfileStats UpdateProfileStats(ulong profileId, int tutorialId, string tutorialStep, DateTime currentTime)
		{
			TutorialStatsTracker.ProfileStats profileStats;
			if (this.profilesStats.TryGetValue(profileId, out profileStats))
			{
				profileStats.Update(tutorialId, tutorialStep, currentTime);
				return profileStats;
			}
			Log.Warning("TutorialStatsTracker : UpdateProfileStats : Event type IN_PROGRESS is applied to the user that hasn't started playing tutorial yet");
			return null;
		}

		// Token: 0x06001FE4 RID: 8164 RVA: 0x00081C94 File Offset: 0x00080094
		private void RemoveProfileStats(ulong profileId, int tutorialId, string tutorialStep, DateTime currentTime, TutorialEvent completionStatus)
		{
			TutorialStatsTracker.ProfileStats profileStats = this.UpdateProfileStats(profileId, tutorialId, tutorialStep, currentTime);
			if (profileStats != null)
			{
				this.LogEvent(profileId, completionStatus, profileStats);
				this.profilesStats.Remove(profileId);
			}
			string session_id = string.Format("tutorial_{0}", profileId);
			this.m_sessionStorage.RemoveData(session_id, ESessionData.RewardMultiplier);
		}

		// Token: 0x06001FE5 RID: 8165 RVA: 0x00081CE8 File Offset: 0x000800E8
		private void FireOnTutorialCompletedEvent(ulong profileId, int tutorialId, ref ProfileProgressionInfo output, ILogGroup logGroup)
		{
			if (this.OnTutorialCompleted != null)
			{
				foreach (Delegate @delegate in this.OnTutorialCompleted.GetInvocationList())
				{
					try
					{
						((OnTutorialCompletedDeleg)@delegate)(profileId, tutorialId, ref output, logGroup);
					}
					catch (Exception e)
					{
						Log.Error(e);
					}
				}
			}
		}

		// Token: 0x06001FE6 RID: 8166 RVA: 0x00081D58 File Offset: 0x00080158
		public void TrackEvent(TutorialEvent eventType, UserInfo.User user, int tutorialId, string tutorialStep, XmlElement response)
		{
			DateTime utcNow = DateTime.UtcNow;
			switch (eventType)
			{
			case TutorialEvent.STARTED:
				this.AddNewProfileStats(user, tutorialId, tutorialStep, utcNow);
				break;
			case TutorialEvent.IN_PROGRESS:
				this.UpdateProfileStats(user.ProfileID, tutorialId, tutorialStep, utcNow);
				break;
			case TutorialEvent.COMPLETED:
				using (ILogGroup logGroup = this.m_logService.CreateGroup())
				{
					ProfileProgressionInfo profileProgression = user.ProfileProgression;
					this.FireOnTutorialCompletedEvent(user.ProfileID, tutorialId, ref profileProgression, logGroup);
					XmlElement newChild = profileProgression.ToXml(response.OwnerDocument, true);
					response.AppendChild(newChild);
				}
				this.RemoveProfileStats(user.ProfileID, tutorialId, tutorialStep, utcNow, eventType);
				break;
			case TutorialEvent.ABORTED:
				this.RemoveProfileStats(user.ProfileID, tutorialId, tutorialStep, utcNow, eventType);
				break;
			default:
				throw new ApplicationException("TutorialStatsTracker : TrackEvent : Invalid event type");
			}
		}

		// Token: 0x06001FE7 RID: 8167 RVA: 0x00081E40 File Offset: 0x00080240
		private void LogEvent(ulong profileId, TutorialEvent eventType, TutorialStatsTracker.ProfileStats trackedProfile)
		{
			SProfileInfo profileInfo = this.m_dal.ProfileSystem.GetProfileInfo(profileId);
			this.m_logService.Event.TutorialStatsLog(profileInfo.UserID, profileId, eventType, trackedProfile.tutorialId, trackedProfile.tutorialStep, trackedProfile.totalPlaytime, trackedProfile.deltaPlaytime, trackedProfile.playedFirstTime);
		}

		// Token: 0x04000F96 RID: 3990
		private const int ENTRY_EXPIRATION_TIME = 1800;

		// Token: 0x04000F97 RID: 3991
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000F98 RID: 3992
		private readonly ILogService m_logService;

		// Token: 0x04000F99 RID: 3993
		private readonly IDALService m_dal;

		// Token: 0x04000F9A RID: 3994
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x04000F9B RID: 3995
		private readonly IRewardMultiplierService m_rewardMultiplierService;

		// Token: 0x04000F9C RID: 3996
		private CacheDictionary<ulong, TutorialStatsTracker.ProfileStats> profilesStats = new CacheDictionary<ulong, TutorialStatsTracker.ProfileStats>(1800);

		// Token: 0x020005D8 RID: 1496
		private class ProfileStats
		{
			// Token: 0x06001FE8 RID: 8168 RVA: 0x00081E96 File Offset: 0x00080296
			public ProfileStats(DateTime startTime, string step, int tutorialMissionId, bool firstAttempt)
			{
				this.lastUpdateTime = startTime;
				this.tutorialStep = step;
				this.tutorialId = tutorialMissionId;
				this.playedFirstTime = firstAttempt;
				this.deltaPlaytime = TimeSpan.Zero;
				this.totalPlaytime = TimeSpan.Zero;
			}

			// Token: 0x06001FE9 RID: 8169 RVA: 0x00081ED4 File Offset: 0x000802D4
			public void Update(int tutorialMissionId, string step, DateTime currentTime)
			{
				if (tutorialMissionId != this.tutorialId)
				{
					Log.Warning("TutorialStatsTracker : ProfileStats : Incorrect tutorial id for Update");
					return;
				}
				if (!string.IsNullOrEmpty(step))
				{
					this.tutorialStep = step;
				}
				this.deltaPlaytime = currentTime - this.lastUpdateTime;
				this.totalPlaytime += this.deltaPlaytime;
				this.lastUpdateTime = currentTime;
			}

			// Token: 0x04000F9E RID: 3998
			public DateTime lastUpdateTime;

			// Token: 0x04000F9F RID: 3999
			public string tutorialStep;

			// Token: 0x04000FA0 RID: 4000
			public TimeSpan totalPlaytime;

			// Token: 0x04000FA1 RID: 4001
			public TimeSpan deltaPlaytime;

			// Token: 0x04000FA2 RID: 4002
			public int tutorialId;

			// Token: 0x04000FA3 RID: 4003
			public bool playedFirstTime;
		}
	}
}
