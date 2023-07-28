using System;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020002F6 RID: 758
	[ConsoleCmdAttributes(CmdName = "set_room_ready_players_diff", ArgsSize = 2, Help = "Set team diff in room with player")]
	internal class SetTeamsReadyPlayersDiffCmd : IConsoleCmd
	{
		// Token: 0x06001189 RID: 4489 RVA: 0x0004564D File Offset: 0x00043A4D
		public SetTeamsReadyPlayersDiffCmd(IGameRoomManager gameRoomManager)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x0600118A RID: 4490 RVA: 0x0004565C File Offset: 0x00043A5C
		public void ExecuteCmd(string[] args)
		{
			if (args.Length >= 3)
			{
				IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(ulong.Parse(args[1]));
				if (roomByPlayer != null)
				{
					roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						r.TeamsReadyPlayersDiff = int.Parse(args[2]);
					});
				}
				else
				{
					Log.Info("You must be in the room.");
				}
			}
			else
			{
				Log.Info("Enter your ProfileID and new value.\nset_room_ready_players_diff <ProfileID> <Value>");
			}
		}

		// Token: 0x040007BB RID: 1979
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
