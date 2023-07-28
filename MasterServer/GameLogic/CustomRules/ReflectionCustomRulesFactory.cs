using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using HK2Net;
using HK2Net.Kernel;
using MasterServer.DAL.CustomRules;
using Util.Common;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002A4 RID: 676
	[Service]
	[Singleton]
	internal class ReflectionCustomRulesFactory : ICustomRulesFactory
	{
		// Token: 0x06000E7B RID: 3707 RVA: 0x0003A5E0 File Offset: 0x000389E0
		public ReflectionCustomRulesFactory(IContainer container)
		{
			this.m_container = container;
			this.m_ruleTypes = new Dictionary<string, Type>();
			foreach (KeyValuePair<CustomRuleAttribute, Type> keyValuePair in ReflectionUtils.GetTypesByAttribute<CustomRuleAttribute>(Assembly.GetExecutingAssembly()))
			{
				this.m_ruleTypes.Add(keyValuePair.Key.Name, keyValuePair.Value);
			}
		}

		// Token: 0x06000E7C RID: 3708 RVA: 0x0003A66C File Offset: 0x00038A6C
		public ICustomRule CreateRule(CustomRuleInfo ruleInfo)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(ruleInfo.Data);
			XmlElement xmlElement = (XmlElement)xmlDocument.FirstChild;
			return this.CreateRule(xmlElement.Name, xmlElement);
		}

		// Token: 0x06000E7D RID: 3709 RVA: 0x0003A6A4 File Offset: 0x00038AA4
		public ICustomRule CreateRule(string name, XmlElement config)
		{
			Type type;
			if (!this.m_ruleTypes.TryGetValue(name, out type))
			{
				throw new KeyNotFoundException(string.Format("Custom rule {0} doesn't implemented", name));
			}
			return (ICustomRule)this.m_container.CreateWithParams(type, new Dictionary<string, object>
			{
				{
					"config",
					config
				}
			});
		}

		// Token: 0x040006AF RID: 1711
		private readonly IContainer m_container;

		// Token: 0x040006B0 RID: 1712
		private readonly Dictionary<string, Type> m_ruleTypes;
	}
}
