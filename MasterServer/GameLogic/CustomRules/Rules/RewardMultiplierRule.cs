using System;
using System.Threading.Tasks;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.Users;
using NCrontab;
using Util.Common;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002D3 RID: 723
	[CustomRule("reward_multiplier")]
	internal class RewardMultiplierRule : CustomRule, IRewardMultiplierProvider
	{
		// Token: 0x06000F70 RID: 3952 RVA: 0x0003E2F8 File Offset: 0x0003C6F8
		public RewardMultiplierRule(XmlElement config, IRewardMultiplierService rewardMultiplierService, ITimeSource timeSource, IUserRepository userRepository, ILogService logService, INotificationService notificationService, ITagService tagService) : base(config, userRepository, logService, notificationService, tagService, RewardMultiplierRule.RULE_CFG_ATTRS)
		{
			this.m_rewardMultiplierService = rewardMultiplierService;
			this.m_timeSource = timeSource;
			this.m_tagService = tagService;
			string attribute = config.GetAttribute("schedule");
			if (!string.IsNullOrEmpty(attribute))
			{
				this.m_schedule = CrontabSchedule.Parse(attribute);
				this.m_duration = TimeSpan.FromSeconds(double.Parse(config.GetAttribute("duration_sec")));
			}
			this.m_tagFilter = new TagsFilter(config.GetAttribute("tag_filter"));
			string attributeDefault = config.GetAttributeDefault("description", string.Empty);
			this.m_multiplier = new SRewardMultiplier
			{
				MoneyMultiplier = float.Parse(config.GetAttributeDefault("money_multiplier", "1")),
				ExperienceMultiplier = float.Parse(config.GetAttributeDefault("experience_multiplier", "1")),
				SponsorPointsMultiplier = float.Parse(config.GetAttributeDefault("sponsor_points_multiplier", "1")),
				CrownMultiplier = float.Parse(config.GetAttributeDefault("crown_multiplier", "1")),
				Description = attributeDefault,
				ProviderID = string.Format("{0}: {1}", base.RuleID, attributeDefault)
			};
			if (this.m_multiplier.IsEmpty())
			{
				throw new ApplicationException("Rule has empty reward multiplier");
			}
		}

		// Token: 0x06000F71 RID: 3953 RVA: 0x0003E453 File Offset: 0x0003C853
		public override bool IsActive()
		{
			return base.Enabled;
		}

		// Token: 0x06000F72 RID: 3954 RVA: 0x0003E45B File Offset: 0x0003C85B
		public override void Activate()
		{
			this.m_rewardMultiplierService.RegisterRewardMultiplierProvider(this);
		}

		// Token: 0x06000F73 RID: 3955 RVA: 0x0003E469 File Offset: 0x0003C869
		public override void Dispose()
		{
			this.m_rewardMultiplierService.UnregisterRewardMultiplierProvider(this);
		}

		// Token: 0x06000F74 RID: 3956 RVA: 0x0003E478 File Offset: 0x0003C878
		public Task<SRewardMultiplier> GetMultipliers(ulong profileId)
		{
			SRewardMultiplier result;
			if (this.CheckConditions(profileId))
			{
				UserInfo.User user = this.UserRepository.GetUser(profileId);
				base.ReportCustomRuleTriggered(user.UserID, user.ProfileID);
				result = this.m_multiplier;
			}
			else
			{
				result = SRewardMultiplier.Empty;
			}
			return TaskHelpers.Completed<SRewardMultiplier>(result);
		}

		// Token: 0x06000F75 RID: 3957 RVA: 0x0003E4C8 File Offset: 0x0003C8C8
		private bool CheckConditions(ulong profileId)
		{
			UserInfo.User user = this.UserRepository.GetUser(profileId);
			if (user == null || !this.m_tagFilter.Check(this.m_tagService.GetUserTags(user.UserID)))
			{
				return false;
			}
			if (this.m_schedule == null)
			{
				return true;
			}
			DateTime dateTime = this.m_timeSource.Now();
			DateTime nextOccurrence = this.m_schedule.GetNextOccurrence(dateTime - this.m_duration);
			return nextOccurrence <= dateTime;
		}

		// Token: 0x06000F76 RID: 3958 RVA: 0x0003E544 File Offset: 0x0003C944
		protected override ulong GetRuleID(XmlElement config)
		{
			ulong num = (ulong)((long)"reward_multiplier".GetHashCode());
			num ^= (ulong)((long)config.GetAttribute("schedule").GetHashCode());
			num ^= (ulong)((long)config.GetAttribute("duration_sec").GetHashCode());
			num ^= (ulong)((long)config.GetAttribute("tag_filter").GetHashCode());
			num ^= (ulong)((long)config.GetAttributeDefault("money_multiplier", "1").GetHashCode());
			num ^= (ulong)((long)config.GetAttributeDefault("experience_multiplier", "1").GetHashCode());
			num ^= (ulong)((long)config.GetAttributeDefault("sponsor_points_multiplier", "1").GetHashCode());
			num ^= (ulong)((long)config.GetAttributeDefault("crown_multiplier", "1").GetHashCode());
			return num ^ (ulong)((long)config.GetAttributeDefault("description", string.Empty).GetHashCode());
		}

		// Token: 0x06000F77 RID: 3959 RVA: 0x0003E618 File Offset: 0x0003CA18
		public override string ToString()
		{
			return string.Format("{0} id={1} schedule='{2}' duration_sec='{3}' {4}", new object[]
			{
				"reward_multiplier",
				base.RuleID,
				this.m_schedule,
				this.m_duration.TotalSeconds,
				this.m_multiplier
			});
		}

		// Token: 0x04000734 RID: 1844
		private const string RULE_NAME = "reward_multiplier";

		// Token: 0x04000735 RID: 1845
		private static readonly string[] RULE_CFG_ATTRS = new string[]
		{
			"enabled",
			"schedule",
			"duration_sec",
			"tag_filter",
			"money_multiplier",
			"experience_multiplier",
			"sponsor_points_multiplier",
			"crown_multiplier",
			"description"
		};

		// Token: 0x04000736 RID: 1846
		private const string DEFAULT_MULTIPLIER = "1";

		// Token: 0x04000737 RID: 1847
		private readonly CrontabSchedule m_schedule;

		// Token: 0x04000738 RID: 1848
		private readonly TimeSpan m_duration;

		// Token: 0x04000739 RID: 1849
		private readonly TagsFilter m_tagFilter;

		// Token: 0x0400073A RID: 1850
		private readonly SRewardMultiplier m_multiplier;

		// Token: 0x0400073B RID: 1851
		private readonly IRewardMultiplierService m_rewardMultiplierService;

		// Token: 0x0400073C RID: 1852
		private readonly ITimeSource m_timeSource;
	}
}
