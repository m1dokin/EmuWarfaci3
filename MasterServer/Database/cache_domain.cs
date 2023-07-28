using System;
using System.Collections.Generic;

namespace MasterServer.Database
{
	// Token: 0x020001E1 RID: 481
	public class cache_domain
	{
		// Token: 0x06000946 RID: 2374 RVA: 0x00022D58 File Offset: 0x00021158
		public cache_domain(string name)
		{
			this._name = name;
		}

		// Token: 0x06000947 RID: 2375 RVA: 0x00022D67 File Offset: 0x00021167
		public override string ToString()
		{
			return this._name;
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x00022D70 File Offset: 0x00021170
		public virtual IEnumerable<cache_domain> Terminals()
		{
			yield return this;
			yield break;
		}

		// Token: 0x04000549 RID: 1353
		public string _name;
	}
}
