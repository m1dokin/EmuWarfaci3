using System;
using System.Collections;
using System.Linq;
using System.Text;
using MasterServer.Core;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020002F4 RID: 756
	[ConsoleCmdAttributes(CmdName = "dump_game_mode_settings", ArgsSize = 2, Help = "Dump to console game mode settings per room type")]
	internal class DumpGameModeSettingsToConsoleCmd : IConsoleCmd
	{
		// Token: 0x06001184 RID: 4484 RVA: 0x00045423 File Offset: 0x00043823
		public DumpGameModeSettingsToConsoleCmd(IGameModesSystem gameModesSystem)
		{
			this.m_gameModesSystem = gameModesSystem;
		}

		// Token: 0x06001185 RID: 4485 RVA: 0x00045434 File Offset: 0x00043834
		public void ExecuteCmd(string[] args)
		{
			if (args.Length > 1)
			{
				GameRoomType roomType = (args.Length <= 2) ? GameRoomType.All : ((GameRoomType)Enum.Parse(typeof(GameRoomType), args[2], true));
				GameModeSetting gameModeSetting = this.m_gameModesSystem.GetGameModeSetting(args[1]);
				if (gameModeSetting != null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendFormat("Game settings for mode {0}\n", args[1]);
					IEnumerator enumerator = Enum.GetValues(typeof(ERoomSetting)).GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							ERoomSetting eroomSetting = (ERoomSetting)obj;
							if (eroomSetting != ERoomSetting.NOT_SETTING)
							{
								string arg;
								if (gameModeSetting.GetSetting(roomType, eroomSetting, out arg))
								{
									stringBuilder.AppendFormat("\t{0} = {1}\n", eroomSetting, arg);
								}
							}
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
					Log.Info(stringBuilder.ToString());
				}
			}
			else
			{
				string p = Enum.GetValues(typeof(GameRoomType)).Cast<GameRoomType>().Aggregate(string.Empty, (string current, GameRoomType type) => current + string.Format("{0} = {1}\n", type.ToString(), (int)type));
				Log.Info<string>("Enter the name of game mode as a parameter, available modes: pve, ffa, ctf, ptb, stm, tdm, dst.\ndump_game_mode_settings <GameMode>\n Room typies: {0}", p);
			}
		}

		// Token: 0x040007B8 RID: 1976
		private readonly IGameModesSystem m_gameModesSystem;
	}
}
