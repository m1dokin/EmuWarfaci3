using System;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020002F8 RID: 760
	[ConsoleCmdAttributes(CmdName = "dump_room_game_mode_settings", ArgsSize = 1, Help = "Dump game mode settings in room with player")]
	internal class DumpRoomGameModeSettingsToConsoleCmd : IConsoleCmd
	{
		// Token: 0x0600118D RID: 4493 RVA: 0x000457A3 File Offset: 0x00043BA3
		public DumpRoomGameModeSettingsToConsoleCmd(IGameRoomManager gameRoomManager)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x0600118E RID: 4494 RVA: 0x000457B4 File Offset: 0x00043BB4
		public void ExecuteCmd(string[] args)
		{
			if (args.Length >= 2)
			{
				IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(ulong.Parse(args[1]));
				if (roomByPlayer != null)
				{
					roomByPlayer.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
					{
						Log.Info<ulong>("Game settings for room {0}", r.ID);
						Log.Info<int>("\tMIN_PLAYERS_READY = {0}", r.MinReadyPlayers);
						Log.Info<int>("\tTEAMS_READY_PLAYERS_DIFF = {0}", r.TeamsReadyPlayersDiff);
						Log.Info<bool>("\tNO_TEAMS_MODE = {0}", r.NoTeamsMode);
					});
				}
				else
				{
					Log.Info("You must be in the room.");
				}
			}
			else
			{
				Log.Info("Enter your ProfileID.\ndump_game_mode_settings <ProfileID>");
			}
		}

		// Token: 0x040007BD RID: 1981
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
