using System;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000468 RID: 1128
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "debug_room_reset_team", Help = "Reset team id in autobalance room.")]
	internal class DebugRoomResetTeamCmd : ConsoleCommand<DebugRoomResetTeamCmdParams>
	{
		// Token: 0x060017DA RID: 6106 RVA: 0x00062D49 File Offset: 0x00061149
		public DebugRoomResetTeamCmd(IGameRoomManager roomManager)
		{
			this.m_roomManager = roomManager;
		}

		// Token: 0x060017DB RID: 6107 RVA: 0x00062D58 File Offset: 0x00061158
		protected override void Execute(DebugRoomResetTeamCmdParams param)
		{
			IGameRoom room = this.m_roomManager.GetRoom(param.RoomId);
			if (room != null)
			{
				bool autoBalance = false;
				room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
				{
					autoBalance = r.Autobalance;
				});
				if (autoBalance)
				{
					room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						if (param.ProfileId != null)
						{
							RoomPlayer player = r.GetPlayer(param.ProfileId.Value);
							if (player != null)
							{
								player.TeamID = 0;
							}
							else
							{
								Log.Info<ulong, ulong>("Can't find player {0} in room {1}", param.ProfileId.Value, param.RoomId);
							}
						}
						else
						{
							foreach (RoomPlayer roomPlayer in r.Players)
							{
								roomPlayer.TeamID = 0;
							}
						}
					});
				}
				else
				{
					Log.Info<ulong>("Autobalanace is disabled in room {0}", param.RoomId);
				}
			}
			else
			{
				Log.Info<ulong>("Room {0} doesn't exist", param.RoomId);
			}
		}

		// Token: 0x04000B84 RID: 2948
		private readonly IGameRoomManager m_roomManager;
	}
}
