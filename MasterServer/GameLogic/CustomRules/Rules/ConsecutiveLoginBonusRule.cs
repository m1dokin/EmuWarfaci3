using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using MasterServer.Core;
using MasterServer.GameLogic.ItemsSystem.RandomBoxChoiceLimitation;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.SpecialProfileRewards;
using MasterServer.Users;
using NCrontab;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002BB RID: 699
	[CustomRule("consecutive_login_bonus")]
	internal class ConsecutiveLoginBonusRule : CustomRule
	{
		// Token: 0x06000F02 RID: 3842 RVA: 0x0003BED8 File Offset: 0x0003A2D8
		public ConsecutiveLoginBonusRule(XmlElement config, ILogService logService, IUserRepository userRepository, ICustomRulesStateStorage stateStorage, ISpecialProfileRewardService specialRewards, INotificationService notificationService, ITagService tagService, IRandomBoxChoiceLimitationService randomBoxChoiceLimitation) : base(config, userRepository, logService, notificationService, tagService, ConsecutiveLoginBonusRule.RULE_CFG_ATTRS)
		{
			this.m_stateStorage = stateStorage;
			this.m_specialRewards = specialRewards;
			this.m_randomBoxChoiceLimitation = randomBoxChoiceLimitation;
			this.m_schedule = CrontabSchedule.Parse(config.GetAttribute("schedule"));
			string attribute = config.GetAttribute("expiration");
			this.m_expiration = TimeSpan.Parse(attribute);
			this.InitAndValidationRewardSets(base.Config);
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06000F03 RID: 3843 RVA: 0x0003BF49 File Offset: 0x0003A349
		protected virtual string RuleName
		{
			get
			{
				return "consecutive_login_bonus";
			}
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06000F04 RID: 3844 RVA: 0x0003BF50 File Offset: 0x0003A350
		// (set) Token: 0x06000F05 RID: 3845 RVA: 0x0003BF58 File Offset: 0x0003A358
		private ConsecutiveLoginBonusRewardSet RewardSet { get; set; }

		// Token: 0x06000F06 RID: 3846 RVA: 0x0003BF61 File Offset: 0x0003A361
		public override bool IsActive()
		{
			return true;
		}

		// Token: 0x06000F07 RID: 3847 RVA: 0x0003BF64 File Offset: 0x0003A364
		public override void Activate()
		{
			this.UserRepository.UserLoggingIn += this.OnUserLoggingIn;
			ServicesManager.OnExecutionPhaseChanged += this.OnExecutionPhaseChanged;
		}

		// Token: 0x06000F08 RID: 3848 RVA: 0x0003BF8E File Offset: 0x0003A38E
		public override void Dispose()
		{
			ServicesManager.OnExecutionPhaseChanged -= this.OnExecutionPhaseChanged;
			this.UserRepository.UserLoggingIn -= this.OnUserLoggingIn;
		}

		// Token: 0x06000F09 RID: 3849 RVA: 0x0003BFB8 File Offset: 0x0003A3B8
		protected override ulong GetRuleID(XmlElement config)
		{
			return (ulong)((long)this.RuleName.GetHashCode());
		}

		// Token: 0x06000F0A RID: 3850 RVA: 0x0003BFC8 File Offset: 0x0003A3C8
		private void OnUserLoggingIn(UserInfo.User user, ELoginType type, DateTime lastSeen)
		{
			Log.Verbose(Log.Group.CustomRules, "Checking user '{0}' for eligibility by rule '{1}'", new object[]
			{
				user,
				this
			});
			if (type != ELoginType.SwitchChannel)
			{
				using (ILogGroup logGroup = this.LogService.CreateGroup())
				{
					XmlElement xmlElement = new XmlDocument().CreateElement("consecutive_login_bonus");
					string setName;
					if (this.CheckScheduleAndGetReward(user, type, logGroup, xmlElement, out setName))
					{
						base.ReportCustomRuleTriggered(user.UserID, user.ProfileID, logGroup);
						List<SNotification> notifications = this.m_specialRewards.ProcessEvent(setName, user.ProfileID, logGroup, xmlElement);
						base.SendNotifications(user.ProfileID, notifications);
					}
				}
			}
		}

		// Token: 0x06000F0B RID: 3851 RVA: 0x0003C080 File Offset: 0x0003A480
		private bool CheckScheduleAndGetReward(UserInfo.User user, ELoginType type, ILogGroup group, XmlElement clbInfoEl, out string reward)
		{
			ConsecutiveLoginBonusRewardSet.Result res = default(ConsecutiveLoginBonusRewardSet.Result);
			bool flag = this.m_stateStorage.UpdateState(user.ProfileID, this, delegate(CustomRuleState st)
			{
				ConsecutiveLoginBonusRuleState consecutiveLoginBonusRuleState = (ConsecutiveLoginBonusRuleState)st;
				DateTime baseTime = consecutiveLoginBonusRuleState.LastActivationTime.ToLocalTime();
				DateTime t = user.LoginTime.ToLocalTime();
				DateTime nextOccurrence = this.m_schedule.GetNextOccurrence(baseTime);
				DateTime t2 = nextOccurrence + this.m_expiration;
				if (t <= nextOccurrence)
				{
					return false;
				}
				consecutiveLoginBonusRuleState.LastActivationTime = user.LoginTime;
				if (type == ELoginType.NewProfile)
				{
					return false;
				}
				bool flag2 = t > t2;
				res = this.RewardSet.GetNextReward(consecutiveLoginBonusRuleState.PrevStreak, consecutiveLoginBonusRuleState.PrevReward, flag2);
				if (!this.Enabled)
				{
					return !flag2;
				}
				clbInfoEl.SetAttribute("previous_streak", consecutiveLoginBonusRuleState.PrevStreak.ToString(CultureInfo.InvariantCulture));
				clbInfoEl.SetAttribute("previous_reward", consecutiveLoginBonusRuleState.PrevReward.ToString(CultureInfo.InvariantCulture));
				clbInfoEl.SetAttribute("current_streak", res.CurrStreak.ToString(CultureInfo.InvariantCulture));
				clbInfoEl.SetAttribute("current_reward", res.CurrReward.ToString(CultureInfo.InvariantCulture));
				consecutiveLoginBonusRuleState.PrevStreak = res.CurrStreak;
				consecutiveLoginBonusRuleState.PrevReward = res.CurrReward;
				group.ConsecutiveLoginBonusActivationLog(this.RuleID, user.UserID, user.ProfileID, flag2, res.InputWasInvalidated, res.Reward, consecutiveLoginBonusRuleState.PrevStreak, consecutiveLoginBonusRuleState.PrevReward);
				return true;
			});
			reward = res.Reward;
			return base.Enabled && flag;
		}

		// Token: 0x06000F0C RID: 3852 RVA: 0x0003C108 File Offset: 0x0003A508
		public override string ToString()
		{
			return string.Format("{0} id={1} enabled='{2}' schedule='{3}' expiration='{4}' reward_set='{5}'", new object[]
			{
				this.RuleName,
				base.RuleID,
				base.Enabled,
				this.m_schedule,
				this.m_expiration,
				this.RewardSet
			});
		}

		// Token: 0x06000F0D RID: 3853 RVA: 0x0003C16C File Offset: 0x0003A56C
		private void InitAndValidationRewardSets(XmlElement config)
		{
			this.RewardSet = ConsecutiveLoginBonusRewardSet.Parse(config);
			if (!this.RewardSet.IsAllStreakContainReward())
			{
				throw new ConsecutiveLoginBonusRewardSetAttributeException(config.InnerXml.ToString(CultureInfo.InvariantCulture));
			}
			IEnumerable<string> rewardSetsWhichDoesNotContainSingleReward = this.RewardSet.GetRewardSetsWhichDoesNotContainSingleReward(this.m_specialRewards);
			if (rewardSetsWhichDoesNotContainSingleReward != null && rewardSetsWhichDoesNotContainSingleReward.Any<string>())
			{
				throw new ConsecutiveLoginBonusRewardSetDoesNotContainSingleRewardException(rewardSetsWhichDoesNotContainSingleReward.Aggregate((string resStr, string next) => resStr + ", " + next));
			}
		}

		// Token: 0x06000F0E RID: 3854 RVA: 0x0003C1F8 File Offset: 0x0003A5F8
		private void ValidateAfterInit()
		{
			IEnumerable<Tuple<string, string>> enumerable = this.RewardSet.FindRegularItemsInRewards(this.m_randomBoxChoiceLimitation, this.m_specialRewards);
			if (enumerable.Any<Tuple<string, string>>())
			{
				StringBuilder stringBuilder = new StringBuilder("It's forbidden to use regular items as a reward for the conscutive login bonus!");
				foreach (Tuple<string, string> tuple in enumerable)
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendFormat("Item {0} from reward {1} is regular or contains regular item inside", tuple.Item2, tuple.Item1);
				}
				throw new ConsecutiveLoginBonusValidationException(stringBuilder.ToString());
			}
		}

		// Token: 0x06000F0F RID: 3855 RVA: 0x0003C2A0 File Offset: 0x0003A6A0
		private void OnExecutionPhaseChanged(ExecutionPhase executionPhase)
		{
			if (executionPhase == ExecutionPhase.Started)
			{
				this.ValidateAfterInit();
			}
		}

		// Token: 0x040006E6 RID: 1766
		public const string RULE_NAME = "consecutive_login_bonus";

		// Token: 0x040006E7 RID: 1767
		private static readonly string[] RULE_CFG_ATTRS = new string[]
		{
			"enabled",
			"tag_filter",
			"schedule",
			"expiration",
			"message",
			"use_notification"
		};

		// Token: 0x040006E8 RID: 1768
		private readonly ICustomRulesStateStorage m_stateStorage;

		// Token: 0x040006E9 RID: 1769
		private readonly ISpecialProfileRewardService m_specialRewards;

		// Token: 0x040006EA RID: 1770
		private readonly IRandomBoxChoiceLimitationService m_randomBoxChoiceLimitation;

		// Token: 0x040006EB RID: 1771
		private readonly CrontabSchedule m_schedule;

		// Token: 0x040006EC RID: 1772
		private readonly TimeSpan m_expiration;
	}
}
