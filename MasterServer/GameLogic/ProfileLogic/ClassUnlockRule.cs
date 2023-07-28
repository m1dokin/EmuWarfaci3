using System;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.CustomRules;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.PlayerStats;
using MasterServer.GameLogic.SpecialProfileRewards;
using MasterServer.GameLogic.StatsTracking;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x020003FE RID: 1022
	[ProfileProgressionRule(Name = "class_unlock")]
	internal class ClassUnlockRule : ProfileProgressionRuleBase
	{
		// Token: 0x06001618 RID: 5656 RVA: 0x0005D4B0 File Offset: 0x0005B8B0
		public ClassUnlockRule(ITutorialStatsTracker statsTracker, IProfileProgressionService progressionService, ISpecialProfileRewardService specialRewards, IPlayerStatsService playerStatsService, IUserRepository userRepository, IDALService dalService, ICatalogService catalogService, IProfileItems profileItemsService, ICustomRulesService customRulesService, ICustomRulesStateStorage customRulesStateStorage) : base(progressionService, userRepository, specialRewards, playerStatsService, dalService, catalogService, profileItemsService, customRulesService, customRulesStateStorage)
		{
			this.m_statsTracker = statsTracker;
			this.m_statsTracker.OnTutorialCompleted += this.Check;
		}

		// Token: 0x06001619 RID: 5657 RVA: 0x0005D4F4 File Offset: 0x0005B8F4
		public override void Init(ConfigSection section)
		{
			base.Init(section);
			string value = section.Get("tutorial_passed");
			this.m_tutorial = ((!string.IsNullOrEmpty(value)) ? Utils.ParseEnum<ProfileProgressionInfo.Tutorial>(value) : ProfileProgressionInfo.Tutorial.None);
			this.m_playerClass = Utils.ParseEnum<ProfileProgressionInfo.PlayerClass>(section.Get("unlock_class"));
		}

		// Token: 0x0600161A RID: 5658 RVA: 0x0005D547 File Offset: 0x0005B947
		public override void Dispose()
		{
			base.Dispose();
			this.m_statsTracker.OnTutorialCompleted -= this.Check;
		}

		// Token: 0x0600161B RID: 5659 RVA: 0x0005D568 File Offset: 0x0005B968
		private void Check(ulong profileId, int tutorialId, ref ProfileProgressionInfo output, ILogGroup logGroup)
		{
			if (this.Check(profileId) && tutorialId != 0)
			{
				ProfileProgressionInfo.Tutorial tutorial = (ProfileProgressionInfo.Tutorial)Enum.Parse(typeof(ProfileProgressionInfo.Tutorial), string.Format("tutorial_{0}", tutorialId), true);
				if (tutorial == this.m_tutorial)
				{
					output |= this.ProgressionService.UnlockClass(output, this.m_playerClass, this.Silent, logGroup);
					if (output.IsClassUnlocked(this.m_playerClass))
					{
						base.ProcessRewardEvent(profileId, logGroup);
					}
				}
			}
		}

		// Token: 0x0600161C RID: 5660 RVA: 0x0005D5FC File Offset: 0x0005B9FC
		public override ProfileProgressionInfo TrigerRule(ulong profileId, ProfileProgressionInfo info, ILogGroup logGroup)
		{
			if (!info.IsClassUnlocked(this.m_playerClass) && this.m_tutorial == ProfileProgressionInfo.Tutorial.None)
			{
				info |= this.ProgressionService.UnlockClass(info, this.m_playerClass, this.Silent, logGroup);
				base.ProcessRewardEvent(profileId, logGroup);
			}
			return info;
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x0005D650 File Offset: 0x0005BA50
		public override string ToString()
		{
			return string.Format("<class_unlock tutorial_passed={0} unlock_class={1} silent={2} special_reward={3}>", new object[]
			{
				this.m_tutorial,
				this.m_playerClass,
				this.Silent,
				this.Event
			});
		}

		// Token: 0x04000AB0 RID: 2736
		private readonly ITutorialStatsTracker m_statsTracker;

		// Token: 0x04000AB1 RID: 2737
		private ProfileProgressionInfo.PlayerClass m_playerClass;

		// Token: 0x04000AB2 RID: 2738
		private ProfileProgressionInfo.Tutorial m_tutorial;
	}
}
