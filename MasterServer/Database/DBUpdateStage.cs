using System;

namespace MasterServer.Database
{
	// Token: 0x020001D2 RID: 466
	public enum DBUpdateStage
	{
		// Token: 0x04000524 RID: 1316
		None,
		// Token: 0x04000525 RID: 1317
		PreUpdate,
		// Token: 0x04000526 RID: 1318
		Schema,
		// Token: 0x04000527 RID: 1319
		Procedures,
		// Token: 0x04000528 RID: 1320
		Data,
		// Token: 0x04000529 RID: 1321
		PostUpdate,
		// Token: 0x0400052A RID: 1322
		CheckVersion
	}
}
