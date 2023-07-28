using System;
using System.Collections.Generic;

namespace MasterServer.DAL
{
	// Token: 0x02000059 RID: 89
	[Serializable]
	public struct MoneyUpdateResultMulti
	{
		// Token: 0x040000ED RID: 237
		public TransactionStatus Status;

		// Token: 0x040000EE RID: 238
		public List<KeyValuePair<Currency, ulong>> Money;
	}
}
