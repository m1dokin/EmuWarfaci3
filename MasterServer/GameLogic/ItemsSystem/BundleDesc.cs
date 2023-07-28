using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200036A RID: 874
	public class BundleDesc : IItemsContainer
	{
		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06001382 RID: 4994 RVA: 0x0004FA35 File Offset: 0x0004DE35
		// (set) Token: 0x06001383 RID: 4995 RVA: 0x0004FA3D File Offset: 0x0004DE3D
		public string Name { get; set; }

		// Token: 0x06001384 RID: 4996 RVA: 0x0004FA48 File Offset: 0x0004DE48
		public bool HasItemNamed(string name)
		{
			return this.Items.Any((BundleDesc.BundledItem item) => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x0400091F RID: 2335
		public readonly List<BundleDesc.BundledItem> Items = new List<BundleDesc.BundledItem>();

		// Token: 0x0200036B RID: 875
		public class BundledItem : GenericItemBase
		{
			// Token: 0x06001385 RID: 4997 RVA: 0x0004FA79 File Offset: 0x0004DE79
			public BundledItem(IDictionary<string, string> @params) : base(@params)
			{
			}
		}
	}
}
