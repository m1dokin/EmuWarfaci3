using System;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200035B RID: 859
	public struct ItemToReimburse
	{
		// Token: 0x06001347 RID: 4935 RVA: 0x0004EE0F File Offset: 0x0004D20F
		public ItemToReimburse(string _name, string _message, Currency _currency, ulong _moneyAmount)
		{
			this.name = _name;
			this.message = _message;
			this.currency = _currency;
			this.moneyAmount = _moneyAmount;
		}

		// Token: 0x040008EB RID: 2283
		public string name;

		// Token: 0x040008EC RID: 2284
		public string message;

		// Token: 0x040008ED RID: 2285
		public Currency currency;

		// Token: 0x040008EE RID: 2286
		public ulong moneyAmount;
	}
}
