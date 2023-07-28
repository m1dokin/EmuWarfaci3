using System;

namespace MasterServer.GFaceAPI.Responses
{
	// Token: 0x0200065C RID: 1628
	public class UserProfileDTO
	{
		// Token: 0x04001131 RID: 4401
		public string nickname;

		// Token: 0x04001132 RID: 4402
		[GRspOptional]
		public IdDTO profilepic;

		// Token: 0x04001133 RID: 4403
		[GRspOptional]
		public bool? online;

		// Token: 0x04001134 RID: 4404
		[GRspOptional]
		public bool? isconnected;

		// Token: 0x04001135 RID: 4405
		[GRspOptional]
		public string firstname;

		// Token: 0x04001136 RID: 4406
		[GRspOptional]
		public string lastname;

		// Token: 0x04001137 RID: 4407
		[GRspOptional]
		public string gender;

		// Token: 0x04001138 RID: 4408
		[GRspOptional]
		public string dob;

		// Token: 0x04001139 RID: 4409
		[GRspOptional]
		public string stage;

		// Token: 0x0400113A RID: 4410
		[GRspOptional]
		public string activatebydate;
	}
}
