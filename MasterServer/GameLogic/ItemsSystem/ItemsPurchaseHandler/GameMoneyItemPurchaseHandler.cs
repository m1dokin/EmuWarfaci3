using System;
using HK2Net;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameLogic.ItemsSystem.ItemsPurchaseHandler
{
	// Token: 0x02000320 RID: 800
	[Service]
	[Singleton]
	internal class GameMoneyItemPurchaseHandler : MoneyItemPurchaseHandler
	{
		// Token: 0x0600122B RID: 4651 RVA: 0x00048272 File Offset: 0x00046672
		public GameMoneyItemPurchaseHandler(ICatalogService catalogService, IRewardService rewardService) : base(catalogService, rewardService)
		{
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x0600122C RID: 4652 RVA: 0x0004827C File Offset: 0x0004667C
		public override string ItemType
		{
			get
			{
				return "game_money";
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x0600122D RID: 4653 RVA: 0x00048283 File Offset: 0x00046683
		protected override Currency ItemCurrency
		{
			get
			{
				return Currency.GameMoney;
			}
		}
	}
}
