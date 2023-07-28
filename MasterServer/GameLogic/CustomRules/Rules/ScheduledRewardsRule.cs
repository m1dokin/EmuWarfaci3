using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.Core;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.SpecialProfileRewards;
using MasterServer.Users;
using NCrontab;
using Util.Common;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002D4 RID: 724
	[CustomRule("scheduled_reward")]
	internal class ScheduledRewardsRule : CustomRule
	{
		// Token: 0x06000F79 RID: 3961 RVA: 0x0003E6DC File Offset: 0x0003CADC
		public ScheduledRewardsRule(XmlElement config, ILogService logService, IUserRepository userRepository, ICustomRulesStateStorage stateStorage, ISpecialProfileRewardService specialRewards, INotificationService notificationService, ITagService tagService, ITimeSource timeSource) : base(config, userRepository, logService, notificationService, tagService, ScheduledRewardsRule.RULE_CFG_ATTRS)
		{
			this.m_stateStorage = stateStorage;
			this.m_specialRewards = specialRewards;
			this.m_timeSource = timeSource;
			this.m_loginFilter = this.GetLoginFilter(config);
			this.m_tagFilter = new TagsFilter(config.GetAttribute("tag_filter"));
			this.m_rewardSet = config.GetAttribute("reward_set");
			if (config.HasAttribute("rank"))
			{
				this.m_rank = int.Parse(config.GetAttribute("rank"));
			}
			string attribute = config.GetAttribute("schedule");
			if (!string.IsNullOrEmpty(attribute))
			{
				this.m_schedule = CrontabSchedule.Parse(attribute);
			}
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x0003E790 File Offset: 0x0003CB90
		public override bool IsActive()
		{
			return base.Enabled;
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x0003E798 File Offset: 0x0003CB98
		public override void Activate()
		{
			this.UserRepository.UserLoggingIn += this.OnUserLoggingIn;
		}

		// Token: 0x06000F7C RID: 3964 RVA: 0x0003E7B1 File Offset: 0x0003CBB1
		public override void Dispose()
		{
			this.UserRepository.UserLoggingIn -= this.OnUserLoggingIn;
		}

		// Token: 0x06000F7D RID: 3965 RVA: 0x0003E7CC File Offset: 0x0003CBCC
		private ELoginType[] GetLoginFilter(XmlElement config)
		{
			string attribute = config.GetAttribute("login_filter");
			if (string.IsNullOrEmpty(attribute))
			{
				return ScheduledRewardsRule.DEFAULT_LOGIN_FILTER;
			}
			return (from lt in attribute.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries)
			select ReflectionUtils.EnumParse<ELoginType>(lt.Trim())).ToArray<ELoginType>();
		}

		// Token: 0x06000F7E RID: 3966 RVA: 0x0003E830 File Offset: 0x0003CC30
		protected override ulong GetRuleID(XmlElement config)
		{
			ulong num = (ulong)((long)"scheduled_reward".GetHashCode());
			string[] array = new string[]
			{
				"login_filter",
				"tag_filter",
				"schedule",
				"reward_set"
			};
			for (int num2 = 0; num2 != array.Length; num2++)
			{
				num ^= (ulong)((ulong)((long)config.GetAttribute(array[num2]).GetHashCode()) << 32 * (num2 % 2));
			}
			return num;
		}

		// Token: 0x06000F7F RID: 3967 RVA: 0x0003E8A4 File Offset: 0x0003CCA4
		private void OnUserLoggingIn(UserInfo.User user, ELoginType type, DateTime lastSeen)
		{
			Log.Verbose(Log.Group.CustomRules, "Checking user '{0}' for eligibility by rule '{1}'", new object[]
			{
				user,
				this
			});
			if (this.CheckFilters(user, type) && this.CheckSchedule(user))
			{
				Log.Verbose(Log.Group.CustomRules, "Rule '{0}' is triggered for user '{1}'", new object[]
				{
					base.RuleID,
					user.UserID
				});
				using (ILogGroup logGroup = this.LogService.CreateGroup())
				{
					base.ReportCustomRuleTriggered(user.UserID, user.ProfileID, logGroup);
					List<SNotification> notifications = this.m_specialRewards.ProcessEvent(this.m_rewardSet, user.ProfileID, logGroup);
					base.SendNotifications(user.ProfileID, notifications);
				}
			}
		}

		// Token: 0x06000F80 RID: 3968 RVA: 0x0003E980 File Offset: 0x0003CD80
		private bool CheckFilters(UserInfo.User user, ELoginType type)
		{
			UserTags userTags = this.m_tagService.GetUserTags(user.UserID);
			return this.m_loginFilter.Contains(type) && this.m_tagFilter.Check(userTags) && user.Rank >= this.m_rank;
		}

		// Token: 0x06000F81 RID: 3969 RVA: 0x0003E9D5 File Offset: 0x0003CDD5
		private bool CheckSchedule(UserInfo.User user)
		{
			return this.m_schedule == null || this.m_stateStorage.UpdateState(user.ProfileID, this, delegate(CustomRuleState st)
			{
				ScheduledRewardsRuleState scheduledRewardsRuleState = (ScheduledRewardsRuleState)st;
				DateTime dateTime = this.m_timeSource.Now();
				DateTime nextOccurrence = this.m_schedule.GetNextOccurrence(scheduledRewardsRuleState.LastActivationTime.ToLocalTime());
				if (nextOccurrence >= dateTime)
				{
					return false;
				}
				scheduledRewardsRuleState.LastActivationTime = dateTime;
				return true;
			});
		}

		// Token: 0x06000F82 RID: 3970 RVA: 0x0003EA04 File Offset: 0x0003CE04
		public override string ToString()
		{
			string format = "{0} id={1} login_filter='{2}' tag_filter='{3}' schedule='{4}' message='{5}' reward_set='{6}'";
			object[] array = new object[7];
			array[0] = "scheduled_reward";
			array[1] = base.RuleID;
			array[2] = string.Join(",", (from x in this.m_loginFilter
			select x.ToString()).ToArray<string>());
			array[3] = this.m_tagFilter;
			array[4] = this.m_schedule;
			array[5] = base.Message;
			array[6] = this.m_rewardSet;
			return string.Format(format, array);
		}

		// Token: 0x0400073D RID: 1853
		public const string RULE_NAME = "scheduled_reward";

		// Token: 0x0400073E RID: 1854
		private static readonly ELoginType[] DEFAULT_LOGIN_FILTER = new ELoginType[]
		{
			ELoginType.Ordinary,
			ELoginType.NewProfile
		};

		// Token: 0x0400073F RID: 1855
		private static readonly string[] RULE_CFG_ATTRS = new string[]
		{
			"enabled",
			"login_filter",
			"tag_filter",
			"schedule",
			"reward_set",
			"rank",
			"message",
			"use_notification"
		};

		// Token: 0x04000740 RID: 1856
		private readonly ICustomRulesStateStorage m_stateStorage;

		// Token: 0x04000741 RID: 1857
		private readonly ISpecialProfileRewardService m_specialRewards;

		// Token: 0x04000742 RID: 1858
		private ITimeSource m_timeSource;

		// Token: 0x04000743 RID: 1859
		private readonly ELoginType[] m_loginFilter;

		// Token: 0x04000744 RID: 1860
		private readonly TagsFilter m_tagFilter;

		// Token: 0x04000745 RID: 1861
		private readonly CrontabSchedule m_schedule;

		// Token: 0x04000746 RID: 1862
		private readonly string m_rewardSet;

		// Token: 0x04000747 RID: 1863
		private readonly int m_rank;
	}
}
