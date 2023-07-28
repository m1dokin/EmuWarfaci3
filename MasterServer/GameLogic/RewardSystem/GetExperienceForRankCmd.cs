using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005B1 RID: 1457
	[ConsoleCmdAttributes(CmdName = "get_experience_for_rank", ArgsSize = 1, Help = "Return experience needed to get specified rank")]
	internal class GetExperienceForRankCmd : IConsoleCmd
	{
		// Token: 0x06001F51 RID: 8017 RVA: 0x0007F25A File Offset: 0x0007D65A
		public GetExperienceForRankCmd(IRankSystem rankSystem)
		{
			this.m_rankSystem = rankSystem;
		}

		// Token: 0x06001F52 RID: 8018 RVA: 0x0007F26C File Offset: 0x0007D66C
		public void ExecuteCmd(string[] args)
		{
			int num = int.Parse(args[1]);
			Log.Info<int, string>("Rank: {0}, Exp: {1}", num, this.m_rankSystem.GetExperience(num).ToString());
		}

		// Token: 0x04000F3C RID: 3900
		private readonly IRankSystem m_rankSystem;
	}
}
