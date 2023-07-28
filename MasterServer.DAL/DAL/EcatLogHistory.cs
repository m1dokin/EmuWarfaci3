using System;

namespace MasterServer.DAL
{
	// Token: 0x0200001E RID: 30
	[Serializable]
	public struct EcatLogHistory
	{
		// Token: 0x0600004D RID: 77 RVA: 0x00002DE8 File Offset: 0x000011E8
		public override string ToString()
		{
			return string.Format("ID: {0}, CustomerID: {1}, RealmID: {2}, CatalogID: {3}, Currency: {4}, Price: {5}, Time: {6}, Action: {7}", new object[]
			{
				this.id,
				this.customer_id,
				this.realm_id,
				this.catalog_id,
				this.currency,
				this.price,
				this.time,
				this.action
			});
		}

		// Token: 0x04000049 RID: 73
		public ulong id;

		// Token: 0x0400004A RID: 74
		public ulong customer_id;

		// Token: 0x0400004B RID: 75
		public int realm_id;

		// Token: 0x0400004C RID: 76
		public ulong catalog_id;

		// Token: 0x0400004D RID: 77
		public int currency;

		// Token: 0x0400004E RID: 78
		public ulong price;

		// Token: 0x0400004F RID: 79
		public DateTime time;

		// Token: 0x04000050 RID: 80
		public int action;
	}
}
