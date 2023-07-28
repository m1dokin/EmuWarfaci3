using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020002F3 RID: 755
	[ConsoleCmdAttributes(CmdName = "enable_debug_game_modes_settings", ArgsSize = 1, Help = "Dump to console debug_game_modes is enabled or disabled")]
	internal class EnableDebugGameModesSettingsCmd : IConsoleCmd
	{
		// Token: 0x06001183 RID: 4483 RVA: 0x000453F9 File Offset: 0x000437F9
		public void ExecuteCmd(string[] args)
		{
			if (args.Length >= 2)
			{
				Resources.DebugGameModeSettingsEnabled = (uint.Parse(args[1]) == 1U);
			}
			Log.Info<bool>("enable_debug_game_modes = {0}", Resources.DebugGameModeSettingsEnabled);
		}
	}
}
