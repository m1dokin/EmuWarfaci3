using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HK2Net;
using HK2Net.Kernel;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000561 RID: 1377
	[Service]
	[Singleton]
	internal class ProfileProgressionService : ServiceModule, IProfileProgressionService, IProfileProgressionDebug, IRewardProcessor
	{
		// Token: 0x06001DC4 RID: 7620 RVA: 0x00078508 File Offset: 0x00076908
		public ProfileProgressionService(IDALService dal, ILogService log, IContainer container, INotificationService notifications, IUserRepository userRepository, IDBUpdateService dbUpdater, IRankSystem rankSystem)
		{
			this.m_dal = dal;
			this.m_notifications = notifications;
			this.m_userRepository = userRepository;
			this.m_log = log;
			this.m_dbUpdater = dbUpdater;
			this.m_rankSystem = rankSystem;
			this.m_container = container;
		}

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x06001DC5 RID: 7621 RVA: 0x0007855B File Offset: 0x0007695B
		public bool IsEnabled
		{
			get
			{
				return this.m_enabled;
			}
		}

		// Token: 0x06001DC6 RID: 7622 RVA: 0x00078563 File Offset: 0x00076963
		public override void Init()
		{
		}

		// Token: 0x06001DC7 RID: 7623 RVA: 0x00078568 File Offset: 0x00076968
		public override void Start()
		{
			base.Start();
			this.InitMissionTypesAndLegacyDiff();
			this.m_rules = this.LoadRules();
			this.m_rankSystem.OnProfileRankChanged += this.OnRankChanged;
			Resources.ProfileProgressionConfig.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x06001DC8 RID: 7624 RVA: 0x000785BC File Offset: 0x000769BC
		public override void Stop()
		{
			Resources.ProfileProgressionConfig.OnConfigChanged -= this.OnConfigChanged;
			this.m_rankSystem.OnProfileRankChanged -= this.OnRankChanged;
			this.UnloadRules(this.m_rules);
			base.Stop();
		}

		// Token: 0x06001DC9 RID: 7625 RVA: 0x00078608 File Offset: 0x00076A08
		private List<IProfileProgressionRule> LoadRules()
		{
			List<IProfileProgressionRule> list = new List<IProfileProgressionRule>();
			ConfigSection profileProgressionConfig = Resources.ProfileProgressionConfig;
			profileProgressionConfig.Get("enabled", out this.m_enabled);
			Dictionary<string, Type> dictionary = ReflectionUtils.GetTypesByAttribute<ProfileProgressionRuleAttribute>(Assembly.GetExecutingAssembly()).ToDictionary((KeyValuePair<ProfileProgressionRuleAttribute, Type> rule) => rule.Key.Name, (KeyValuePair<ProfileProgressionRuleAttribute, Type> rule) => rule.Value);
			foreach (KeyValuePair<string, List<ConfigSection>> keyValuePair in Resources.ProfileProgressionConfig.GetAllSections())
			{
				foreach (ConfigSection section in keyValuePair.Value)
				{
					IProfileProgressionRule profileProgressionRule = (IProfileProgressionRule)this.m_container.Create(dictionary[keyValuePair.Key]);
					profileProgressionRule.Init(section);
					list.Add(profileProgressionRule);
				}
			}
			return list;
		}

		// Token: 0x06001DCA RID: 7626 RVA: 0x00078744 File Offset: 0x00076B44
		private void UnloadRules(List<IProfileProgressionRule> rules)
		{
			foreach (IProfileProgressionRule profileProgressionRule in rules)
			{
				profileProgressionRule.Dispose();
			}
			rules.Clear();
		}

		// Token: 0x06001DCB RID: 7627 RVA: 0x000787A0 File Offset: 0x00076BA0
		private void InitMissionTypesAndLegacyDiff()
		{
			this.m_missionTypes = new List<ProfileProgressionService.MissionUnlockInfo>();
			Config profileProgressionConfig = Resources.ProfileProgressionConfig;
			MissionType missionType;
			foreach (KeyValuePair<string, List<ConfigSection>> keyValuePair in profileProgressionConfig.GetAllSections())
			{
				if (keyValuePair.Key.Equals("mission_unlock"))
				{
					foreach (ConfigSection configSection in keyValuePair.Value)
					{
						string text = configSection.Get("type");
						ProfileProgressionInfo.MissionType missionToPass = ProfileProgressionInfo.MissionType.None;
						if (!string.IsNullOrEmpty(text) && !Utils.TryParse<ProfileProgressionInfo.MissionType>(text, out missionToPass))
						{
							throw new ArgumentException(string.Format("Unsupported mission type {0} detected in profile_progression_config in type attribute", text));
						}
						string text2 = configSection.Get("unlock_type");
						ProfileProgressionInfo.MissionType unlockType = ProfileProgressionInfo.MissionType.None;
						if (!string.IsNullOrEmpty(text2) && !Utils.TryParse<ProfileProgressionInfo.MissionType>(text2, out unlockType))
						{
							throw new ArgumentException(string.Format("Unsupported mission type {0} detected in profile_progression_config in unlock_type attribute", text));
						}
						string text3 = configSection.Get("max_value");
						missionType = new MissionType(text2);
						this.m_missionTypes.Add(new ProfileProgressionService.MissionUnlockInfo((!string.IsNullOrEmpty(text3)) ? int.Parse(text3) : 0, missionToPass, unlockType, configSection.Get("legacy_diff"), ProfileProgressionService.GetMissionUnlockBranch(missionType)));
					}
				}
			}
			this.m_legacyDiffsConversion = (from cs in this.m_missionTypes
			where !string.IsNullOrEmpty(cs.LegacyDiff) && cs.UnlockBranch == MissionUnlockBranch.Default
			select cs).ToDictionary((ProfileProgressionService.MissionUnlockInfo cs) => (ProfileProgressionService.LegacyMissionMask)Enum.Parse(typeof(ProfileProgressionService.LegacyMissionMask), cs.LegacyDiff, true), delegate(ProfileProgressionService.MissionUnlockInfo cs)
			{
				ProfileProgressionInfo.MissionType missionType = cs.MissionToPass;
				return this.m_missionTypes.First((ProfileProgressionService.MissionUnlockInfo x) => x.UnlockType.HasFlag(missionType)).MaxValue;
			});
		}

		// Token: 0x06001DCC RID: 7628 RVA: 0x000789A4 File Offset: 0x00076DA4
		private void OnConfigChanged(ConfigEventArgs e)
		{
			ConfigSection profileProgressionConfig = Resources.ProfileProgressionConfig;
			profileProgressionConfig.Get("enabled", out this.m_enabled);
		}

		// Token: 0x06001DCD RID: 7629 RVA: 0x000789C8 File Offset: 0x00076DC8
		public ProfileProgressionInfo InitProgression(ulong profileId)
		{
			ProfileProgressionInfo profileProgressionInfo = new ProfileProgressionInfo(profileId, this.m_dal);
			this.m_dal.ProfileProgressionSystem.SetProfileProgression(profileId, profileProgressionInfo);
			return this.m_rules.Aggregate(profileProgressionInfo, (ProfileProgressionInfo current, IProfileProgressionRule rule) => rule.TrigerRule(profileId, current, this.m_log.Event));
		}

		// Token: 0x06001DCE RID: 7630 RVA: 0x00078A30 File Offset: 0x00076E30
		public ProfileProgressionInfo GetProgression(ulong profileId)
		{
			ProfileProgressionInfo profileProgressionInfo = new ProfileProgressionInfo(this.m_dal.ProfileProgressionSystem.GetProfileProgression(profileId), this.m_dal, this);
			if (profileProgressionInfo.ProfileId == 0UL)
			{
				profileProgressionInfo = this.ConvertProgression(profileId);
			}
			return this.m_rules.Aggregate(profileProgressionInfo, (ProfileProgressionInfo current, IProfileProgressionRule rule) => rule.TrigerRule(profileId, current, this.m_log.Event));
		}

		// Token: 0x06001DCF RID: 7631 RVA: 0x00078AA8 File Offset: 0x00076EA8
		private ProfileProgressionInfo ConvertProgression(ulong profileId)
		{
			ProfileProgressionInfo profileProgressionInfo = new ProfileProgressionInfo(profileId, this.m_dal);
			SProfileInfo profileInfo = this.m_dal.ProfileSystem.GetProfileInfo(profileId);
			using (ILogGroup logGroup = this.m_log.CreateGroup())
			{
				profileProgressionInfo.UnlockTutorial(ProfileProgressionInfo.Tutorial.Tutorial_1, logGroup);
				profileProgressionInfo.UnlockTutorial(ProfileProgressionInfo.Tutorial.Tutorial_2, logGroup);
				profileProgressionInfo.UnlockTutorial(ProfileProgressionInfo.Tutorial.Tutorial_3, logGroup);
				profileProgressionInfo.UnlockClass(ProfileProgressionInfo.PlayerClass.Rifleman, logGroup);
				profileProgressionInfo.UnlockClass(ProfileProgressionInfo.PlayerClass.Sniper, logGroup);
				profileProgressionInfo.UnlockClass(ProfileProgressionInfo.PlayerClass.Medic, logGroup);
				profileProgressionInfo.UnlockClass(ProfileProgressionInfo.PlayerClass.Engineer, logGroup);
				ProfileProgressionInfo.MissionType missionPassed = (ProfileProgressionInfo.MissionType)profileInfo.MissionPassed;
				if (missionPassed.HasAnyFlag(ProfileProgressionInfo.MissionType.TrainingMission))
				{
					profileProgressionInfo.PassTutorial(ProfileProgressionInfo.Tutorial.Tutorial_1, logGroup);
				}
				profileProgressionInfo.MissionPassCounter = this.ConvertFromDiff(missionPassed);
				profileProgressionInfo = new ProfileProgressionInfo(this.m_dal.ProfileProgressionSystem.SetProfileProgression(profileId, profileProgressionInfo), this.m_dal, this);
				foreach (ProfileProgressionInfo.MissionType missionType in profileProgressionInfo.GetUnlockedMissions())
				{
					this.MissionUnlockedLog(profileId, missionType, logGroup);
				}
			}
			return profileProgressionInfo;
		}

		// Token: 0x06001DD0 RID: 7632 RVA: 0x00078BE0 File Offset: 0x00076FE0
		public ProfileProgressionInfo IncrementMissionPassCounter(ulong profileId, int value, int maxValue, MissionUnlockBranch branch)
		{
			SProfileProgression progression = SProfileProgression.Empty;
			if (branch == MissionUnlockBranch.Default)
			{
				progression = this.m_dal.ProfileProgressionSystem.IncrementMissionPassCounter(profileId, value, maxValue);
			}
			else if (branch == MissionUnlockBranch.Zombie)
			{
				progression = this.m_dal.ProfileProgressionSystem.IncrementZombieMissionPassCounter(profileId, value, maxValue);
			}
			else if (branch == MissionUnlockBranch.Campaign)
			{
				progression = this.m_dal.ProfileProgressionSystem.IncrementCampaignPassCounter(profileId, value, maxValue);
			}
			else if (branch == MissionUnlockBranch.VolcanoCampaign)
			{
				progression = this.m_dal.ProfileProgressionSystem.IncrementVolcanoCampaignPassCounter(profileId, value, maxValue);
			}
			else if (branch == MissionUnlockBranch.AnubisCampaign)
			{
				progression = this.m_dal.ProfileProgressionSystem.IncrementAnubisCampaignPassCounter(profileId, value, maxValue);
			}
			else if (branch == MissionUnlockBranch.ZombieTowerCampaign)
			{
				progression = this.m_dal.ProfileProgressionSystem.IncrementZombieTowerCampaignPassCounter(profileId, value, maxValue);
			}
			else if (branch == MissionUnlockBranch.IceBreakerCampaign)
			{
				progression = this.m_dal.ProfileProgressionSystem.IncrementIceBreakerCampaignPassCounter(profileId, value, maxValue);
			}
			ProfileProgressionInfo profileProgressionInfo = new ProfileProgressionInfo(progression, this.m_dal, this);
			this.UpdateUser(profileId, profileProgressionInfo);
			return profileProgressionInfo;
		}

		// Token: 0x06001DD1 RID: 7633 RVA: 0x00078CEC File Offset: 0x000770EC
		public ProfileProgressionInfo UnlockMission(ProfileProgressionInfo progression, ProfileProgressionInfo.MissionType unlockedMissionType, bool silent, ILogGroup logGroup)
		{
			ProfileProgressionInfo result = new ProfileProgressionInfo(progression.ProfileId, this.m_dal);
			if (!progression.IsMissionTypeUnlocked(unlockedMissionType))
			{
				ProfileProgressionInfo profileProgressionInfo = new ProfileProgressionInfo(this.m_dal.ProfileProgressionSystem.UnlockMission(progression.ProfileId, unlockedMissionType), this.m_dal, this);
				result = (progression ^ profileProgressionInfo);
				this.AddMissionUnlockNotification(progression.ProfileId, unlockedMissionType.ToString().ToLower(), silent);
				this.MissionUnlockedLog(progression.ProfileId, unlockedMissionType, logGroup);
				this.UpdateUser(progression.ProfileId, profileProgressionInfo);
			}
			return result;
		}

		// Token: 0x06001DD2 RID: 7634 RVA: 0x00078D7F File Offset: 0x0007717F
		public ProfileProgressionInfo UnlockTutorial(ProfileProgressionInfo progression, int tutorialId, bool silent, ILogGroup logGroup)
		{
			return this.UnlockTutorial(progression, Utils.ConvertToEnumFlag<ProfileProgressionInfo.Tutorial>(tutorialId), silent, logGroup);
		}

		// Token: 0x06001DD3 RID: 7635 RVA: 0x00078D94 File Offset: 0x00077194
		public ProfileProgressionInfo UnlockTutorial(ProfileProgressionInfo progression, ProfileProgressionInfo.Tutorial tutorialId, bool silent, ILogGroup logGroup)
		{
			ProfileProgressionInfo result = new ProfileProgressionInfo(progression.ProfileId, this.m_dal);
			UserInfo.User user = this.m_userRepository.GetUser(progression.ProfileId);
			if (!progression.IsTutorialUnlocked(tutorialId))
			{
				ProfileProgressionInfo profileProgressionInfo = new ProfileProgressionInfo(this.m_dal.ProfileProgressionSystem.UnlockTutorial(progression.ProfileId, tutorialId), this.m_dal, this);
				result = (progression ^ profileProgressionInfo);
				if (!silent)
				{
					this.AddNotification(progression.ProfileId, string.Format("@{0}_unlocked", tutorialId.ToString()));
				}
				this.TutorialUnlockedLog(progression.ProfileId, tutorialId, logGroup);
				this.UpdateUser(progression.ProfileId, profileProgressionInfo);
			}
			return result;
		}

		// Token: 0x06001DD4 RID: 7636 RVA: 0x00078E43 File Offset: 0x00077243
		public ProfileProgressionInfo PassTutorial(ulong profileId, int tutorialId, bool silent, ILogGroup logGroup)
		{
			return this.PassTutorial(profileId, Utils.ConvertToEnumFlag<ProfileProgressionInfo.Tutorial>(tutorialId), silent, logGroup);
		}

		// Token: 0x06001DD5 RID: 7637 RVA: 0x00078E58 File Offset: 0x00077258
		public ProfileProgressionInfo PassTutorial(ulong profileId, ProfileProgressionInfo.Tutorial tutorialId, bool silent, ILogGroup logGroup)
		{
			ProfileProgressionInfo result = new ProfileProgressionInfo(profileId, this.m_dal);
			UserInfo.User user = this.m_userRepository.GetUser(profileId);
			ProfileProgressionInfo profileProgressionInfo = (user == null) ? this.GetProgression(profileId) : user.ProfileProgression;
			if (!profileProgressionInfo.IsTutorialPassed(tutorialId))
			{
				ProfileProgressionInfo profileProgressionInfo2 = new ProfileProgressionInfo(this.m_dal.ProfileProgressionSystem.PassTutorial(profileId, tutorialId), this.m_dal, this);
				result = (profileProgressionInfo ^ profileProgressionInfo2);
				if (!silent)
				{
					this.AddNotification(profileId, string.Format("@{0}_passed", tutorialId.ToString()));
				}
				this.TutorialPassedLog(profileId, tutorialId, logGroup);
				this.UpdateUser(profileId, profileProgressionInfo2);
			}
			return result;
		}

		// Token: 0x06001DD6 RID: 7638 RVA: 0x00078F02 File Offset: 0x00077302
		public ProfileProgressionInfo UnlockClass(ProfileProgressionInfo progression, int classId, bool silent, ILogGroup logGroup)
		{
			return this.UnlockClass(progression, Utils.ConvertToEnumFlag<ProfileProgressionInfo.PlayerClass>(classId), silent, logGroup);
		}

		// Token: 0x06001DD7 RID: 7639 RVA: 0x00078F14 File Offset: 0x00077314
		public ProfileProgressionInfo UnlockClass(ProfileProgressionInfo progression, ProfileProgressionInfo.PlayerClass classId, bool silent, ILogGroup logGroup)
		{
			ProfileProgressionInfo result = new ProfileProgressionInfo(progression.ProfileId, this.m_dal);
			if (!progression.IsClassUnlocked(classId))
			{
				ProfileProgressionInfo profileProgressionInfo = new ProfileProgressionInfo(this.m_dal.ProfileProgressionSystem.UnlockClass(progression.ProfileId, classId), this.m_dal, this);
				result = (progression ^ profileProgressionInfo);
				if (!silent)
				{
					this.AddNotification(progression.ProfileId, string.Format("@{0}_unlocked", classId.ToString()));
				}
				this.ClassUnlockedLog(progression.ProfileId, classId, logGroup);
				this.UpdateUser(progression.ProfileId, profileProgressionInfo);
			}
			return result;
		}

		// Token: 0x06001DD8 RID: 7640 RVA: 0x00078FB4 File Offset: 0x000773B4
		public RewardOutputData ProcessRewardData(ulong userId, RewardProcessorState state, MissionContext missionContext, RewardOutputData aggRewardData, ILogGroup logGroup)
		{
			ProfileProgressionService.<ProcessRewardData>c__AnonStorey3 <ProcessRewardData>c__AnonStorey = new ProfileProgressionService.<ProcessRewardData>c__AnonStorey3();
			<ProcessRewardData>c__AnonStorey.aggRewardData = aggRewardData;
			<ProcessRewardData>c__AnonStorey.$this = this;
			if (state != RewardProcessorState.PostProcess)
			{
				return <ProcessRewardData>c__AnonStorey.aggRewardData;
			}
			foreach (IProfileProgressionRule profileProgressionRule in this.m_rules)
			{
				ProfileProgressionService.<ProcessRewardData>c__AnonStorey3 <ProcessRewardData>c__AnonStorey2 = <ProcessRewardData>c__AnonStorey;
				<ProcessRewardData>c__AnonStorey2.aggRewardData.progression = (<ProcessRewardData>c__AnonStorey2.aggRewardData.progression | profileProgressionRule.ProcessRewardData(missionContext, <ProcessRewardData>c__AnonStorey.aggRewardData, logGroup));
			}
			<ProcessRewardData>c__AnonStorey.aggRewardData.progression = this.m_rules.Aggregate(<ProcessRewardData>c__AnonStorey.aggRewardData.progression, (ProfileProgressionInfo current, IProfileProgressionRule rule) => rule.TrigerRule(<ProcessRewardData>c__AnonStorey.aggRewardData.profileId, current, <ProcessRewardData>c__AnonStorey.$this.m_log.Event));
			return <ProcessRewardData>c__AnonStorey.aggRewardData;
		}

		// Token: 0x06001DD9 RID: 7641 RVA: 0x00079084 File Offset: 0x00077484
		private void AddNotification(ulong profileId, string data)
		{
			this.m_notifications.AddNotification<string>(profileId, ENotificationType.CongratulationMessage, data, TimeSpan.FromDays(1.0), EDeliveryType.SendNowOrLater, EConfirmationType.Confirmation);
		}

		// Token: 0x06001DDA RID: 7642 RVA: 0x000790AC File Offset: 0x000774AC
		private void AddMissionUnlockNotification(ulong profileId, string data, bool silent)
		{
			MissionUnlockNotification data2 = new MissionUnlockNotification(data, silent);
			this.m_notifications.AddNotification<MissionUnlockNotification>(profileId, ENotificationType.MissionUnlockMessage, data2, TimeSpan.FromDays(1.0), EDeliveryType.SendNowOrLater, EConfirmationType.Confirmation);
		}

		// Token: 0x06001DDB RID: 7643 RVA: 0x000790E4 File Offset: 0x000774E4
		private void TutorialPassedLog(ulong profileId, ProfileProgressionInfo.Tutorial tutorialId, ILogGroup logGroup)
		{
			SProfileInfo profileInfo = this.m_dal.ProfileSystem.GetProfileInfo(profileId);
			logGroup.TutorialPassedLog(profileInfo.UserID, profileId, tutorialId, profileInfo.RankInfo.RankId);
		}

		// Token: 0x06001DDC RID: 7644 RVA: 0x00079120 File Offset: 0x00077520
		private void TutorialUnlockedLog(ulong profileId, ProfileProgressionInfo.Tutorial tutorialId, ILogGroup logGroup)
		{
			SProfileInfo profileInfo = this.m_dal.ProfileSystem.GetProfileInfo(profileId);
			logGroup.TutorialUnlockedLog(profileInfo.UserID, profileId, tutorialId, profileInfo.RankInfo.RankId);
		}

		// Token: 0x06001DDD RID: 7645 RVA: 0x0007915C File Offset: 0x0007755C
		private void ClassUnlockedLog(ulong profileId, ProfileProgressionInfo.PlayerClass classId, ILogGroup logGroup)
		{
			SProfileInfo profileInfo = this.m_dal.ProfileSystem.GetProfileInfo(profileId);
			logGroup.ClassUnlockedLog(profileInfo.UserID, profileId, classId, profileInfo.RankInfo.RankId);
		}

		// Token: 0x06001DDE RID: 7646 RVA: 0x00079198 File Offset: 0x00077598
		private void MissionUnlockedLog(ulong profileId, ProfileProgressionInfo.MissionType missionType, ILogGroup logGroup)
		{
			SProfileInfo profileInfo = this.m_dal.ProfileSystem.GetProfileInfo(profileId);
			logGroup.MissionUnlockedLog(profileInfo.UserID, profileId, missionType.ToString().ToLower(), profileInfo.RankInfo.RankId);
		}

		// Token: 0x06001DDF RID: 7647 RVA: 0x000791E4 File Offset: 0x000775E4
		public void DumpRules()
		{
			for (int i = 0; i < this.m_rules.Count; i++)
			{
				Log.Info<int, string>("INDEX {0} >> {1}", i, this.m_rules[i].ToString());
			}
		}

		// Token: 0x06001DE0 RID: 7648 RVA: 0x0007922C File Offset: 0x0007762C
		public void UpdateTutorialUnlockPlaytime(int index, TimeSpan time)
		{
			TutorialUnlockRule tutorialUnlockRule = this.m_rules[index] as TutorialUnlockRule;
			if (tutorialUnlockRule != null)
			{
				tutorialUnlockRule.PlayTime = time;
			}
		}

		// Token: 0x06001DE1 RID: 7649 RVA: 0x00079258 File Offset: 0x00077658
		private int ConvertFromDiff(ProfileProgressionInfo.MissionType missionPassed)
		{
			int result = 0;
			if (!missionPassed.HasAnyFlag(ProfileProgressionService.LegacyMissionMask.Hard) || !this.m_legacyDiffsConversion.TryGetValue(ProfileProgressionService.LegacyMissionMask.Hard, out result))
			{
				bool flag = missionPassed.HasAnyFlag(ProfileProgressionService.LegacyMissionMask.Normal) && this.m_legacyDiffsConversion.TryGetValue(ProfileProgressionService.LegacyMissionMask.Normal, out result);
			}
			return result;
		}

		// Token: 0x06001DE2 RID: 7650 RVA: 0x000792C8 File Offset: 0x000776C8
		private void UpdateUser(ulong profileId, ProfileProgressionInfo profileProgression)
		{
			UserInfo.User user = this.m_userRepository.GetUser(profileId);
			if (user != null)
			{
				UserInfo.User userInfo = user.CloneWithProgression(profileProgression);
				this.m_userRepository.SetUserInfo(userInfo);
			}
		}

		// Token: 0x06001DE3 RID: 7651 RVA: 0x000792FC File Offset: 0x000776FC
		public static MissionUnlockBranch GetMissionUnlockBranch(ProfileProgressionInfo.MissionType missionType)
		{
			MissionType missionType2 = new MissionType(missionType);
			return ProfileProgressionService.GetMissionUnlockBranch(missionType2);
		}

		// Token: 0x06001DE4 RID: 7652 RVA: 0x00079318 File Offset: 0x00077718
		public static MissionUnlockBranch GetMissionUnlockBranch(MissionType missionType)
		{
			if (missionType.IsZombie())
			{
				return MissionUnlockBranch.Zombie;
			}
			if (missionType.IsCampaign())
			{
				return MissionUnlockBranch.Campaign;
			}
			if (missionType.IsVolcanoCampaign())
			{
				return MissionUnlockBranch.VolcanoCampaign;
			}
			if (missionType.IsAnubisCampaign())
			{
				return MissionUnlockBranch.AnubisCampaign;
			}
			if (missionType.IsZombieTowerCampaign())
			{
				return MissionUnlockBranch.ZombieTowerCampaign;
			}
			if (missionType.IsIceBreakerCampaign())
			{
				return MissionUnlockBranch.IceBreakerCampaign;
			}
			return MissionUnlockBranch.Default;
		}

		// Token: 0x06001DE5 RID: 7653 RVA: 0x00079374 File Offset: 0x00077774
		public static int GetMissionPassCounter(ProfileProgressionInfo info, MissionUnlockBranch branch)
		{
			switch (branch)
			{
			case MissionUnlockBranch.Zombie:
				return info.ZombieMissionPassCounter;
			case MissionUnlockBranch.Campaign:
				return info.CampaignPassCounter;
			case MissionUnlockBranch.VolcanoCampaign:
				return info.VolcanoCampaignPassCounter;
			case MissionUnlockBranch.AnubisCampaign:
				return info.AnubisCampaignPassCounter;
			case MissionUnlockBranch.ZombieTowerCampaign:
				return info.ZombieTowerCampaignPassCounter;
			case MissionUnlockBranch.IceBreakerCampaign:
				return info.IceBreakerCampaignPassCounter;
			default:
				return info.MissionPassCounter;
			}
		}

		// Token: 0x06001DE6 RID: 7654 RVA: 0x000793D8 File Offset: 0x000777D8
		private void OnRankChanged(SProfileInfo profile, SRankInfo newRank, SRankInfo oldRank, ILogGroup logGroup)
		{
			ProfileProgressionInfo progression = this.GetProgression(profile.Id);
		}

		// Token: 0x04000E62 RID: 3682
		private readonly IDALService m_dal;

		// Token: 0x04000E63 RID: 3683
		private readonly INotificationService m_notifications;

		// Token: 0x04000E64 RID: 3684
		private readonly ILogService m_log;

		// Token: 0x04000E65 RID: 3685
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000E66 RID: 3686
		private readonly IDBUpdateService m_dbUpdater;

		// Token: 0x04000E67 RID: 3687
		private readonly IContainer m_container;

		// Token: 0x04000E68 RID: 3688
		private readonly IRankSystem m_rankSystem;

		// Token: 0x04000E69 RID: 3689
		private List<IProfileProgressionRule> m_rules = new List<IProfileProgressionRule>();

		// Token: 0x04000E6A RID: 3690
		private Dictionary<ProfileProgressionService.LegacyMissionMask, int> m_legacyDiffsConversion;

		// Token: 0x04000E6B RID: 3691
		private List<ProfileProgressionService.MissionUnlockInfo> m_missionTypes;

		// Token: 0x04000E6C RID: 3692
		private bool m_enabled;

		// Token: 0x02000562 RID: 1378
		private enum LegacyMissionMask
		{
			// Token: 0x04000E72 RID: 3698
			Easy,
			// Token: 0x04000E73 RID: 3699
			Normal = 64,
			// Token: 0x04000E74 RID: 3700
			Hard = 128
		}

		// Token: 0x02000563 RID: 1379
		private struct MissionUnlockInfo
		{
			// Token: 0x06001DEC RID: 7660 RVA: 0x00079487 File Offset: 0x00077887
			public MissionUnlockInfo(int maxValue, ProfileProgressionInfo.MissionType missionToPass, ProfileProgressionInfo.MissionType unlockType, string legacyDiff, MissionUnlockBranch unlockBranch)
			{
				this.MaxValue = maxValue;
				this.UnlockType = unlockType;
				this.LegacyDiff = legacyDiff;
				this.UnlockBranch = unlockBranch;
				this.MissionToPass = missionToPass;
			}

			// Token: 0x04000E75 RID: 3701
			public readonly int MaxValue;

			// Token: 0x04000E76 RID: 3702
			public readonly string LegacyDiff;

			// Token: 0x04000E77 RID: 3703
			public readonly ProfileProgressionInfo.MissionType UnlockType;

			// Token: 0x04000E78 RID: 3704
			public readonly MissionUnlockBranch UnlockBranch;

			// Token: 0x04000E79 RID: 3705
			public readonly ProfileProgressionInfo.MissionType MissionToPass;
		}
	}
}
