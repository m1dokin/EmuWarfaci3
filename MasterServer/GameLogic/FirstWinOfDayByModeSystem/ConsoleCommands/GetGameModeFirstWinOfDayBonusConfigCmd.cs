using System;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.GameLogic.FirstWinOfDayByModeSystem.Configs.GameModeFirstWinOfDayBonusConfig;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem.ConsoleCommands
{
	// Token: 0x02000079 RID: 121
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "get_game_mode_first_win_of_day_bonus_config", Help = "Dumps game mode first win of day bonus config")]
	internal class GetGameModeFirstWinOfDayBonusConfigCmd : IConsoleCmd
	{
		// Token: 0x060001D2 RID: 466 RVA: 0x0000B3B4 File Offset: 0x000097B4
		public GetGameModeFirstWinOfDayBonusConfigCmd(IConfigProvider<GameModeFirstWinOfDayBonusConfig> gameModeFirstWinOfDayBonusConfigConfigProvider)
		{
			this.m_gameModeFirstWinOfDayBonusConfigConfigProvider = gameModeFirstWinOfDayBonusConfigConfigProvider;
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000B3C4 File Offset: 0x000097C4
		public void ExecuteCmd(string[] args)
		{
			GameModeFirstWinOfDayBonusConfig gameModeFirstWinOfDayBonusConfig = this.m_gameModeFirstWinOfDayBonusConfigConfigProvider.Get();
			Log.Info(gameModeFirstWinOfDayBonusConfig.ToString());
		}

		// Token: 0x040000DA RID: 218
		private readonly IConfigProvider<GameModeFirstWinOfDayBonusConfig> m_gameModeFirstWinOfDayBonusConfigConfigProvider;
	}
}
