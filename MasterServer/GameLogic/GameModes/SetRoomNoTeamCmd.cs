using System;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020002F7 RID: 759
	[ConsoleCmdAttributes(CmdName = "set_room_no_team", ArgsSize = 2, Help = "Set no team in room with player")]
	internal class SetRoomNoTeamCmd : IConsoleCmd
	{
		// Token: 0x0600118B RID: 4491 RVA: 0x000456F1 File Offset: 0x00043AF1
		public SetRoomNoTeamCmd(IGameRoomManager gameRoomManager)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x0600118C RID: 4492 RVA: 0x00045700 File Offset: 0x00043B00
		public void ExecuteCmd(string[] args)
		{
			if (args.Length >= 3)
			{
				IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(ulong.Parse(args[1]));
				if (roomByPlayer != null)
				{
					roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						r.GetState<MissionState>(AccessMode.ReadWrite).Mission.noTeamsMode = (uint.Parse(args[2]) == 1U);
					});
				}
				else
				{
					Log.Info("You must be in the room.");
				}
			}
			else
			{
				Log.Info("Enter your ProfileID and new value.\nset_room_no_team <ProfileID> <Value>");
			}
		}

		// Token: 0x040007BC RID: 1980
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
