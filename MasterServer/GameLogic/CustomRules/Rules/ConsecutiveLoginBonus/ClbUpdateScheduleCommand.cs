using System;
using System.Linq;
using MasterServer.Core;

namespace MasterServer.GameLogic.CustomRules.Rules.ConsecutiveLoginBonus
{
	// Token: 0x020002BE RID: 702
	[ConsoleCmdAttributes(CmdName = "clb_update_schedule", ArgsSize = 3, Help = "Set schedule for clb rule (args:ruleId, schedule, expiration)")]
	internal class ClbUpdateScheduleCommand : IConsoleCmd
	{
		// Token: 0x06000F16 RID: 3862 RVA: 0x0003C824 File Offset: 0x0003AC24
		public ClbUpdateScheduleCommand(ICustomRulesService customRulesService, IDebugCustomRulesService debugCustomRulesService)
		{
			this.m_customRulesService = customRulesService;
			this.m_debugCustomRulesService = debugCustomRulesService;
		}

		// Token: 0x06000F17 RID: 3863 RVA: 0x0003C83C File Offset: 0x0003AC3C
		public void ExecuteCmd(string[] args)
		{
			ulong ruleId = ulong.Parse(args[1]);
			string text = args[2];
			string value = args[3];
			ICustomRule customRule = this.m_customRulesService.GetActiveRules().Concat(this.m_customRulesService.GetDisabledRules()).FirstOrDefault((ICustomRule x) => x.RuleID == ruleId);
			if (customRule == null)
			{
				Log.Info<ulong>("Can't find rule with id {0}", ruleId);
				return;
			}
			customRule.Config.SetAttribute("schedule", text);
			customRule.Config.SetAttribute("expiration", value);
			this.m_debugCustomRulesService.RemoveStaticRule(ruleId);
			this.m_debugCustomRulesService.AddStaticRule(customRule.Config.OuterXml, customRule.Enabled);
			Log.Info<ulong, string>("Schedule updated for rule {0} to {1}", ruleId, text);
		}

		// Token: 0x040006F3 RID: 1779
		private readonly ICustomRulesService m_customRulesService;

		// Token: 0x040006F4 RID: 1780
		private readonly IDebugCustomRulesService m_debugCustomRulesService;
	}
}
