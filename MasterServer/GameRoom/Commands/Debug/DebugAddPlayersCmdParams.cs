using System;
using CommandLine;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000455 RID: 1109
	internal class DebugAddPlayersCmdParams : DebugRoomPlayersParams
	{
		// Token: 0x1700022C RID: 556
		// (get) Token: 0x0600177D RID: 6013 RVA: 0x00061F63 File Offset: 0x00060363
		// (set) Token: 0x0600177E RID: 6014 RVA: 0x00061F6B File Offset: 0x0006036B
		[Option('r', "room_id", Required = true, HelpText = "Room's id we want to add players to.")]
		public ulong RoomId { get; set; }
	}
}
