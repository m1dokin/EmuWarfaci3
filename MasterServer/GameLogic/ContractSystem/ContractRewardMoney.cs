using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameLogic.ContractSystem
{
	// Token: 0x02000293 RID: 659
	internal class ContractRewardMoney : IContractReward
	{
		// Token: 0x06000E3C RID: 3644 RVA: 0x00038E7C File Offset: 0x0003727C
		public ContractRewardMoney(string name, uint amount)
		{
			this.Name = name;
			this.Amount = amount;
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000E3D RID: 3645 RVA: 0x00038E92 File Offset: 0x00037292
		// (set) Token: 0x06000E3E RID: 3646 RVA: 0x00038E9A File Offset: 0x0003729A
		public string Name { get; private set; }

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06000E3F RID: 3647 RVA: 0x00038EA3 File Offset: 0x000372A3
		// (set) Token: 0x06000E40 RID: 3648 RVA: 0x00038EAB File Offset: 0x000372AB
		public uint Amount { get; private set; }

		// Token: 0x06000E41 RID: 3649 RVA: 0x00038EB4 File Offset: 0x000372B4
		public uint GiveReward(ulong userId, ulong profileId, SRewardMultiplier dynamicMultiplier, ILogGroup logGroup)
		{
			IRewardService service = ServicesManager.GetService<IRewardService>();
			IItemCache service2 = ServicesManager.GetService<IItemCache>();
			Dictionary<string, SItem> allItemsByName = service2.GetAllItemsByName();
			string type = allItemsByName[this.Name].Type;
			Currency currency = (!(type == "game_money")) ? Currency.CrownMoney : Currency.GameMoney;
			float num = (!(type == "game_money")) ? dynamicMultiplier.CrownMultiplier : dynamicMultiplier.MoneyMultiplier;
			uint num2 = (uint)(0.5 + (double)(this.Amount * num));
			service.RewardMoney(userId, profileId, currency, (ulong)num2, logGroup, LogGroup.ProduceType.Contract);
			return num2;
		}
	}
}
