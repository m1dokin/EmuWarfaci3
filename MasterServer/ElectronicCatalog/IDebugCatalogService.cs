using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000244 RID: 580
	[Contract]
	internal interface IDebugCatalogService
	{
		// Token: 0x06000C83 RID: 3203
		void DebugResetCustomerItems(ulong customerId);

		// Token: 0x06000C84 RID: 3204
		void DebugLogsBackup();

		// Token: 0x06000C85 RID: 3205
		void DebugDeleteAllItems(ulong customerId);

		// Token: 0x06000C86 RID: 3206
		void DebugGenEcatRecords(uint count, uint dayInterval);

		// Token: 0x06000C87 RID: 3207
		IEnumerable<EcatLogHistory> DebugGetLogHistory(ulong customerId);

		// Token: 0x06000C88 RID: 3208
		void ClearGiveMoneyTransactionHistory(ulong customerId);
	}
}
