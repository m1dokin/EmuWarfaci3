using System;
using System.Linq;
using HK2Net;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;

namespace MasterServer.GameLogic.VoucherSystem
{
	// Token: 0x02000485 RID: 1157
	[Service]
	[Singleton]
	internal class VoucherValidator : IVoucherValidator
	{
		// Token: 0x06001857 RID: 6231 RVA: 0x00064BDB File Offset: 0x00062FDB
		public VoucherValidator(IItemService itemService)
		{
			this.m_itemService = itemService;
		}

		// Token: 0x06001858 RID: 6232 RVA: 0x00064BEA File Offset: 0x00062FEA
		public bool IsValid(Voucher voucher)
		{
			return voucher.Aggregate(voucher.Any<VoucherItem>(), (bool current, VoucherItem item) => current & this.CheckVoucherItem(item));
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x00064C04 File Offset: 0x00063004
		private bool CheckVoucherItem(VoucherItem item)
		{
			bool flag = this.m_itemService.CanGiveItem(item.ItemName, item.Type);
			OfferType type = item.Type;
			if (type != OfferType.Expiration)
			{
				if (type == OfferType.Consumable)
				{
					flag &= (item.Quantity > 0);
				}
			}
			else
			{
				flag &= (item.ExpirationTime > TimeSpan.Zero);
			}
			return flag;
		}

		// Token: 0x04000BB1 RID: 2993
		private readonly IItemService m_itemService;
	}
}
