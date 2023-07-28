using System;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000241 RID: 577
	public enum RepairStatus
	{
		// Token: 0x040005C3 RID: 1475
		Undefined,
		// Token: 0x040005C4 RID: 1476
		Ok,
		// Token: 0x040005C5 RID: 1477
		Failed,
		// Token: 0x040005C6 RID: 1478
		Prohibited,
		// Token: 0x040005C7 RID: 1479
		NotEnoughMoney,
		// Token: 0x040005C8 RID: 1480
		OffByUser,
		// Token: 0x040005C9 RID: 1481
		PartiallyRepaired,
		// Token: 0x040005CA RID: 1482
		NothingToRepair,
		// Token: 0x040005CB RID: 1483
		InternalError
	}
}
