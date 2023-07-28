using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Core;

namespace MasterServer.GameLogic.CustomRules.Rules.ConsecutiveLoginBonus
{
	// Token: 0x020002BC RID: 700
	[ConsoleCmdAttributes(CmdName = "clb_update_state", ArgsSize = 5, Help = "Update state of rule (args:profile_id, rule_id, last_activation_time, prev_streak_id, prev_reward_id)")]
	internal class ClbUpdateStateCommand : IConsoleCmd
	{
		// Token: 0x06000F12 RID: 3858 RVA: 0x0003C503 File Offset: 0x0003A903
		public ClbUpdateStateCommand(ICustomRulesStateStorage customRulesStateStorage, ICustomRulesService customRulesService)
		{
			this.m_customRulesStateStorage = customRulesStateStorage;
			this.m_customRulesService = customRulesService;
		}

		// Token: 0x06000F13 RID: 3859 RVA: 0x0003C51C File Offset: 0x0003A91C
		public void ExecuteCmd(string[] args)
		{
			ulong profileID = ulong.Parse(args[1]);
			ulong ruleId = ulong.Parse(args[2]);
			DateTime lastActivationTime = DateTime.Parse(args[3]);
			int prevStreak = int.Parse(args[4]);
			int prevReward = int.Parse(args[5]);
			IEnumerable<ConsecutiveLoginBonusRule> source = this.m_customRulesService.GetActiveRules().Concat(this.m_customRulesService.GetDisabledRules()).OfType<ConsecutiveLoginBonusRule>();
			ConsecutiveLoginBonusRule consecutiveLoginBonusRule = source.FirstOrDefault((ConsecutiveLoginBonusRule x) => x.RuleID == ruleId);
			if (consecutiveLoginBonusRule == null)
			{
				Log.Info("State update failed");
				return;
			}
			this.m_customRulesStateStorage.UpdateState(profileID, consecutiveLoginBonusRule, delegate(CustomRuleState st)
			{
				ConsecutiveLoginBonusRuleState consecutiveLoginBonusRuleState = (ConsecutiveLoginBonusRuleState)st;
				consecutiveLoginBonusRuleState.LastActivationTime = lastActivationTime.ToUniversalTime();
				consecutiveLoginBonusRuleState.PrevStreak = prevStreak;
				consecutiveLoginBonusRuleState.PrevReward = prevReward;
				return true;
			});
			Log.Info<ulong>("State for rule {0} update was successful", consecutiveLoginBonusRule.RuleID);
		}

		// Token: 0x040006EF RID: 1775
		private readonly ICustomRulesStateStorage m_customRulesStateStorage;

		// Token: 0x040006F0 RID: 1776
		private readonly ICustomRulesService m_customRulesService;
	}
}
