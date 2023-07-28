using System;
using System.Text;
using MasterServer.DAL;
using Util.Common;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200036F RID: 879
	public class SProfileItem
	{
		// Token: 0x060013C1 RID: 5057 RVA: 0x00050EB4 File Offset: 0x0004F2B4
		public SProfileItem(SProfileItem item)
		{
			this.EquipItem = (SEquipItem)item.EquipItem.Clone();
			this.GameItem = item.GameItem;
			this.CustomerItem = item.CustomerItem;
		}

		// Token: 0x060013C2 RID: 5058 RVA: 0x00050EEA File Offset: 0x0004F2EA
		public SProfileItem(SEquipItem equip, SItem game, CustomerItem cat)
		{
			this.EquipItem = equip;
			this.GameItem = game;
			this.CustomerItem = (cat ?? new CustomerItem());
		}

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x060013C3 RID: 5059 RVA: 0x00050F13 File Offset: 0x0004F313
		public ulong ProfileItemID
		{
			get
			{
				return this.EquipItem.ProfileItemID;
			}
		}

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x060013C4 RID: 5060 RVA: 0x00050F20 File Offset: 0x0004F320
		public ulong ItemID
		{
			get
			{
				return this.GameItem.ID;
			}
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x060013C5 RID: 5061 RVA: 0x00050F2D File Offset: 0x0004F32D
		public ulong CatalogID
		{
			get
			{
				return this.EquipItem.CatalogID;
			}
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x060013C6 RID: 5062 RVA: 0x00050F3A File Offset: 0x0004F33A
		public ulong AttachedTo
		{
			get
			{
				return this.EquipItem.AttachedTo;
			}
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x060013C7 RID: 5063 RVA: 0x00050F47 File Offset: 0x0004F347
		public ulong SlotIDs
		{
			get
			{
				return this.EquipItem.SlotIDs;
			}
		}

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x060013C8 RID: 5064 RVA: 0x00050F54 File Offset: 0x0004F354
		public string Config
		{
			get
			{
				return this.EquipItem.Config;
			}
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x060013C9 RID: 5065 RVA: 0x00050F61 File Offset: 0x0004F361
		public EProfileItemStatus Status
		{
			get
			{
				return this.EquipItem.Status;
			}
		}

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x060013CA RID: 5066 RVA: 0x00050F6E File Offset: 0x0004F36E
		public bool IsExpired
		{
			get
			{
				return this.EquipItem.IsExpired;
			}
		}

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x060013CB RID: 5067 RVA: 0x00050F7B File Offset: 0x0004F37B
		public bool IsDefault
		{
			get
			{
				return this.EquipItem.IsDefault;
			}
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x060013CC RID: 5068 RVA: 0x00050F88 File Offset: 0x0004F388
		public bool IsReward
		{
			get
			{
				return this.EquipItem.IsReward;
			}
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x060013CD RID: 5069 RVA: 0x00050F95 File Offset: 0x0004F395
		public bool IsEquipped
		{
			get
			{
				return this.EquipItem.IsEquipped;
			}
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x060013CE RID: 5070 RVA: 0x00050FA2 File Offset: 0x0004F3A2
		public OfferType OfferType
		{
			get
			{
				return this.CustomerItem.OfferType;
			}
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x060013CF RID: 5071 RVA: 0x00050FAF File Offset: 0x0004F3AF
		public int TotalDurabilityPoints
		{
			get
			{
				return this.CustomerItem.TotalDurabilityPoints;
			}
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x060013D0 RID: 5072 RVA: 0x00050FBC File Offset: 0x0004F3BC
		public int DurabilityPoints
		{
			get
			{
				return this.CustomerItem.DurabilityPoints;
			}
		}

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x060013D1 RID: 5073 RVA: 0x00050FC9 File Offset: 0x0004F3C9
		public bool IsBroken
		{
			get
			{
				return this.CustomerItem.DurabilityPoints < this.CustomerItem.TotalDurabilityPoints;
			}
		}

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x060013D2 RID: 5074 RVA: 0x00050FE3 File Offset: 0x0004F3E3
		public ulong BuyTimeUTC
		{
			get
			{
				return this.CustomerItem.BuyTimeUTC;
			}
		}

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x060013D3 RID: 5075 RVA: 0x00050FF0 File Offset: 0x0004F3F0
		public DateTime BuyTime
		{
			get
			{
				return TimeUtils.UTCTimestampToUTCTime(this.BuyTimeUTC);
			}
		}

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x060013D4 RID: 5076 RVA: 0x00050FFD File Offset: 0x0004F3FD
		public ulong ExpirationTimeUTC
		{
			get
			{
				return this.CustomerItem.ExpirationTimeUTC;
			}
		}

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x060013D5 RID: 5077 RVA: 0x0005100A File Offset: 0x0004F40A
		public DateTime ExpirationTime
		{
			get
			{
				return TimeUtils.UTCTimestampToUTCTime(this.ExpirationTimeUTC);
			}
		}

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x060013D6 RID: 5078 RVA: 0x00051018 File Offset: 0x0004F418
		public int SecondsLeft
		{
			get
			{
				return (int)Math.Max(Math.Floor((this.ExpirationTime - DateTime.UtcNow).TotalSeconds), 0.0);
			}
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x060013D7 RID: 5079 RVA: 0x00051051 File Offset: 0x0004F451
		public ulong Quantity
		{
			get
			{
				return this.CustomerItem.Quantity;
			}
		}

		// Token: 0x060013D8 RID: 5080 RVA: 0x00051060 File Offset: 0x0004F460
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("Id: {0}, ItemId: {1}, AttachedTo: {2}, SlotIds:{3}, Status:{4}, Name: {5} ", new object[]
			{
				this.ProfileItemID,
				this.ItemID,
				this.AttachedTo,
				this.SlotIDs,
				this.Status,
				this.GameItem.Name
			}));
			if (this.CatalogID > 0UL)
			{
				stringBuilder.Append(string.Format(", OfferType: {0}, Durability: {1}, TotalDurability: {2}, Expiration: {3}, Quantity: {4}, BuyTime: {5}", new object[]
				{
					this.OfferType,
					this.DurabilityPoints,
					this.TotalDurabilityPoints,
					this.ExpirationTime,
					this.Quantity,
					this.BuyTime
				}));
			}
			stringBuilder.Append(string.Format(", IsActive: {0}", this.GameItem.Active));
			return stringBuilder.ToString();
		}

		// Token: 0x060013D9 RID: 5081 RVA: 0x0005117D File Offset: 0x0004F57D
		public ulong CalculateRepairCost(SRepairItemDesc rid)
		{
			if (this.DurabilityPoints >= this.TotalDurabilityPoints)
			{
				return 0UL;
			}
			return (ulong)Math.Ceiling((double)(this.TotalDurabilityPoints - this.DurabilityPoints) * (double)rid.RepairCost / (double)rid.Durability);
		}

		// Token: 0x060013DA RID: 5082 RVA: 0x000511B9 File Offset: 0x0004F5B9
		public int CalculateRepairDurability(SRepairItemDesc rid, ulong repairCost)
		{
			return (int)Math.Ceiling(repairCost * (double)rid.Durability / (double)rid.RepairCost);
		}

		// Token: 0x04000934 RID: 2356
		public SEquipItem EquipItem;

		// Token: 0x04000935 RID: 2357
		public SItem GameItem;

		// Token: 0x04000936 RID: 2358
		public CustomerItem CustomerItem;
	}
}
