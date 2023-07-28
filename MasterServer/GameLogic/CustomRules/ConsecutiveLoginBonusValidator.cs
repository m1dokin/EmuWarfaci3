using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.GameLogic.CustomRules.Rules;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x0200029F RID: 671
	[Service]
	internal class ConsecutiveLoginBonusValidator : ICustomRulesValidator
	{
		// Token: 0x06000E70 RID: 3696 RVA: 0x0003A3FC File Offset: 0x000387FC
		public void Validate(IEnumerable<ICustomRule> rules)
		{
			IEnumerable<ConsecutiveLoginBonusRule> enumerable = rules.OfType<ConsecutiveLoginBonusRule>();
			if (enumerable.Count((ConsecutiveLoginBonusRule x) => x.Enabled) > 1)
			{
				throw new ConsecutiveLoginBonusValidationException(string.Format("More than 1 consecutive login rule is enabled, {0}", string.Join<ConsecutiveLoginBonusRule>(Environment.NewLine, enumerable)));
			}
		}
	}
}
