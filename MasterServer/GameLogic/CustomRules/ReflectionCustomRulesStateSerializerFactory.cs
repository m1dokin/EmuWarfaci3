using System;
using System.Collections.Generic;
using System.Reflection;
using HK2Net;
using Util.Common;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002AD RID: 685
	[Service]
	[Singleton]
	internal class ReflectionCustomRulesStateSerializerFactory : ICustomRuleStateSerializerFactory
	{
		// Token: 0x06000EC0 RID: 3776 RVA: 0x0003B4DC File Offset: 0x000398DC
		public ReflectionCustomRulesStateSerializerFactory()
		{
			this.m_stateTypeToInfo = new Dictionary<Type, CustomRuleStateInfo>();
			this.m_stateTypeToSerializer = new Dictionary<Type, ICustomRuleStateSerializer>();
			foreach (KeyValuePair<CustomRuleStateSerializerAttribute, Type> keyValuePair in ReflectionUtils.GetTypesByAttribute<CustomRuleStateSerializerAttribute>(Assembly.GetExecutingAssembly()))
			{
				ICustomRuleStateSerializer value = (ICustomRuleStateSerializer)Activator.CreateInstance(keyValuePair.Value);
				this.m_stateTypeToInfo.Add(keyValuePair.Key.StateInfo.StateType, keyValuePair.Key.StateInfo);
				this.m_stateTypeToSerializer.Add(keyValuePair.Key.StateInfo.StateType, value);
			}
		}

		// Token: 0x06000EC1 RID: 3777 RVA: 0x0003B5A8 File Offset: 0x000399A8
		public CustomRuleStateInfo GetInfo(Type stateType)
		{
			CustomRuleStateInfo result;
			if (!this.m_stateTypeToInfo.TryGetValue(stateType, out result))
			{
				throw new KeyNotFoundException(string.Format("No state type info found for '{0}'", stateType));
			}
			return result;
		}

		// Token: 0x06000EC2 RID: 3778 RVA: 0x0003B5DC File Offset: 0x000399DC
		public ICustomRuleStateSerializer GetSerializer(Type stateType)
		{
			ICustomRuleStateSerializer result;
			if (!this.m_stateTypeToSerializer.TryGetValue(stateType, out result))
			{
				throw new KeyNotFoundException(string.Format("No state serializer found for '{0}'", stateType));
			}
			return result;
		}

		// Token: 0x040006C3 RID: 1731
		private readonly Dictionary<Type, CustomRuleStateInfo> m_stateTypeToInfo;

		// Token: 0x040006C4 RID: 1732
		private readonly Dictionary<Type, ICustomRuleStateSerializer> m_stateTypeToSerializer;
	}
}
