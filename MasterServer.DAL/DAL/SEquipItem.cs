using System;

namespace MasterServer.DAL
{
	// Token: 0x02000075 RID: 117
	[Serializable]
	public class SEquipItem : ICloneable
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000144 RID: 324 RVA: 0x00004A1D File Offset: 0x00002E1D
		public bool IsExpired
		{
			get
			{
				return this.Status == EProfileItemStatus.EXPIRED || this.Status == EProfileItemStatus.EXPIRED_CONFIRMED;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000145 RID: 325 RVA: 0x00004A37 File Offset: 0x00002E37
		public bool IsDefault
		{
			get
			{
				return this.Status == EProfileItemStatus.DEFAULT;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000146 RID: 326 RVA: 0x00004A42 File Offset: 0x00002E42
		public bool IsReward
		{
			get
			{
				return this.Status == EProfileItemStatus.REWARD;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000147 RID: 327 RVA: 0x00004A4D File Offset: 0x00002E4D
		public bool IsEquipped
		{
			get
			{
				return this.SlotIDs != 0UL;
			}
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00004A5C File Offset: 0x00002E5C
		public object Clone()
		{
			return base.MemberwiseClone();
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00004A64 File Offset: 0x00002E64
		public override string ToString()
		{
			return string.Format("ItemID: {0}, ProfileItemID: {1}, ProfileID: {2}, AttachedTo: {3}, SlotIDs: {4}, Status: {5}, CatalogID: {6}", new object[]
			{
				this.ItemID,
				this.ProfileItemID,
				this.ProfileID,
				this.AttachedTo,
				this.SlotIDs,
				this.Status,
				this.CatalogID
			});
		}

		// Token: 0x04000138 RID: 312
		public ulong ItemID;

		// Token: 0x04000139 RID: 313
		public ulong ProfileItemID;

		// Token: 0x0400013A RID: 314
		public ulong ProfileID;

		// Token: 0x0400013B RID: 315
		public ulong AttachedTo;

		// Token: 0x0400013C RID: 316
		public ulong SlotIDs;

		// Token: 0x0400013D RID: 317
		public string Config;

		// Token: 0x0400013E RID: 318
		public EProfileItemStatus Status;

		// Token: 0x0400013F RID: 319
		public ulong CatalogID;
	}
}
