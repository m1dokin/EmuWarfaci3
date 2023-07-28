using System;
using CommandLine;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000464 RID: 1124
	internal class DebugKickRoomPlayerCmdParams
	{
		// Token: 0x1700023A RID: 570
		// (get) Token: 0x060017B2 RID: 6066 RVA: 0x00062B08 File Offset: 0x00060F08
		// (set) Token: 0x060017B3 RID: 6067 RVA: 0x00062B10 File Offset: 0x00060F10
		[Option('p', "profile_id", Required = true, HelpText = "Player's id we want to kick.")]
		public ulong ProfileId { get; set; }

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x060017B4 RID: 6068 RVA: 0x00062B19 File Offset: 0x00060F19
		// (set) Token: 0x060017B5 RID: 6069 RVA: 0x00062B21 File Offset: 0x00060F21
		[Option('r', "reason", Required = true, HelpText = "Reason for kicking player.")]
		public GameRoomPlayerRemoveReason Reason { get; set; }
	}
}
