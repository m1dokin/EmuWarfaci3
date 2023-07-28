using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.RatingSystem;
using MasterServer.GameLogic.SpecialProfileRewards;
using MasterServer.Users;

namespace MasterServer.GameLogic.CustomRules.Rules.RatingSeason
{
	// Token: 0x020000A9 RID: 169
	[CustomRule("rating_season_rule")]
	internal class RatingSeasonRule : CustomRule
	{
		// Token: 0x060002A3 RID: 675 RVA: 0x0000D4C8 File Offset: 0x0000B8C8
		public RatingSeasonRule(XmlElement config, ILogService logService, IUserRepository userRepository, ICustomRulesStateStorage stateStorage, ISpecialProfileRewardService specialRewards, INotificationService notificationService, ITagService tagService, IRatingService ratingService, IRatingSeasonService ratingSeasonService, IConfigProvider<RatingConfig> ratingConfigProvider, IConfigProvider<RatingSeasonConfig> ratingSeasonConfigProvider) : base(config, userRepository, logService, notificationService, tagService, RatingSeasonRule.RULE_CFG_ATTRS)
		{
			this.m_ruleStateStorage = stateStorage;
			this.m_specialRewardsService = specialRewards;
			this.m_ratingService = ratingService;
			this.m_ratingSeasonService = ratingSeasonService;
			this.m_ratingConfigProvider = ratingConfigProvider;
			this.m_ratingSeasonConfigProvider = ratingSeasonConfigProvider;
			this.SetupSeasonConfig();
			this.SetupSeasonRewardSet();
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x0000D523 File Offset: 0x0000B923
		public override bool IsActive()
		{
			return true;
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x0000D528 File Offset: 0x0000B928
		public override void Activate()
		{
			this.m_ratingSeasonService.ProfileRatingReseted += this.OnProfileRatingReseted;
			this.m_ratingService.ProfileRatingChanged += this.OnProfileRatingChanged;
			this.UserRepository.UserLoggingIn += this.OnUserLoggingIn;
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0000D57C File Offset: 0x0000B97C
		public override void Dispose()
		{
			this.m_ratingSeasonService.ProfileRatingReseted -= this.OnProfileRatingReseted;
			this.m_ratingService.ProfileRatingChanged -= this.OnProfileRatingChanged;
			this.UserRepository.UserLoggingIn -= this.OnUserLoggingIn;
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x0000D5D0 File Offset: 0x0000B9D0
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string value = string.Format("Rule: name='{0}', id='{1}', enabled='{2}'", "rating_season_rule", base.RuleID, base.Enabled);
			stringBuilder.AppendLine(value);
			stringBuilder.AppendLine("Rewards:");
			stringBuilder.Append(this.m_ratingRewardSet);
			return stringBuilder.ToString();
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0000D630 File Offset: 0x0000BA30
		protected override ulong GetRuleID(XmlElement config)
		{
			return (ulong)((long)"rating_season_rule".GetHashCode());
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x0000D640 File Offset: 0x0000BA40
		private void SetupSeasonConfig()
		{
			RatingSeasonRuleConfig seasonConfig = this.GetSeasonConfig();
			this.m_ratingSeasonService.SetupSeason(seasonConfig, base.Enabled);
		}

		// Token: 0x060002AA RID: 682 RVA: 0x0000D668 File Offset: 0x0000BA68
		private RatingSeasonRuleConfig GetSeasonConfig()
		{
			RatingSeasonRuleConfigParser ratingSeasonRuleConfigParser = new RatingSeasonRuleConfigParser();
			RatingSeasonRuleConfig ratingSeasonRuleConfig = ratingSeasonRuleConfigParser.Parse(base.Config);
			RatingSeasonRuleConfigValidator ratingSeasonRuleConfigValidator = new RatingSeasonRuleConfigValidator();
			ratingSeasonRuleConfigValidator.Validate(ratingSeasonRuleConfig);
			return ratingSeasonRuleConfig;
		}

		// Token: 0x060002AB RID: 683 RVA: 0x0000D698 File Offset: 0x0000BA98
		private void SetupSeasonRewardSet()
		{
			RatingSeasonRewardsParser ratingSeasonRewardsParser = new RatingSeasonRewardsParser();
			Dictionary<uint, RatingSeasonReward> rewards = ratingSeasonRewardsParser.Parse(base.Config);
			RatingSeasonRewardsValidator ratingSeasonRewardsValidator = new RatingSeasonRewardsValidator(this.m_specialRewardsService, this.m_ratingConfigProvider);
			ratingSeasonRewardsValidator.Validate(rewards);
			RatingSeasonRewardSet value = new RatingSeasonRewardSet(rewards);
			Interlocked.Exchange<RatingSeasonRewardSet>(ref this.m_ratingRewardSet, value);
		}

		// Token: 0x060002AC RID: 684 RVA: 0x0000D6E8 File Offset: 0x0000BAE8
		private void OnUserLoggingIn(UserInfo.User user, ELoginType type, DateTime lastSeen)
		{
			RatingSeasonConfig ratingSeasonConfig = this.m_ratingSeasonConfigProvider.Get();
			if (ratingSeasonConfig.Enabled && base.Enabled)
			{
				Rating rating = this.m_ratingService.GetRating(user.ProfileID);
				if (rating.Level > 0U)
				{
					this.GiveSeasonReward(user.UserID, user.ProfileID, rating.Level);
				}
			}
		}

		// Token: 0x060002AD RID: 685 RVA: 0x0000D750 File Offset: 0x0000BB50
		private void OnProfileRatingReseted(ulong profileId, Rating rating)
		{
			this.m_ruleStateStorage.UpdateState(profileId, this, delegate(CustomRuleState st)
			{
				RatingSeasonRuleState ratingSeasonRuleState = (RatingSeasonRuleState)st;
				RatingConfig ratingConfig = this.m_ratingConfigProvider.Get();
				RatingLevelConfig ratingLevelConfigByPoints = ratingConfig.GetRatingLevelConfigByPoints(rating.Points);
				ratingSeasonRuleState.MaxRatingLevelAchieved = ratingLevelConfigByPoints.Level;
				ratingSeasonRuleState.SeasonId = rating.SeasonId;
				return true;
			});
		}

		// Token: 0x060002AE RID: 686 RVA: 0x0000D78C File Offset: 0x0000BB8C
		private void OnProfileRatingChanged(ulong userId, ulong profileId, Rating oldRating, Rating newRating, string sessionId, ILogGroup logGroup)
		{
			if (newRating.Level != oldRating.Level)
			{
				this.UpdateSeasonReward(profileId, newRating.Level);
			}
			if (newRating.Level > oldRating.Level)
			{
				this.GiveRatingAchievedReward(userId, profileId, oldRating.Level, newRating.Level, logGroup);
			}
		}

		// Token: 0x060002AF RID: 687 RVA: 0x0000D7E4 File Offset: 0x0000BBE4
		private void GiveSeasonReward(ulong userId, ulong profileId, uint ratingLevel)
		{
			this.m_ruleStateStorage.UpdateState(profileId, this, delegate(CustomRuleState st)
			{
				RatingSeasonRuleState ratingSeasonRuleState = (RatingSeasonRuleState)st;
				if (!this.ShouldGiveSeasonRewards(ratingSeasonRuleState))
				{
					return false;
				}
				using (ILogGroup logGroup = this.LogService.CreateGroup())
				{
					this.ReportCustomRuleTriggered(userId, profileId, logGroup);
					List<SNotification> notifications = this.m_specialRewardsService.ProcessEvent(ratingSeasonRuleState.SeasonResultRewardName, profileId, logGroup);
					logGroup.RatingSeasonRewardGivenLog(userId, profileId, ratingLevel, ratingSeasonRuleState.SeasonId, ratingSeasonRuleState.SeasonResultRewardName, LogGroup.ProduceType.RatingSeasonResult);
					this.SendNotifications(profileId, notifications);
				}
				ratingSeasonRuleState.SeasonResultRewardName = string.Empty;
				return true;
			});
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x0000D834 File Offset: 0x0000BC34
		private bool ShouldGiveSeasonRewards(RatingSeasonRuleState ruleState)
		{
			RatingSeason ratingSeason = this.m_ratingSeasonService.GetRatingSeason();
			bool flag = ratingSeason.SeasonId != ruleState.SeasonId;
			return ruleState.HasSeasonReward && (!ratingSeason.IsActive || flag);
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x0000D880 File Offset: 0x0000BC80
		private void UpdateSeasonReward(ulong profileId, uint newLevel)
		{
			this.m_ruleStateStorage.UpdateState(profileId, this, delegate(CustomRuleState st)
			{
				RatingSeasonRuleState ratingSeasonRuleState = (RatingSeasonRuleState)st;
				RatingSeasonReward rewardForRatingLevel = this.m_ratingRewardSet.GetRewardForRatingLevel(newLevel);
				if (ratingSeasonRuleState.SeasonResultRewardName != rewardForRatingLevel.SeasonResultRewardName)
				{
					ratingSeasonRuleState.SeasonResultRewardName = rewardForRatingLevel.SeasonResultRewardName;
					return true;
				}
				return false;
			});
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x0000D8BC File Offset: 0x0000BCBC
		private void GiveRatingAchievedReward(ulong userId, ulong profileId, uint oldLevel, uint newLevel, ILogGroup logGroup)
		{
			this.m_ruleStateStorage.UpdateState(profileId, this, delegate(CustomRuleState st)
			{
				RatingSeasonRuleState ratingSeasonRuleState = (RatingSeasonRuleState)st;
				if (newLevel > ratingSeasonRuleState.MaxRatingLevelAchieved)
				{
					ratingSeasonRuleState.MaxRatingLevelAchieved = newLevel;
					this.ReportCustomRuleTriggered(userId, profileId, logGroup);
					List<SNotification> list = new List<SNotification>();
					for (uint num = oldLevel + 1U; num <= newLevel; num += 1U)
					{
						RatingSeasonReward rewardForRatingLevel = this.m_ratingRewardSet.GetRewardForRatingLevel(num);
						list.AddRange(this.m_specialRewardsService.ProcessEvent(rewardForRatingLevel.RatingAchievedRewardName, profileId, logGroup));
						logGroup.RatingSeasonRewardGivenLog(userId, profileId, num, ratingSeasonRuleState.SeasonId, rewardForRatingLevel.RatingAchievedRewardName, LogGroup.ProduceType.RatingAchieved);
					}
					this.SendNotifications(profileId, list);
					return true;
				}
				return false;
			});
		}

		// Token: 0x04000120 RID: 288
		public const string RULE_NAME = "rating_season_rule";

		// Token: 0x04000121 RID: 289
		private static readonly string[] RULE_CFG_ATTRS = new string[]
		{
			"enabled",
			"season_id_template",
			"banner",
			"description",
			"rules",
			"announcement_end_date",
			"games_end_date"
		};

		// Token: 0x04000122 RID: 290
		private readonly ICustomRulesStateStorage m_ruleStateStorage;

		// Token: 0x04000123 RID: 291
		private readonly ISpecialProfileRewardService m_specialRewardsService;

		// Token: 0x04000124 RID: 292
		private readonly IRatingService m_ratingService;

		// Token: 0x04000125 RID: 293
		private readonly IRatingSeasonService m_ratingSeasonService;

		// Token: 0x04000126 RID: 294
		private readonly IConfigProvider<RatingConfig> m_ratingConfigProvider;

		// Token: 0x04000127 RID: 295
		private readonly IConfigProvider<RatingSeasonConfig> m_ratingSeasonConfigProvider;

		// Token: 0x04000128 RID: 296
		private RatingSeasonRewardSet m_ratingRewardSet;
	}
}
