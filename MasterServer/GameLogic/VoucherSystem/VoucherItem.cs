using System;
using MasterServer.DAL;

namespace MasterServer.GameLogic.VoucherSystem
{
	// Token: 0x0200047E RID: 1150
	public struct VoucherItem
	{
		// Token: 0x0600183F RID: 6207 RVA: 0x000642AC File Offset: 0x000626AC
		public override bool Equals(object obj)
		{
			if (!(obj is VoucherItem))
			{
				return false;
			}
			VoucherItem voucherItem = (VoucherItem)obj;
			return this.ItemName == voucherItem.ItemName && this.Type == voucherItem.Type && this.Durability == voucherItem.Durability && this.Quantity == voucherItem.Quantity && this.ExpirationTime == voucherItem.ExpirationTime;
		}

		// Token: 0x06001840 RID: 6208 RVA: 0x0006432F File Offset: 0x0006272F
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06001841 RID: 6209 RVA: 0x00064344 File Offset: 0x00062744
		public override string ToString()
		{
			return string.Format("Name: {0}, Type: {1}, Durability: {2}, Quantity: {3}, ExpirationTime: {4}", new object[]
			{
				this.ItemName,
				this.Type,
				this.Durability,
				this.Quantity,
				this.ExpirationTime
			});
		}

		// Token: 0x04000BA6 RID: 2982
		public string ItemName;

		// Token: 0x04000BA7 RID: 2983
		public OfferType Type;

		// Token: 0x04000BA8 RID: 2984
		public int Durability;

		// Token: 0x04000BA9 RID: 2985
		public ushort Quantity;

		// Token: 0x04000BAA RID: 2986
		public TimeSpan ExpirationTime;
	}
}
