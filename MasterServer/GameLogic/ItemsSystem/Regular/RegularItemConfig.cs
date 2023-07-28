using System;

namespace MasterServer.GameLogic.ItemsSystem.Regular
{
	// Token: 0x02000071 RID: 113
	internal class RegularItemConfig
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x0000B132 File Offset: 0x00009532
		// (set) Token: 0x060001B8 RID: 440 RVA: 0x0000B13A File Offset: 0x0000953A
		public uint MaxAmount { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060001B9 RID: 441 RVA: 0x0000B143 File Offset: 0x00009543
		// (set) Token: 0x060001BA RID: 442 RVA: 0x0000B14B File Offset: 0x0000954B
		public bool StackingEnabled { get; set; }
	}
}
