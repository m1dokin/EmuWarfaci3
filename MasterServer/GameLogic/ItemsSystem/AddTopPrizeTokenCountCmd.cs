using System;
using MasterServer.Core;
using MasterServer.GameLogic.ItemsSystem.WinModels;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000347 RID: 839
	[ConsoleCmdAttributes(CmdName = "add_top_prize_tokens", ArgsSize = 4, Help = "add_top_prize_tokens user_id token_item_name (count)")]
	public class AddTopPrizeTokenCountCmd : IConsoleCmd
	{
		// Token: 0x060012C3 RID: 4803 RVA: 0x0004BB94 File Offset: 0x00049F94
		public AddTopPrizeTokenCountCmd(IWinModelFactory winModelFactory)
		{
			this.m_currentWinModel = winModelFactory.GetWinModel();
		}

		// Token: 0x060012C4 RID: 4804 RVA: 0x0004BBA8 File Offset: 0x00049FA8
		public void ExecuteCmd(string[] args)
		{
			if (args.Length < 3)
			{
				Log.Info("Too few arguments specified.");
				return;
			}
			ulong num = ulong.Parse(args[1]);
			string text = args[2];
			uint num2 = (args.Length <= 3) ? 1U : uint.Parse(args[3]);
			int num3 = 0;
			int num4 = 0;
			while ((long)num4 < (long)((ulong)num2))
			{
				num3 = this.m_currentWinModel.AddPrizeToken(num, text);
				num4++;
			}
			Log.Info(string.Format("User {0} possesses {1} '{2}' tokens now.", num, num3, text));
		}

		// Token: 0x040008B0 RID: 2224
		private readonly IWinModel m_currentWinModel;
	}
}
