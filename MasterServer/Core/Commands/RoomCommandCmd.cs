using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Common;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.Core.Commands
{
	// Token: 0x02000107 RID: 263
	[ConsoleCmdAttributes(CmdName = "room_command", Help = "Broadcast command to all players in room")]
	internal class RoomCommandCmd : ConsoleCommand<RoomCommandParams>
	{
		// Token: 0x06000447 RID: 1095 RVA: 0x000127A9 File Offset: 0x00010BA9
		public RoomCommandCmd(IGameRoomManager gameRoomManager, IQueryManager queryManager)
		{
			this.m_gameRoomManager = gameRoomManager;
			this.m_queryManager = queryManager;
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x000127C0 File Offset: 0x00010BC0
		protected override void Execute(RoomCommandParams param)
		{
			IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(param.ProfileId);
			if (roomByPlayer == null)
			{
				Log.Info<ulong>("Room for player {0} can't be found", param.ProfileId);
				return;
			}
			IEnumerable<string> receivers = Enumerable.Empty<string>();
			roomByPlayer.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				receivers = (from x in r.Players
				select x.OnlineID).ToList<string>();
			});
			this.m_queryManager.BroadcastRequest("room_command", receivers.ToList<string>(), new object[]
			{
				param.Command
			});
		}

		// Token: 0x040001C2 RID: 450
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x040001C3 RID: 451
		private readonly IQueryManager m_queryManager;
	}
}
