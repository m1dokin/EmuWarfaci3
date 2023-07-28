using System;
using System.Text;
using MasterServer.Core;
using Util.Common;

namespace MasterServer.DAL
{
	// Token: 0x02000065 RID: 101
	[Serializable]
	public struct OfferItem
	{
		// Token: 0x060000F6 RID: 246 RVA: 0x00004548 File Offset: 0x00002948
		public override int GetHashCode()
		{
			CRC32 crc = new CRC32();
			crc.GetHash(Encoding.ASCII.GetBytes(TimeUtils.GetExpireTime(this.ExpirationTime)));
			int num = this.Item.GetHashCode();
			num ^= (int)crc.CRCVal;
			num ^= this.DurabilityPoints.GetHashCode();
			crc.Reset();
			crc.GetHash(Encoding.ASCII.GetBytes(this.RepairCost));
			num ^= (int)crc.CRCVal;
			return num ^ this.Quantity.GetHashCode();
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x000045E0 File Offset: 0x000029E0
		public override string ToString()
		{
			return string.Format("Item {0}, ExpirationTime {1}, DurabilityPoints {2}, RepairCost {3}, Quantity {4}", new object[]
			{
				this.Item,
				this.ExpirationTime,
				this.DurabilityPoints,
				this.RepairCost,
				this.Quantity
			});
		}

		// Token: 0x04000114 RID: 276
		public CatalogItem Item;

		// Token: 0x04000115 RID: 277
		public TimeSpan ExpirationTime;

		// Token: 0x04000116 RID: 278
		public int DurabilityPoints;

		// Token: 0x04000117 RID: 279
		public string RepairCost;

		// Token: 0x04000118 RID: 280
		public ulong Quantity;
	}
}
