using System;
using System.Collections.Generic;
using System.Reflection;
using HK2Net;
using HK2Net.Kernel;
using Util.Common;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000379 RID: 889
	[Service]
	[Singleton]
	internal class ReflectionProfileItemsComposerFactory : IProfileItemsComposerFactory
	{
		// Token: 0x0600142C RID: 5164 RVA: 0x0005201E File Offset: 0x0005041E
		public ReflectionProfileItemsComposerFactory(IContainer container)
		{
			this.m_container = container;
		}

		// Token: 0x0600142D RID: 5165 RVA: 0x00052030 File Offset: 0x00050430
		public IEnumerable<KeyValuePair<int, IProfileItemsComposer>> GetComposers()
		{
			List<KeyValuePair<int, IProfileItemsComposer>> list = new List<KeyValuePair<int, IProfileItemsComposer>>();
			IEnumerable<KeyValuePair<ProfileItemsComposerAttribute, Type>> typesByAttribute = ReflectionUtils.GetTypesByAttribute<ProfileItemsComposerAttribute>(Assembly.GetExecutingAssembly());
			foreach (KeyValuePair<ProfileItemsComposerAttribute, Type> keyValuePair in typesByAttribute)
			{
				list.Add(new KeyValuePair<int, IProfileItemsComposer>(keyValuePair.Key.Priority, (IProfileItemsComposer)this.m_container.Create(keyValuePair.Value)));
			}
			list.Sort((KeyValuePair<int, IProfileItemsComposer> a, KeyValuePair<int, IProfileItemsComposer> b) => Comparer<int>.Default.Compare(a.Key, b.Key));
			return list;
		}

		// Token: 0x04000954 RID: 2388
		private readonly IContainer m_container;
	}
}
