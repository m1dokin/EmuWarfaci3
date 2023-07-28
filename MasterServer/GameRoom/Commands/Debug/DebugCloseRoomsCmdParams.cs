using System;
using CommandLine;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x0200045C RID: 1116
	internal class DebugCloseRoomsCmdParams
	{
		// Token: 0x17000234 RID: 564
		// (get) Token: 0x0600179A RID: 6042 RVA: 0x000627B4 File Offset: 0x00060BB4
		// (set) Token: 0x0600179B RID: 6043 RVA: 0x000627BC File Offset: 0x00060BBC
		[OptionArray('r', "room_ids", Required = true, HelpText = "Room id list to close.")]
		public ulong[] RoomIds { get; set; }
	}
}
