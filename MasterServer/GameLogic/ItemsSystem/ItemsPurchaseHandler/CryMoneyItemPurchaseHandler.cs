using System;
using HK2Net;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameLogic.ItemsSystem.ItemsPurchaseHandler
{
	// Token: 0x02000322 RID: 802
	[Service]
	[Singleton]
	internal class CryMoneyItemPurchaseHandler : MoneyItemPurchaseHandler
	{
		// Token: 0x06001231 RID: 4657 RVA: 0x0004829A File Offset: 0x0004669A
		public CryMoneyItemPurchaseHandler(ICatalogService catalogService, IRewardService rewardService) : base(catalogService, rewardService)
		{
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x06001232 RID: 4658 RVA: 0x000482A4 File Offset: 0x000466A4
		public override string ItemType
		{
			get
			{
				return "cry_money";
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06001233 RID: 4659 RVA: 0x000482AB File Offset: 0x000466AB
		protected override Currency ItemCurrency
		{
			get
			{
				return Currency.CryMoney;
			}
		}
	}
}
