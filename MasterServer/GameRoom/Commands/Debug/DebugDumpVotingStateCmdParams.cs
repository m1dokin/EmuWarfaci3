using System;
using CommandLine;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000460 RID: 1120
	internal class DebugDumpVotingStateCmdParams
	{
		// Token: 0x17000238 RID: 568
		// (get) Token: 0x060017A8 RID: 6056 RVA: 0x000629D0 File Offset: 0x00060DD0
		// (set) Token: 0x060017A9 RID: 6057 RVA: 0x000629D8 File Offset: 0x00060DD8
		[Option('r', "room_id", Required = true, HelpText = "Room's id we want to get voting state from.")]
		public ulong RoomId { get; set; }
	}
}
