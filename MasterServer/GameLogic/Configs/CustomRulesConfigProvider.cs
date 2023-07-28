using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using HK2Net;
using MasterServer.Common;
using MasterServer.GameLogic.CustomRules;
using MasterServer.GameLogic.CustomRules.Rules;
using MasterServer.GameLogic.CustomRules.Rules.RatingSeason;

namespace MasterServer.GameLogic.Configs
{
	// Token: 0x02000286 RID: 646
	[Service]
	[Singleton]
	internal class CustomRulesConfigProvider : IConfigProvider
	{
		// Token: 0x06000E0B RID: 3595 RVA: 0x000389D8 File Offset: 0x00036DD8
		public CustomRulesConfigProvider(ICustomRulesService customRulesService)
		{
			this.m_customRulesService = customRulesService;
		}

		// Token: 0x06000E0C RID: 3596 RVA: 0x00038A0D File Offset: 0x00036E0D
		public int GetHash()
		{
			return this.m_customRulesService.GetActiveRules().OfType(this.m_rules).Aggregate(0, (int current, ICustomRule rule) => current ^ rule.Config.OuterXml.GetHashCode());
		}

		// Token: 0x06000E0D RID: 3597 RVA: 0x00038A48 File Offset: 0x00036E48
		public IEnumerable<XmlElement> GetConfig(XmlDocument doc)
		{
			return (from customRule in this.m_customRulesService.GetActiveRules().OfType(this.m_rules)
			select doc.ImportNode(customRule.Config, true) as XmlElement).ToList<XmlElement>();
		}

		// Token: 0x04000671 RID: 1649
		private readonly ICustomRulesService m_customRulesService;

		// Token: 0x04000672 RID: 1650
		private readonly Type[] m_rules = new Type[]
		{
			typeof(ConsecutiveLoginBonusRule),
			typeof(RatingSeasonRule)
		};
	}
}
