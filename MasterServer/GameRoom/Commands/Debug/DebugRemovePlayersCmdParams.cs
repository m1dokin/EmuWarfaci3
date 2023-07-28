using System;
using CommandLine;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000466 RID: 1126
	internal class DebugRemovePlayersCmdParams
	{
		// Token: 0x1700023C RID: 572
		// (get) Token: 0x060017BA RID: 6074 RVA: 0x00062D27 File Offset: 0x00061127
		// (set) Token: 0x060017BB RID: 6075 RVA: 0x00062D2F File Offset: 0x0006112F
		[Option('r', "room_id", Required = true, HelpText = "Room's id we want to remove players from.")]
		public ulong RoomId { get; set; }

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x060017BC RID: 6076 RVA: 0x00062D38 File Offset: 0x00061138
		// (set) Token: 0x060017BD RID: 6077 RVA: 0x00062D40 File Offset: 0x00061140
		[OptionArray('p', "profile_ids", HelpText = "Player's ids to remove from room. Remove all players if empty/not specified.")]
		public ulong[] ProfileIds { get; set; }
	}
}
