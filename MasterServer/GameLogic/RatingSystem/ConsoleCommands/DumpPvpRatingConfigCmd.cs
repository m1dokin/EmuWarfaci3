using System;
using System.Text;
using MasterServer.Core;
using MasterServer.Core.Configs;

namespace MasterServer.GameLogic.RatingSystem.ConsoleCommands
{
	// Token: 0x020000E9 RID: 233
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "dump_pvp_rating_config", Help = "Dumps PvP rating config")]
	internal class DumpPvpRatingConfigCmd : IConsoleCmd
	{
		// Token: 0x060003D2 RID: 978 RVA: 0x00010C4C File Offset: 0x0000F04C
		public DumpPvpRatingConfigCmd(IConfigProvider<RatingConfig> ratingConfigProvider)
		{
			this.m_ratingConfigProvider = ratingConfigProvider;
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00010C5C File Offset: 0x0000F05C
		public void ExecuteCmd(string[] args)
		{
			RatingConfig ratingConfig = this.m_ratingConfigProvider.Get();
			string value = ratingConfig.ToString();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Rating config content:");
			stringBuilder.Append(value);
			Log.Info(stringBuilder.ToString());
		}

		// Token: 0x0400019A RID: 410
		private readonly IConfigProvider<RatingConfig> m_ratingConfigProvider;
	}
}
