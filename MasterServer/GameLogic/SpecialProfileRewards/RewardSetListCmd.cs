using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.SpecialProfileRewards
{
	// Token: 0x020000F3 RID: 243
	[ConsoleCmdAttributes(CmdName = "reward_set_list", ArgsSize = 0, Help = "Lists all active reward sets")]
	internal class RewardSetListCmd : IConsoleCmd
	{
		// Token: 0x060003FB RID: 1019 RVA: 0x000114CC File Offset: 0x0000F8CC
		public RewardSetListCmd(ISpecialProfileRewardServiceDebug profileRewardService)
		{
			this.m_profileRewardService = profileRewardService;
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x000114DB File Offset: 0x0000F8DB
		public void ExecuteCmd(string[] args)
		{
			this.m_profileRewardService.DumpRewardSets();
		}

		// Token: 0x040001AE RID: 430
		private readonly ISpecialProfileRewardServiceDebug m_profileRewardService;
	}
}
