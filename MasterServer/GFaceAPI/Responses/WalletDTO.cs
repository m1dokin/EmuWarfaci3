using System;

namespace MasterServer.GFaceAPI.Responses
{
	// Token: 0x02000665 RID: 1637
	public class WalletDTO
	{
		// Token: 0x04001154 RID: 4436
		[GRspOptional]
		public long userid;

		// Token: 0x04001155 RID: 4437
		[GRspOptional]
		public _CreditBalances? creditbalances;

		// Token: 0x04001156 RID: 4438
		[GRspOptional]
		public _PointsBalances? pointbalances;
	}
}
