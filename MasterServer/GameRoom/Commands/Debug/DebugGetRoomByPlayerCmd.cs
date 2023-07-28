using System;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000461 RID: 1121
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "debug_get_room_by_player", Help = "Returns ID of the room where specified player is.")]
	internal class DebugGetRoomByPlayerCmd : ConsoleCommand<DebugGetRoomByPlayerParams>
	{
		// Token: 0x060017AA RID: 6058 RVA: 0x000629E1 File Offset: 0x00060DE1
		public DebugGetRoomByPlayerCmd(IGameRoomManager roomManager)
		{
			this.m_roomManager = roomManager;
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x000629F0 File Offset: 0x00060DF0
		protected override void Execute(DebugGetRoomByPlayerParams param)
		{
			ulong profileId = param.ProfileId;
			IGameRoom roomByPlayer = this.m_roomManager.GetRoomByPlayer(profileId);
			if (roomByPlayer == null)
			{
				Log.Info<ulong>("Player {0} is not in room", profileId);
				return;
			}
			string missionId = string.Empty;
			roomByPlayer.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				MissionExtension extension = r.GetExtension<MissionExtension>();
				missionId = extension.MissionKey;
			});
			Log.Info<ulong, ulong, string>("Player {0} has room id = {1}, mission id = {2}", profileId, roomByPlayer.ID, missionId);
		}

		// Token: 0x04000B6E RID: 2926
		private readonly IGameRoomManager m_roomManager;
	}
}
