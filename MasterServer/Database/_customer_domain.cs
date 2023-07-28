using System;
using System.Collections.Generic;

namespace MasterServer.Database
{
	// Token: 0x020001E5 RID: 485
	public class _customer_domain : cache_domain
	{
		// Token: 0x06000969 RID: 2409 RVA: 0x00023846 File Offset: 0x00021C46
		public _customer_domain(ulong id) : base("customer_" + id)
		{
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x0600096A RID: 2410 RVA: 0x0002385E File Offset: 0x00021C5E
		public cache_domain items
		{
			get
			{
				return new cache_domain(this._name + ".items");
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x0600096B RID: 2411 RVA: 0x00023875 File Offset: 0x00021C75
		public cache_domain accounts
		{
			get
			{
				return new cache_domain(this._name + ".accounts");
			}
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x0002388C File Offset: 0x00021C8C
		public override IEnumerable<cache_domain> Terminals()
		{
			foreach (cache_domain terminal in base.Terminals())
			{
				yield return terminal;
			}
			yield return this.items;
			yield return this.accounts;
			yield break;
		}
	}
}
