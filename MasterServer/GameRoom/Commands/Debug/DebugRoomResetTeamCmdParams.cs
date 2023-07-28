using System;
using CommandLine;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000469 RID: 1129
	internal class DebugRoomResetTeamCmdParams
	{
		// Token: 0x1700024E RID: 590
		// (get) Token: 0x060017DD RID: 6109 RVA: 0x00062F18 File Offset: 0x00061318
		// (set) Token: 0x060017DE RID: 6110 RVA: 0x00062F20 File Offset: 0x00061320
		[Option('r', "room_id", Required = true, HelpText = "Room's id we want to reset team id in autobalance room in.")]
		public ulong RoomId { get; set; }

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x060017DF RID: 6111 RVA: 0x00062F29 File Offset: 0x00061329
		// (set) Token: 0x060017E0 RID: 6112 RVA: 0x00062F31 File Offset: 0x00061331
		[Option('p', "profile_id", HelpText = "Player's id we want to reset team id in autobalance room.")]
		public ulong? ProfileId { get; set; }
	}
}
