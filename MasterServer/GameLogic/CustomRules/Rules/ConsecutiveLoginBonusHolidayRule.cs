using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.GameLogic.ItemsSystem.RandomBoxChoiceLimitation;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.SpecialProfileRewards;
using MasterServer.Users;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002BA RID: 698
	[CustomRule("consecutive_login_bonus_holiday")]
	internal class ConsecutiveLoginBonusHolidayRule : ConsecutiveLoginBonusRule
	{
		// Token: 0x06000EFF RID: 3839 RVA: 0x0003C4D4 File Offset: 0x0003A8D4
		public ConsecutiveLoginBonusHolidayRule(XmlElement config, ILogService logService, IUserRepository userRepository, ICustomRulesStateStorage stateStorage, ISpecialProfileRewardService specialRewards, INotificationService notificationService, ITagService tagService, IRandomBoxChoiceLimitationService randomBoxChoiceLimitation) : base(config, logService, userRepository, stateStorage, specialRewards, notificationService, tagService, randomBoxChoiceLimitation)
		{
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x06000F00 RID: 3840 RVA: 0x0003C4F4 File Offset: 0x0003A8F4
		protected override string RuleName
		{
			get
			{
				return "consecutive_login_bonus_holiday";
			}
		}

		// Token: 0x06000F01 RID: 3841 RVA: 0x0003C4FB File Offset: 0x0003A8FB
		public override bool IsActive()
		{
			return base.Enabled;
		}

		// Token: 0x040006E5 RID: 1765
		private new const string RULE_NAME = "consecutive_login_bonus_holiday";
	}
}
