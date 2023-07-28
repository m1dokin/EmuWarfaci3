using System;

namespace MasterServer.GFaceAPI.Responses
{
	// Token: 0x02000659 RID: 1625
	public class LoginResponseDTO : gFaceEntityDTO
	{
		// Token: 0x0400112C RID: 4396
		public string token;

		// Token: 0x0400112D RID: 4397
		[GRspOptional]
		public string remembermetoken;

		// Token: 0x0400112E RID: 4398
		[GRspOptional]
		public long? userid;
	}
}
