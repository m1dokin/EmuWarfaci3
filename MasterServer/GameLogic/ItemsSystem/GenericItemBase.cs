using System;
using System.Collections.Generic;
using Util.Common;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000361 RID: 865
	public class GenericItemBase : IGenericItem
	{
		// Token: 0x0600135E RID: 4958 RVA: 0x0004F530 File Offset: 0x0004D930
		protected GenericItemBase(IDictionary<string, string> @params)
		{
			this.Name = @params["name"];
			string text;
			if (@params.TryGetValue("amount", out text))
			{
				this.Amount = new int?(int.Parse(text));
			}
			if (@params.TryGetValue("durability", out text))
			{
				this.m_durability = new int?(int.Parse(text));
			}
			if (@params.TryGetValue("expiration", out text))
			{
				this.m_expiration = new TimeSpan?(TimeUtils.GetOfferExpireTime(text));
			}
			if (@params.ContainsKey("regular"))
			{
				this.m_regular = true;
			}
		}

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x0600135F RID: 4959 RVA: 0x0004F5D4 File Offset: 0x0004D9D4
		// (set) Token: 0x06001360 RID: 4960 RVA: 0x0004F5DC File Offset: 0x0004D9DC
		public string Name { get; private set; }

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06001361 RID: 4961 RVA: 0x0004F5E5 File Offset: 0x0004D9E5
		// (set) Token: 0x06001362 RID: 4962 RVA: 0x0004F5ED File Offset: 0x0004D9ED
		public int? Amount { get; private set; }

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06001363 RID: 4963 RVA: 0x0004F5F6 File Offset: 0x0004D9F6
		public bool IsExpirable
		{
			get
			{
				return this.m_expiration != null;
			}
		}

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06001364 RID: 4964 RVA: 0x0004F603 File Offset: 0x0004DA03
		public bool IsRegular
		{
			get
			{
				return this.m_regular;
			}
		}

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06001365 RID: 4965 RVA: 0x0004F60B File Offset: 0x0004DA0B
		public TimeSpan Expiration
		{
			get
			{
				return this.m_expiration.Value;
			}
		}

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06001366 RID: 4966 RVA: 0x0004F618 File Offset: 0x0004DA18
		public int Durability
		{
			get
			{
				return this.m_durability.Value;
			}
		}

		// Token: 0x040008FC RID: 2300
		public const string NameParam = "name";

		// Token: 0x040008FD RID: 2301
		public const string AmountParam = "amount";

		// Token: 0x040008FE RID: 2302
		public const string DurabilityParam = "durability";

		// Token: 0x040008FF RID: 2303
		public const string ExpirationParam = "expiration";

		// Token: 0x04000900 RID: 2304
		public const string RegularParam = "regular";

		// Token: 0x04000901 RID: 2305
		private int? m_durability;

		// Token: 0x04000902 RID: 2306
		private TimeSpan? m_expiration;

		// Token: 0x04000903 RID: 2307
		private readonly bool m_regular;
	}
}
