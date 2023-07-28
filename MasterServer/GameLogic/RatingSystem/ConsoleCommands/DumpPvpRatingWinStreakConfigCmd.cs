using System;
using System.Text;
using MasterServer.Core;
using MasterServer.Core.Configs;

namespace MasterServer.GameLogic.RatingSystem.ConsoleCommands
{
	// Token: 0x02000090 RID: 144
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "dump_pvp_rating_win_streak_config", Help = "Dumps PvP rating win streak config")]
	internal class DumpPvpRatingWinStreakConfigCmd : IConsoleCmd
	{
		// Token: 0x0600022D RID: 557 RVA: 0x0000C162 File Offset: 0x0000A562
		public DumpPvpRatingWinStreakConfigCmd(IConfigProvider<RatingWinStreakConfig> ratingWinStreakConfigProvider)
		{
			this.m_ratingWinStreakConfigProvider = ratingWinStreakConfigProvider;
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000C174 File Offset: 0x0000A574
		public void ExecuteCmd(string[] args)
		{
			RatingWinStreakConfig ratingWinStreakConfig = this.m_ratingWinStreakConfigProvider.Get();
			string value = ratingWinStreakConfig.ToString();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Rating win streak config content:");
			stringBuilder.Append(value);
			Log.Info(stringBuilder.ToString());
		}

		// Token: 0x040000F8 RID: 248
		private readonly IConfigProvider<RatingWinStreakConfig> m_ratingWinStreakConfigProvider;
	}
}
