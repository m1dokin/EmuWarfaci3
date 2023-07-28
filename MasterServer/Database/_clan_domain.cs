using System;
using System.Collections.Generic;

namespace MasterServer.Database
{
	// Token: 0x020001E6 RID: 486
	public class _clan_domain : cache_domain
	{
		// Token: 0x0600096E RID: 2414 RVA: 0x00023AAC File Offset: 0x00021EAC
		public _clan_domain() : base("clan")
		{
		}

		// Token: 0x0600096F RID: 2415 RVA: 0x00023AB9 File Offset: 0x00021EB9
		public _clan_domain(ulong id) : base("clan_" + id.ToString())
		{
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x00023AD8 File Offset: 0x00021ED8
		public _clan_domain(string name) : base("clan_" + name)
		{
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000971 RID: 2417 RVA: 0x00023AEB File Offset: 0x00021EEB
		public cache_domain members
		{
			get
			{
				return new cache_domain(this._name + ".members");
			}
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06000972 RID: 2418 RVA: 0x00023B02 File Offset: 0x00021F02
		public cache_domain clan_info
		{
			get
			{
				return new cache_domain(this._name + ".clan_info");
			}
		}

		// Token: 0x06000973 RID: 2419 RVA: 0x00023B1C File Offset: 0x00021F1C
		public override IEnumerable<cache_domain> Terminals()
		{
			foreach (cache_domain terminal in base.Terminals())
			{
				yield return terminal;
			}
			yield return this.members;
			yield return this.clan_info;
			yield break;
		}
	}
}
