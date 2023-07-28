using System;

namespace MasterServer.GFaceAPI.Responses
{
	// Token: 0x0200065B RID: 1627
	public class IdDTO : gFaceEntityDTO
	{
		// Token: 0x0400112F RID: 4399
		[GRspOptional]
		public long? id;

		// Token: 0x04001130 RID: 4400
		[GRspOptional]
		public SystemUserDTO gface;
	}
}
