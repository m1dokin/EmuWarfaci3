using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.GameLogic.CustomRules;

namespace MasterServer.Core.ServerDataLog
{
	// Token: 0x0200012B RID: 299
	[Service]
	[Singleton]
	internal class CustomRulesDataLogExtensoin : AbstractServerDataLogExtension
	{
		// Token: 0x060004E8 RID: 1256 RVA: 0x0001513C File Offset: 0x0001353C
		public CustomRulesDataLogExtensoin(ICustomRulesService customRulesService, ILogService logService, bool isEnabled) : base(logService, isEnabled)
		{
			this.m_customRulesService = customRulesService;
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x0001514D File Offset: 0x0001354D
		public override void Start()
		{
			base.Start();
			this.m_customRulesService.RuleSetUpdated += this.RuleSetUpdated;
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x0001516C File Offset: 0x0001356C
		public override void Dispose()
		{
			this.m_customRulesService.RuleSetUpdated -= this.RuleSetUpdated;
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x00015185 File Offset: 0x00013585
		private void RuleSetUpdated(IEnumerable<ICustomRule> active, IEnumerable<ICustomRule> disabled)
		{
			base.OnDataUpdated();
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x00015190 File Offset: 0x00013590
		protected override void LogData()
		{
			using (ILogGroup logGroup = this.LogService.CreateGroup())
			{
				foreach (ICustomRule customRule in this.m_customRulesService.GetActiveRules())
				{
					logGroup.CustomRuleLog(customRule.RuleID, true, customRule.ToString());
				}
				foreach (ICustomRule customRule2 in this.m_customRulesService.GetDisabledRules())
				{
					logGroup.CustomRuleLog(customRule2.RuleID, false, customRule2.ToString());
				}
			}
		}

		// Token: 0x0400020C RID: 524
		private readonly ICustomRulesService m_customRulesService;
	}
}
