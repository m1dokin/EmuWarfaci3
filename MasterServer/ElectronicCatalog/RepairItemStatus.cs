using System;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000240 RID: 576
	[Serializable]
	public class RepairItemStatus
	{
		// Token: 0x040005BD RID: 1469
		public ulong ProfileItemId;

		// Token: 0x040005BE RID: 1470
		public ulong MoneySpent;

		// Token: 0x040005BF RID: 1471
		public RepairStatus RepairStatus;

		// Token: 0x040005C0 RID: 1472
		public int TotalDurability;

		// Token: 0x040005C1 RID: 1473
		public int Durability;
	}
}
