using System;
using System.Collections.Generic;
using System.Reflection;
using HK2Net;
using HK2Net.Kernel;
using MasterServer.Core.Configuration;
using Util.Common;

namespace MasterServer.GameLogic.SpecialProfileRewards
{
	// Token: 0x020005CB RID: 1483
	[Service]
	[Singleton]
	internal class ReflectionSpecialRewardActionFactory : ISpecialRewardActionFactory
	{
		// Token: 0x06001FB8 RID: 8120 RVA: 0x0008164C File Offset: 0x0007FA4C
		public ReflectionSpecialRewardActionFactory(IContainer container)
		{
			this.m_container = container;
			this.m_actionTypes = new Dictionary<string, Type>();
			foreach (KeyValuePair<SpecialRewardActionAttribute, Type> keyValuePair in ReflectionUtils.GetTypesByAttribute<SpecialRewardActionAttribute>(Assembly.GetExecutingAssembly()))
			{
				this.m_actionTypes.Add(keyValuePair.Key.Name, keyValuePair.Value);
			}
		}

		// Token: 0x06001FB9 RID: 8121 RVA: 0x000816D8 File Offset: 0x0007FAD8
		public Dictionary<string, Type> GetActionTypes()
		{
			return this.m_actionTypes;
		}

		// Token: 0x06001FBA RID: 8122 RVA: 0x000816E0 File Offset: 0x0007FAE0
		public ISpecialRewardAction CreateAction(string name, ConfigSection config, bool enableNotifs)
		{
			Type type = this.m_actionTypes[name];
			return (ISpecialRewardAction)this.m_container.CreateWithParams(type, new Dictionary<string, object>
			{
				{
					"config",
					config
				},
				{
					"useNotification",
					enableNotifs
				}
			});
		}

		// Token: 0x04000F7B RID: 3963
		private readonly IContainer m_container;

		// Token: 0x04000F7C RID: 3964
		private readonly Dictionary<string, Type> m_actionTypes;
	}
}
