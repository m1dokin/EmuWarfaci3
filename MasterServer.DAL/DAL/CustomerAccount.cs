using System;

namespace MasterServer.DAL
{
	// Token: 0x02000056 RID: 86
	[Serializable]
	public struct CustomerAccount
	{
		// Token: 0x060000B4 RID: 180 RVA: 0x00003DA9 File Offset: 0x000021A9
		public override string ToString()
		{
			return string.Format("<CustomerAccount> {0}: {1} {2}", this.CustomerId, this.Money, this.Currency);
		}

		// Token: 0x040000DC RID: 220
		public ulong CustomerId;

		// Token: 0x040000DD RID: 221
		public Currency Currency;

		// Token: 0x040000DE RID: 222
		public ulong Money;
	}
}
