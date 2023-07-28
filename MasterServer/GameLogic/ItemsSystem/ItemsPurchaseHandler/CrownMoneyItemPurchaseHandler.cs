using System;
using HK2Net;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameLogic.ItemsSystem.ItemsPurchaseHandler
{
	// Token: 0x02000321 RID: 801
	[Service]
	[Singleton]
	internal class CrownMoneyItemPurchaseHandler : MoneyItemPurchaseHandler
	{
		// Token: 0x0600122E RID: 4654 RVA: 0x00048286 File Offset: 0x00046686
		public CrownMoneyItemPurchaseHandler(ICatalogService catalogService, IRewardService rewardService) : base(catalogService, rewardService)
		{
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x0600122F RID: 4655 RVA: 0x00048290 File Offset: 0x00046690
		public override string ItemType
		{
			get
			{
				return "crown_money";
			}
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x06001230 RID: 4656 RVA: 0x00048297 File Offset: 0x00046697
		protected override Currency ItemCurrency
		{
			get
			{
				return Currency.CrownMoney;
			}
		}
	}
}
