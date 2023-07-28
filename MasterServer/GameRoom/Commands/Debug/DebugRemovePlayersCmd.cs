using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000465 RID: 1125
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "debug_remove_players", Help = "Removes specific (or all) players from a certain room.")]
	internal class DebugRemovePlayersCmd : ConsoleCommand<DebugRemovePlayersCmdParams>
	{
		// Token: 0x060017B6 RID: 6070 RVA: 0x00062B2A File Offset: 0x00060F2A
		public DebugRemovePlayersCmd(IGameRoomManager roomManager, IUserRepository userRepository)
		{
			this.m_roomManager = roomManager;
			this.m_userRepository = userRepository;
		}

		// Token: 0x060017B7 RID: 6071 RVA: 0x00062B40 File Offset: 0x00060F40
		protected override void Execute(DebugRemovePlayersCmdParams param)
		{
			IGameRoom room = this.m_roomManager.GetRoom(param.RoomId);
			if (room != null)
			{
				List<RoomPlayer> removedPlayers = null;
				room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					removedPlayers = (from p in r.Players
					where param.ProfileIds == null || param.ProfileIds.Contains(p.ProfileID)
					select p).ToList<RoomPlayer>();
					r.RemoveFakePlayers(from p in removedPlayers
					select p.ProfileID);
				});
				foreach (UserInfo.User user in from pl in removedPlayers
				select this.m_userRepository.GetUserByUserId(pl.ProfileID) into userInfo
				where userInfo != null
				select userInfo)
				{
					this.m_userRepository.UserLogout(user, ELogoutType.Logout);
				}
			}
			else
			{
				Log.Info<ulong>("Room {0} doesn't exist.", param.RoomId);
			}
		}

		// Token: 0x04000B73 RID: 2931
		private readonly IGameRoomManager m_roomManager;

		// Token: 0x04000B74 RID: 2932
		private readonly IUserRepository m_userRepository;
	}
}
