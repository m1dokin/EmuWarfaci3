using System;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020002F5 RID: 757
	[ConsoleCmdAttributes(CmdName = "set_room_min_players_ready", ArgsSize = 2, Help = "Set min player ready in room with player")]
	internal class SetRoomMinPlayersReadyCmd : IConsoleCmd
	{
		// Token: 0x06001187 RID: 4487 RVA: 0x000455A9 File Offset: 0x000439A9
		public SetRoomMinPlayersReadyCmd(IGameRoomManager gameRoomManager)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x06001188 RID: 4488 RVA: 0x000455B8 File Offset: 0x000439B8
		public void ExecuteCmd(string[] args)
		{
			if (args.Length >= 3)
			{
				IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(ulong.Parse(args[1]));
				if (roomByPlayer != null)
				{
					roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						r.MinReadyPlayers = int.Parse(args[2]);
					});
				}
				else
				{
					Log.Info("You must be in the room.");
				}
			}
			else
			{
				Log.Info("Enter your ProfileID and new value.\nset_room_min_players_ready <ProfileID> <Value>");
			}
		}

		// Token: 0x040007BA RID: 1978
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
