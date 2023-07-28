using System;

namespace MasterServer.GFaceAPI.Responses
{
	// Token: 0x02000661 RID: 1633
	public class SeedDTO : IdDTO
	{
		// Token: 0x04001146 RID: 4422
		public string type;

		// Token: 0x04001147 RID: 4423
		public string title;

		// Token: 0x04001148 RID: 4424
		[GRspOptional]
		public string category;

		// Token: 0x04001149 RID: 4425
		public IdDTO owner;

		// Token: 0x0400114A RID: 4426
		public IdDTO author;

		// Token: 0x0400114B RID: 4427
		[GRspOptional]
		public string created;

		// Token: 0x0400114C RID: 4428
		[GRspOptional]
		public string modified;

		// Token: 0x0400114D RID: 4429
		public string permission;

		// Token: 0x0400114E RID: 4430
		[GRspOptional]
		public string removed;

		// Token: 0x0400114F RID: 4431
		[GRspOptional]
		public IdDTO remover;
	}
}
