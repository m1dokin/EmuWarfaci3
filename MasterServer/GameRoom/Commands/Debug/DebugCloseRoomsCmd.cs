using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x0200045B RID: 1115
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "debug_close_rooms", Help = "Closes all the rooms described by their ids.")]
	internal class DebugCloseRoomsCmd : ConsoleCommand<DebugCloseRoomsCmdParams>
	{
		// Token: 0x06001796 RID: 6038 RVA: 0x00062631 File Offset: 0x00060A31
		public DebugCloseRoomsCmd(IDebugGameRoomActivator roomActivatorDebug, IGameRoomManager roomManager, IUserRepository userRepository)
		{
			this.m_roomActivatorDebug = roomActivatorDebug;
			this.m_roomManager = roomManager;
			this.m_userRepository = userRepository;
		}

		// Token: 0x06001797 RID: 6039 RVA: 0x00062650 File Offset: 0x00060A50
		protected override void Execute(DebugCloseRoomsCmdParams param)
		{
			List<RoomPlayer> removedPlayers = new List<RoomPlayer>();
			foreach (ulong num in param.RoomIds)
			{
				IGameRoom room = this.m_roomManager.GetRoom(num);
				if (room != null)
				{
					room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						removedPlayers.AddRange(r.Players);
					});
					this.m_roomActivatorDebug.CloseRooms(new IGameRoom[]
					{
						room
					});
				}
				else
				{
					Log.Error<ulong>("[DebugCloseRoomsCmd] Room {0} doesnot exist", num);
				}
			}
			foreach (UserInfo.User user in from pl in removedPlayers
			select this.m_userRepository.GetUserByUserId(pl.UserID) into userInfo
			where userInfo != null
			select userInfo)
			{
				this.m_userRepository.UserLogout(user, ELogoutType.Logout);
			}
		}

		// Token: 0x04000B5E RID: 2910
		private readonly IDebugGameRoomActivator m_roomActivatorDebug;

		// Token: 0x04000B5F RID: 2911
		private readonly IGameRoomManager m_roomManager;

		// Token: 0x04000B60 RID: 2912
		private readonly IUserRepository m_userRepository;
	}
}
