using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.CustomRules;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.PlayerStats;
using MasterServer.GameLogic.SpecialProfileRewards;
using MasterServer.GameLogic.StatsTracking;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000403 RID: 1027
	[ProfileProgressionRule(Name = "tutorial_passed")]
	internal class TutorialPassedRule : ProfileProgressionRuleBase
	{
		// Token: 0x0600163A RID: 5690 RVA: 0x0005DB6C File Offset: 0x0005BF6C
		public TutorialPassedRule(ITutorialStatsTracker statsTracker, IProfileProgressionService progressionService, ISpecialProfileRewardService specialRewards, IPlayerStatsService playerStatsService, IUserRepository userRepository, IDALService dalService, ICatalogService catalogService, IProfileItems profileItemsService, ICustomRulesService customRulesService, ICustomRulesStateStorage customRulesStateStorage, INotificationService notificationService) : base(progressionService, userRepository, specialRewards, playerStatsService, dalService, catalogService, profileItemsService, customRulesService, customRulesStateStorage)
		{
			this.m_statsTracker = statsTracker;
			this.m_statsTracker.OnTutorialCompleted += this.Check;
			this.m_notificationService = notificationService;
		}

		// Token: 0x0600163B RID: 5691 RVA: 0x0005DBB5 File Offset: 0x0005BFB5
		public override void Init(ConfigSection section)
		{
			base.Init(section);
			this.m_tutorial = (ProfileProgressionInfo.Tutorial)Enum.Parse(typeof(ProfileProgressionInfo.Tutorial), section.Get("type"), true);
		}

		// Token: 0x0600163C RID: 5692 RVA: 0x0005DBE4 File Offset: 0x0005BFE4
		public override void Dispose()
		{
			base.Dispose();
			this.m_statsTracker.OnTutorialCompleted -= this.Check;
		}

		// Token: 0x0600163D RID: 5693 RVA: 0x0005DC04 File Offset: 0x0005C004
		private void Check(ulong profileId, int tutorialId, ref ProfileProgressionInfo output, ILogGroup logGroup)
		{
			if (this.Check(profileId) && tutorialId != 0)
			{
				ProfileProgressionInfo.Tutorial tutorial = (ProfileProgressionInfo.Tutorial)Enum.Parse(typeof(ProfileProgressionInfo.Tutorial), string.Format("tutorial_{0}", tutorialId), true);
				if (tutorial == this.m_tutorial && !output.IsTutorialPassed(tutorial))
				{
					output |= this.ProgressionService.PassTutorial(profileId, this.m_tutorial, this.Silent, logGroup);
					if (output.IsTutorialPassed(tutorial))
					{
						IEnumerable<SNotification> notifications = base.ProcessRewardEvent(profileId, logGroup);
						this.m_notificationService.AddNotifications(profileId, notifications, EDeliveryType.SendNow);
					}
				}
			}
		}

		// Token: 0x0600163E RID: 5694 RVA: 0x0005DCAB File Offset: 0x0005C0AB
		public override string ToString()
		{
			return string.Format("<tutorial_passed type={0} silent={1} special_reward={2}>", this.m_tutorial, this.Silent, this.Event);
		}

		// Token: 0x04000ACD RID: 2765
		private readonly ITutorialStatsTracker m_statsTracker;

		// Token: 0x04000ACE RID: 2766
		private readonly INotificationService m_notificationService;

		// Token: 0x04000ACF RID: 2767
		private ProfileProgressionInfo.Tutorial m_tutorial;
	}
}
