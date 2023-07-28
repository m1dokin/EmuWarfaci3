using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.GameLogic.CustomRules.Rules.RatingSeason.Exceptions;

namespace MasterServer.GameLogic.CustomRules.Rules.RatingSeason
{
	// Token: 0x020000AA RID: 170
	[Service]
	public class RatingSeasonRuleValidator : ICustomRulesValidator
	{
		// Token: 0x060002B5 RID: 693 RVA: 0x0000DBF8 File Offset: 0x0000BFF8
		public void Validate(IEnumerable<ICustomRule> rules)
		{
			List<RatingSeasonRule> rules2 = rules.OfType<RatingSeasonRule>().ToList<RatingSeasonRule>();
			this.RatingSeasonRuleShouldPresent(rules2);
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x0000DC18 File Offset: 0x0000C018
		private void RatingSeasonRuleShouldPresent(IEnumerable<ICustomRule> rules)
		{
			if (!rules.Any<ICustomRule>())
			{
				string message = string.Format("{0} custom rule is mandatory", "rating_season_rule");
				throw new RatingSeasonRuleValidationException(message);
			}
		}
	}
}
