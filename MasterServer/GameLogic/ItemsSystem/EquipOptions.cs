using System;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000370 RID: 880
	[Flags]
	public enum EquipOptions
	{
		// Token: 0x04000938 RID: 2360
		All = 1,
		// Token: 0x04000939 RID: 2361
		ActiveOnly = 2,
		// Token: 0x0400093A RID: 2362
		FilterByTags = 4
	}
}
