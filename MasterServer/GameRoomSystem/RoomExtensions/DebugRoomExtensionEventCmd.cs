using System;
using MasterServer.Common;
using MasterServer.Core;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020004B0 RID: 1200
	[ConsoleCmdAttributes(CmdName = "debug_room_extension_event", ArgsSize = 3, Help = "Enable/Dissable events handling: room id, event name,  ")]
	internal class DebugRoomExtensionEventCmd : IConsoleCmd
	{
		// Token: 0x06001981 RID: 6529 RVA: 0x000678C6 File Offset: 0x00065CC6
		public DebugRoomExtensionEventCmd(IGameRoomManager gameRoomManager)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x06001982 RID: 6530 RVA: 0x000678D8 File Offset: 0x00065CD8
		public void ExecuteCmd(string[] args)
		{
			ulong roomId = ulong.Parse(args[1]);
			string eventName = args[2];
			bool enable = int.Parse(args[3]) > 0;
			IGameRoom room = this.m_gameRoomManager.GetRoom(roomId);
			if (room == null)
			{
				Log.Warning<ulong>("Room {0} doesn't exist", roomId);
				return;
			}
			room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				DebugRoomExtension extension = r.GetExtension<DebugRoomExtension>();
				extension.SetEventEnabled(eventName, enable);
				Log.Info<string, ulong, string>("Event {0} in room {1} is {2}", eventName, roomId, (!extension.IsEventEnabled(eventName)) ? "disabled" : "enabled");
			});
		}

		// Token: 0x04000C2E RID: 3118
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
