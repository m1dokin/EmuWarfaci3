using System;
using System.Xml;
using HK2Net;
using MasterServer.DAL.CustomRules;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002AB RID: 683
	[Contract]
	public interface ICustomRulesFactory
	{
		// Token: 0x06000E97 RID: 3735
		ICustomRule CreateRule(CustomRuleInfo ruleInfo);

		// Token: 0x06000E98 RID: 3736
		ICustomRule CreateRule(string name, XmlElement config);
	}
}
