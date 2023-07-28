using System;

namespace MasterServer.Core
{
	// Token: 0x0200014D RID: 333
	public enum ExecutionPhase
	{
		// Token: 0x040003C1 RID: 961
		Initializing,
		// Token: 0x040003C2 RID: 962
		PreUpdate,
		// Token: 0x040003C3 RID: 963
		Update,
		// Token: 0x040003C4 RID: 964
		PostUpdate,
		// Token: 0x040003C5 RID: 965
		Starting,
		// Token: 0x040003C6 RID: 966
		Started,
		// Token: 0x040003C7 RID: 967
		Stopping,
		// Token: 0x040003C8 RID: 968
		Stopped
	}
}
