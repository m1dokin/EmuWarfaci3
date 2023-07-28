using System;
using System.Collections.Generic;

namespace MasterServer.Database
{
	// Token: 0x020001E2 RID: 482
	public class _user_domain : cache_domain
	{
		// Token: 0x06000949 RID: 2377 RVA: 0x00022E58 File Offset: 0x00021258
		public _user_domain(ulong id) : base("user_" + id.ToString())
		{
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x0600094A RID: 2378 RVA: 0x00022E77 File Offset: 0x00021277
		public cache_domain profiles
		{
			get
			{
				return new cache_domain(this._name + ".profiles");
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x0600094B RID: 2379 RVA: 0x00022E8E File Offset: 0x0002128E
		public cache_domain all_voucher
		{
			get
			{
				return new cache_domain(this._name + ".all_voucher");
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x0600094C RID: 2380 RVA: 0x00022EA5 File Offset: 0x000212A5
		public cache_domain voucher
		{
			get
			{
				return new cache_domain(this._name + ".voucher");
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x0600094D RID: 2381 RVA: 0x00022EBC File Offset: 0x000212BC
		public cache_domain token
		{
			get
			{
				return new cache_domain(this._name + ".token");
			}
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x00022ED4 File Offset: 0x000212D4
		public override IEnumerable<cache_domain> Terminals()
		{
			foreach (cache_domain terminal in base.Terminals())
			{
				yield return terminal;
			}
			yield return this.profiles;
			yield return this.all_voucher;
			yield return this.voucher;
			yield return this.token;
			yield break;
		}
	}
}
