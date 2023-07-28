using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterServer.Core;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x02000307 RID: 775
	[ConsoleCmdAttributes(CmdName = "set_debug_setting", ArgsSize = 3, Help = "<gameMode> <setting> <value>")]
	internal class SetGameModeSettingsConsoleCmd : IConsoleCmd
	{
		// Token: 0x060011E8 RID: 4584 RVA: 0x00046C78 File Offset: 0x00045078
		public SetGameModeSettingsConsoleCmd(IGameModesSystem gameModesSystem)
		{
			this.m_gameModesSystem = gameModesSystem;
		}

		// Token: 0x060011E9 RID: 4585 RVA: 0x00046C88 File Offset: 0x00045088
		public void ExecuteCmd(string[] args)
		{
			if (args.Length > 1)
			{
				List<string> list = new List<string>();
				if (args[1].Equals("pve"))
				{
					list.AddRange(from x in Enum.GetNames(typeof(ProfileProgressionInfo.MissionType))
					select x.ToLower());
				}
				else if (args[1].Equals("pvp"))
				{
					IEnumerable<string> pveModes = from x in Enum.GetNames(typeof(ProfileProgressionInfo.MissionType))
					select x.ToLower();
					list.AddRange(from x in this.m_gameModesSystem.GetSupportedModes()
					where !pveModes.Contains(x)
					select x);
				}
				else
				{
					list.Add(args[1]);
				}
				ERoomSetting eroomSetting;
				if (!Enum.TryParse<ERoomSetting>(args[2], true, out eroomSetting))
				{
					Log.Info<string, string>("Incorrect setting {0}, supported {1}", args[2], string.Join(", ", Enum.GetNames(typeof(ERoomSetting))));
					return;
				}
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string text in list)
				{
					GameModeSetting gameModeSetting = this.m_gameModesSystem.GetGameModeSetting(text);
					if (gameModeSetting != null)
					{
						stringBuilder.AppendLine(string.Format("Game settings for mode {0}", text));
						string text2 = args[3];
						gameModeSetting.SetDebugSetting(eroomSetting, text2);
						gameModeSetting.GetSetting(eroomSetting, out text2);
						stringBuilder.AppendLine(string.Format("{0} = {1}", eroomSetting, text2));
					}
				}
				Log.Info(stringBuilder.ToString());
			}
			else
			{
				Log.Info<string>("Enter the name of game mode as a parameter, available modes: pve, pvp, {0}.\ndump_game_mode_settings <GameMode>", string.Join(", ", this.m_gameModesSystem.GetSupportedModes()));
			}
		}

		// Token: 0x04000802 RID: 2050
		private readonly IGameModesSystem m_gameModesSystem;
	}
}
