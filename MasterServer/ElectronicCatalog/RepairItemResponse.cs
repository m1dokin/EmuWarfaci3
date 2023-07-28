using System;
using System.Runtime.InteropServices;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x0200023F RID: 575
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct RepairItemResponse
	{
		// Token: 0x06000C59 RID: 3161 RVA: 0x000307B8 File Offset: 0x0002EBB8
		public RepairItemResponse(int durability, int totalDurability, ulong repairCost, ulong gameMoney)
		{
			this = default(RepairItemResponse);
			this.Durability = durability;
			this.TotalDurability = totalDurability;
			this.RepairCost = repairCost;
			this.GameMoney = gameMoney;
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000C5A RID: 3162 RVA: 0x000307DE File Offset: 0x0002EBDE
		// (set) Token: 0x06000C5B RID: 3163 RVA: 0x000307E6 File Offset: 0x0002EBE6
		public int Durability { get; private set; }

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06000C5C RID: 3164 RVA: 0x000307EF File Offset: 0x0002EBEF
		// (set) Token: 0x06000C5D RID: 3165 RVA: 0x000307F7 File Offset: 0x0002EBF7
		public int TotalDurability { get; private set; }

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06000C5E RID: 3166 RVA: 0x00030800 File Offset: 0x0002EC00
		// (set) Token: 0x06000C5F RID: 3167 RVA: 0x00030808 File Offset: 0x0002EC08
		public ulong RepairCost { get; private set; }

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06000C60 RID: 3168 RVA: 0x00030811 File Offset: 0x0002EC11
		// (set) Token: 0x06000C61 RID: 3169 RVA: 0x00030819 File Offset: 0x0002EC19
		public ulong GameMoney { get; private set; }
	}
}
