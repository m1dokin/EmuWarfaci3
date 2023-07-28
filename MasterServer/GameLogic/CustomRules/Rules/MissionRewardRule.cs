using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SpecialProfileRewards;
using MasterServer.Users;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002D1 RID: 721
	[CustomRule("mission_reward")]
	internal class MissionRewardRule : CustomRule
	{
		// Token: 0x06000F5F RID: 3935 RVA: 0x0003DD48 File Offset: 0x0003C148
		public MissionRewardRule(XmlElement config, ILogService logService, IUserRepository userRepository, ISpecialProfileRewardService specialRewards, IRewardService rewardService, MasterServer.GameLogic.MissionSystem.IMissionSystem missionSystem, INotificationService notificationService, ITagService tagService) : base(config, userRepository, logService, notificationService, tagService, MissionRewardRule.RULE_CFG_ATTRS)
		{
			this.m_specialRewards = specialRewards;
			this.m_rewardService = rewardService;
			this.m_missionSystem = missionSystem;
			this.m_tagService = tagService;
			this.m_missionType = config.GetAttribute("mission_type");
			this.m_rewardSet = config.GetAttribute("reward_set");
		}

		// Token: 0x06000F60 RID: 3936 RVA: 0x0003DDA9 File Offset: 0x0003C1A9
		public override bool IsActive()
		{
			return base.Enabled;
		}

		// Token: 0x06000F61 RID: 3937 RVA: 0x0003DDB1 File Offset: 0x0003C1B1
		public override void Activate()
		{
			this.m_rewardService.OnRewardsGiven += this.OnRewardsGiven;
		}

		// Token: 0x06000F62 RID: 3938 RVA: 0x0003DDCA File Offset: 0x0003C1CA
		public override void Dispose()
		{
			this.m_rewardService.OnRewardsGiven -= this.OnRewardsGiven;
		}

		// Token: 0x06000F63 RID: 3939 RVA: 0x0003DDE4 File Offset: 0x0003C1E4
		protected override ulong GetRuleID(XmlElement config)
		{
			ulong num = (ulong)((long)"mission_reward".GetHashCode());
			string[] array = new string[]
			{
				"mission_type",
				"reward_set"
			};
			for (int num2 = 0; num2 != array.Length; num2++)
			{
				num ^= (ulong)((ulong)((long)config.GetAttribute(array[num2]).GetHashCode()) << 32 * (num2 % 2));
			}
			return num;
		}

		// Token: 0x06000F64 RID: 3940 RVA: 0x0003DE48 File Offset: 0x0003C248
		private void OnRewardsGiven(string sessionId, List<RewardUpdateData> usersRewards)
		{
			Log.Verbose(Log.Group.CustomRules, "Checking session '{0}' for eligibility by rule '{1}'", new object[]
			{
				sessionId,
				this
			});
			string key = (usersRewards.FirstOrDefault<RewardUpdateData>() ?? new RewardUpdateData()).mission.ToString();
			MissionContext mission = this.m_missionSystem.GetMission(key);
			if (mission == null || this.m_missionType != mission.missionType.Name)
			{
				return;
			}
			foreach (RewardUpdateData rewardUpdateData in usersRewards)
			{
				if (rewardUpdateData.status == MissionStatus.Finished)
				{
					ulong profileId = rewardUpdateData.rewards.profileId;
					Log.Verbose(Log.Group.CustomRules, "Rule '{0}' is triggered on Player '{1}' for session '{2}'", new object[]
					{
						base.RuleID,
						profileId,
						sessionId
					});
					using (ILogGroup logGroup = this.LogService.CreateGroup())
					{
						base.ReportCustomRuleTriggered(rewardUpdateData.rewards.userId, profileId, logGroup);
						List<SNotification> notifications = this.m_specialRewards.ProcessEvent(this.m_rewardSet, profileId, logGroup);
						base.SendNotifications(profileId, notifications);
					}
				}
			}
		}

		// Token: 0x06000F65 RID: 3941 RVA: 0x0003DFBC File Offset: 0x0003C3BC
		public override string ToString()
		{
			return string.Format("{0} id={1} mission_type='{2}' message='{3}' reward_set='{4}'", new object[]
			{
				"mission_reward",
				base.RuleID,
				this.m_missionType,
				base.Message,
				this.m_rewardSet
			});
		}

		// Token: 0x04000726 RID: 1830
		private const string RULE_NAME = "mission_reward";

		// Token: 0x04000727 RID: 1831
		private static readonly string[] RULE_CFG_ATTRS = new string[]
		{
			"enabled",
			"mission_type",
			"reward_set",
			"message",
			"use_notification"
		};

		// Token: 0x04000728 RID: 1832
		private readonly ISpecialProfileRewardService m_specialRewards;

		// Token: 0x04000729 RID: 1833
		private readonly IRewardService m_rewardService;

		// Token: 0x0400072A RID: 1834
		private readonly MasterServer.GameLogic.MissionSystem.IMissionSystem m_missionSystem;

		// Token: 0x0400072B RID: 1835
		private readonly string m_missionType;

		// Token: 0x0400072C RID: 1836
		private readonly string m_rewardSet;
	}
}
