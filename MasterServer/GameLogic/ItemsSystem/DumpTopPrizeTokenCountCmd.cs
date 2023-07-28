using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Core;
using MasterServer.GameLogic.ItemsSystem.WinModels;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000346 RID: 838
	[ConsoleCmdAttributes(CmdName = "dump_top_prize_tokens", ArgsSize = 3, Help = "dump_top_prize_tokens user_id (token_item_name)")]
	public class DumpTopPrizeTokenCountCmd : IConsoleCmd
	{
		// Token: 0x060012C0 RID: 4800 RVA: 0x0004BA85 File Offset: 0x00049E85
		public DumpTopPrizeTokenCountCmd(IWinModelFactory winModelFactory)
		{
			this.m_currentWinModel = winModelFactory.GetWinModel();
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x0004BA9C File Offset: 0x00049E9C
		public void ExecuteCmd(string[] args)
		{
			if (args.Length == 1)
			{
				Log.Info("Too few arguments specified.");
				return;
			}
			ulong num = ulong.Parse(args[1]);
			IEnumerable<KeyValuePair<string, ulong>> source = this.m_currentWinModel.GetCollectedPrizeTokensCount(num).AsEnumerable<KeyValuePair<string, ulong>>();
			if (args.Length > 2)
			{
				source = from t in source
				where t.Key == args[2]
				select t;
			}
			Log.Info(string.Format("User {0} possesses the following tokens:\n{1}", num, string.Join("\n", (from pair in source
			select string.Format("{0}: {1}", pair.Key, pair.Value)).ToArray<string>())));
		}

		// Token: 0x040008AE RID: 2222
		private readonly IWinModel m_currentWinModel;
	}
}
