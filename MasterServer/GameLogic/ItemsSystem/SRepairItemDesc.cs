using System;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200035E RID: 862
	public struct SRepairItemDesc
	{
		// Token: 0x06001353 RID: 4947 RVA: 0x0004F4B5 File Offset: 0x0004D8B5
		public SRepairItemDesc(int durability, int repairCost)
		{
			this.Durability = durability;
			this.RepairCost = repairCost;
		}

		// Token: 0x06001354 RID: 4948 RVA: 0x0004F4C8 File Offset: 0x0004D8C8
		public override bool Equals(object obj)
		{
			if (!(obj is SRepairItemDesc))
			{
				return false;
			}
			SRepairItemDesc srepairItemDesc = (SRepairItemDesc)obj;
			return this.Durability == srepairItemDesc.Durability && this.RepairCost == srepairItemDesc.RepairCost;
		}

		// Token: 0x06001355 RID: 4949 RVA: 0x0004F50D File Offset: 0x0004D90D
		public override string ToString()
		{
			return string.Format("Durability: {0}, RepairCost: {1}", this.Durability, this.RepairCost);
		}

		// Token: 0x040008FA RID: 2298
		public int Durability;

		// Token: 0x040008FB RID: 2299
		public int RepairCost;
	}
}
