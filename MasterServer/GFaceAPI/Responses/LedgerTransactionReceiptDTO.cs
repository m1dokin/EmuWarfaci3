using System;

namespace MasterServer.GFaceAPI.Responses
{
	// Token: 0x02000667 RID: 1639
	public class LedgerTransactionReceiptDTO
	{
		// Token: 0x04001158 RID: 4440
		[GRspOptional]
		public string transactionid;

		// Token: 0x04001159 RID: 4441
		[GRspOptional]
		public _Wallets wallets;
	}
}
