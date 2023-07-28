using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterServer.Core;
using NCrontab;

namespace MasterServer.GameLogic.CustomRules.Rules.ConsecutiveLoginBonus
{
	// Token: 0x020002BD RID: 701
	[ConsoleCmdAttributes(CmdName = "clb_dump_state", ArgsSize = 1, Help = "Dump consecutive login bonus state for specified profile (args:profile_id)")]
	internal class ClbGetActivationInfoCommand : IConsoleCmd
	{
		// Token: 0x06000F14 RID: 3860 RVA: 0x0003C636 File Offset: 0x0003AA36
		public ClbGetActivationInfoCommand(ICustomRulesStateStorage customRulesStateStorage, ICustomRulesService customRulesService)
		{
			this.m_customRulesStateStorage = customRulesStateStorage;
			this.m_customRulesService = customRulesService;
		}

		// Token: 0x06000F15 RID: 3861 RVA: 0x0003C64C File Offset: 0x0003AA4C
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			StringBuilder stringBuilder = new StringBuilder();
			IEnumerable<ConsecutiveLoginBonusRule> enumerable = this.m_customRulesService.GetActiveRules().Concat(this.m_customRulesService.GetDisabledRules()).OfType<ConsecutiveLoginBonusRule>();
			foreach (ConsecutiveLoginBonusRule consecutiveLoginBonusRule in enumerable)
			{
				CrontabSchedule crontabSchedule = CrontabSchedule.Parse(consecutiveLoginBonusRule.Config.GetAttribute("schedule"));
				TimeSpan t = TimeSpan.Parse(consecutiveLoginBonusRule.Config.GetAttribute("expiration"));
				stringBuilder.AppendLine(string.Format("profile_id: {0}, rule_id: {1}", num, consecutiveLoginBonusRule.RuleID));
				ConsecutiveLoginBonusRuleState consecutiveLoginBonusRuleState = (ConsecutiveLoginBonusRuleState)this.m_customRulesStateStorage.GetState(num, consecutiveLoginBonusRule);
				if (consecutiveLoginBonusRuleState == null || consecutiveLoginBonusRuleState.LastActivationTime == DateTime.MinValue)
				{
					stringBuilder.AppendLine("State is empty");
				}
				else
				{
					DateTime dateTime = consecutiveLoginBonusRuleState.LastActivationTime.ToLocalTime();
					DateTime nextOccurrence = crontabSchedule.GetNextOccurrence(dateTime);
					DateTime dateTime2 = nextOccurrence + t;
					stringBuilder.AppendLine(string.Format("streak: {0}", consecutiveLoginBonusRuleState.PrevStreak));
					stringBuilder.AppendLine(string.Format("reward: {0}", consecutiveLoginBonusRuleState.PrevReward));
					stringBuilder.AppendLine(string.Format("last_activation_time: {0}", dateTime));
					stringBuilder.AppendLine(string.Format("next_slot_start_time: {0}", nextOccurrence));
					stringBuilder.AppendLine(string.Format("next_slot_end_time: {0}", dateTime2));
				}
			}
			Log.Info(stringBuilder.ToString());
		}

		// Token: 0x040006F1 RID: 1777
		private readonly ICustomRulesStateStorage m_customRulesStateStorage;

		// Token: 0x040006F2 RID: 1778
		private readonly ICustomRulesService m_customRulesService;
	}
}
