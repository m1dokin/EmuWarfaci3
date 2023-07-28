using System;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.CustomRules;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.PlayerStats;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SpecialProfileRewards;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000404 RID: 1028
	[ProfileProgressionRule(Name = "tutorial_unlock")]
	internal class TutorialUnlockRule : ProfileProgressionRuleBase
	{
		// Token: 0x0600163F RID: 5695 RVA: 0x0005DCD4 File Offset: 0x0005C0D4
		public TutorialUnlockRule(IProfileProgressionService progressionService, ISpecialProfileRewardService specialRewards, IPlayerStatsService playerStatsService, IUserRepository userRepository, IDALService dalService, ICatalogService catalogService, IProfileItems profileItemsService, ICustomRulesService customRulesService, ICustomRulesStateStorage customRulesStateStorage) : base(progressionService, userRepository, specialRewards, playerStatsService, dalService, catalogService, profileItemsService, customRulesService, customRulesStateStorage)
		{
		}

		// Token: 0x06001640 RID: 5696 RVA: 0x0005DCF6 File Offset: 0x0005C0F6
		public override void Init(ConfigSection section)
		{
			base.Init(section);
			this.m_tutorial = (ProfileProgressionInfo.Tutorial)Enum.Parse(typeof(ProfileProgressionInfo.Tutorial), section.Get("unlock_type"), true);
		}

		// Token: 0x06001641 RID: 5697 RVA: 0x0005DD28 File Offset: 0x0005C128
		public override ProfileProgressionInfo ProcessRewardData(MissionContext missionContext, RewardOutputData aggRewardData, ILogGroup logGroup)
		{
			ProfileProgressionInfo profileProgressionInfo = new ProfileProgressionInfo(aggRewardData.profileId, this.DalService);
			if (this.Check(aggRewardData.profileId, (long)aggRewardData.sessionTime.TotalSeconds))
			{
				profileProgressionInfo = this.TrigerRule(aggRewardData.profileId, profileProgressionInfo, logGroup);
			}
			return profileProgressionInfo;
		}

		// Token: 0x06001642 RID: 5698 RVA: 0x0005DD78 File Offset: 0x0005C178
		public override ProfileProgressionInfo TrigerRule(ulong profileId, ProfileProgressionInfo info, ILogGroup logGroup)
		{
			if (!info.IsTutorialUnlocked(this.m_tutorial) && this.Check(profileId, 0L))
			{
				info |= this.ProgressionService.UnlockTutorial(info, this.m_tutorial, this.Silent, logGroup);
				base.ProcessRewardEvent(profileId, logGroup);
			}
			return info;
		}

		// Token: 0x06001643 RID: 5699 RVA: 0x0005DDD0 File Offset: 0x0005C1D0
		protected override bool Check(ulong profileId, long sessionTime)
		{
			UserInfo.User user = this.UserRepository.GetUser(profileId);
			return (user == null || !user.ProfileProgression.IsTutorialUnlocked(this.m_tutorial)) && base.Check(profileId, sessionTime);
		}

		// Token: 0x06001644 RID: 5700 RVA: 0x0005DE10 File Offset: 0x0005C210
		public override string ToString()
		{
			return string.Format("<tutorial_unlock playtime={0} rank_reached={1} unlock_type={2} silent={3} special_reward={4}>", new object[]
			{
				base.PlayTime,
				this.RankReached,
				this.m_tutorial,
				this.Silent,
				this.Event
			});
		}

		// Token: 0x04000AD0 RID: 2768
		private ProfileProgressionInfo.Tutorial m_tutorial;
	}
}
