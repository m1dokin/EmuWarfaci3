using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SpecialProfileRewards;
using MasterServer.Users;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002D2 RID: 722
	[CustomRule("progression_reward")]
	internal class ProgressionRewardRule : CustomRule
	{
		// Token: 0x06000F67 RID: 3943 RVA: 0x0003E040 File Offset: 0x0003C440
		public ProgressionRewardRule(XmlElement config, IUserRepository userRepository, IRankSystem rankSystem, ISpecialProfileRewardService specialRewards, ILogService logService, INotificationService notificationService, ITagService tagService) : base(config, userRepository, logService, notificationService, tagService, ProgressionRewardRule.RULE_CFG_ATTRS)
		{
			this.m_rankSystem = rankSystem;
			this.m_specialRewards = specialRewards;
			this.m_tagService = tagService;
			this.m_tagFilter = new TagsFilter(config.GetAttribute("tag_filter"));
			this.m_rank = int.Parse(config.GetAttribute("rank"));
			this.m_rewardSet = config.GetAttribute("reward_set");
		}

		// Token: 0x06000F68 RID: 3944 RVA: 0x0003E0B4 File Offset: 0x0003C4B4
		public override bool IsActive()
		{
			return base.Enabled;
		}

		// Token: 0x06000F69 RID: 3945 RVA: 0x0003E0BC File Offset: 0x0003C4BC
		public override void Activate()
		{
			this.m_rankSystem.OnProfileRankChanged += this.OnProfileRankChanged;
		}

		// Token: 0x06000F6A RID: 3946 RVA: 0x0003E0D5 File Offset: 0x0003C4D5
		public override void Dispose()
		{
			this.m_rankSystem.OnProfileRankChanged -= this.OnProfileRankChanged;
		}

		// Token: 0x06000F6B RID: 3947 RVA: 0x0003E0F0 File Offset: 0x0003C4F0
		private void OnProfileRankChanged(SProfileInfo profile, SRankInfo newRank, SRankInfo oldRank, ILogGroup logGroup)
		{
			if (!this.CheckConditions(profile, newRank, oldRank))
			{
				return;
			}
			NewRankNotification data = new NewRankNotification(oldRank.RankId, newRank.RankId);
			List<SNotification> list = new List<SNotification>
			{
				NotificationFactory.CreateNotification<NewRankNotification>(ENotificationType.NewRankReached, data, NotificationService.DefaultNotificationTTL, EConfirmationType.None)
			};
			base.ReportCustomRuleTriggered(profile.UserID, profile.Id, logGroup);
			list.AddRange(this.m_specialRewards.ProcessEvent(this.m_rewardSet, profile.Id, logGroup));
			base.SendNotifications(profile.Id, list);
		}

		// Token: 0x06000F6C RID: 3948 RVA: 0x0003E184 File Offset: 0x0003C584
		private bool CheckConditions(SProfileInfo profile, SRankInfo newRank, SRankInfo oldRank)
		{
			if (newRank.RankId < this.m_rank || oldRank.RankId >= this.m_rank)
			{
				return false;
			}
			UserInfo.User user = this.UserRepository.GetUser(profile.Id);
			return user != null && this.m_tagFilter.Check(this.m_tagService.GetUserTags(user.UserID));
		}

		// Token: 0x06000F6D RID: 3949 RVA: 0x0003E1F0 File Offset: 0x0003C5F0
		protected override ulong GetRuleID(XmlElement config)
		{
			ulong num = (ulong)((long)"progression_reward".GetHashCode());
			string[] array = new string[]
			{
				"tag_filter",
				"rank",
				"reward_set"
			};
			for (int num2 = 0; num2 != array.Length; num2++)
			{
				num ^= (ulong)((ulong)((long)config.GetAttribute(array[num2]).GetHashCode()) << 32 * (num2 % 2));
			}
			return num;
		}

		// Token: 0x06000F6E RID: 3950 RVA: 0x0003E25C File Offset: 0x0003C65C
		public override string ToString()
		{
			return string.Format("{0} id={1} tag_filter='{2}' rank='{3}' message='{4}' reward_set='{5}'", new object[]
			{
				"progression_reward",
				base.RuleID,
				this.m_tagFilter,
				this.m_rank,
				base.Message,
				this.m_rewardSet
			});
		}

		// Token: 0x0400072D RID: 1837
		private const string RULE_NAME = "progression_reward";

		// Token: 0x0400072E RID: 1838
		private static readonly string[] RULE_CFG_ATTRS = new string[]
		{
			"enabled",
			"tag_filter",
			"rank",
			"reward_set",
			"message",
			"use_notification"
		};

		// Token: 0x0400072F RID: 1839
		private readonly IRankSystem m_rankSystem;

		// Token: 0x04000730 RID: 1840
		private readonly ISpecialProfileRewardService m_specialRewards;

		// Token: 0x04000731 RID: 1841
		private readonly TagsFilter m_tagFilter;

		// Token: 0x04000732 RID: 1842
		private readonly int m_rank;

		// Token: 0x04000733 RID: 1843
		private readonly string m_rewardSet;
	}
}
