using System;

namespace MasterServer.DAL
{
	// Token: 0x02000061 RID: 97
	[Serializable]
	public struct PriceTag
	{
		// Token: 0x060000E0 RID: 224 RVA: 0x0000404D File Offset: 0x0000244D
		public override int GetHashCode()
		{
			if (this.Currency == Currency.KeyMoney)
			{
				return ((int)this.Price ^ this.KeyCatalogName.GetHashCode()) << (int)this.Currency;
			}
			return (int)this.Price << (int)this.Currency;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000408A File Offset: 0x0000248A
		public override string ToString()
		{
			return string.Format("<PriceTag> {0} {1} {2}", this.KeyCatalogName, this.Price, this.Currency);
		}

		// Token: 0x040000FB RID: 251
		public Currency Currency;

		// Token: 0x040000FC RID: 252
		public string KeyCatalogName;

		// Token: 0x040000FD RID: 253
		public ulong Price;
	}
}
