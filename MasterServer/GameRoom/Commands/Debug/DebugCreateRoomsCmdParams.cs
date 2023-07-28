using System;
using CommandLine;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x0200045E RID: 1118
	internal class DebugCreateRoomsCmdParams : DebugRoomPlayersParams
	{
		// Token: 0x17000235 RID: 565
		// (get) Token: 0x0600179F RID: 6047 RVA: 0x00062880 File Offset: 0x00060C80
		// (set) Token: 0x060017A0 RID: 6048 RVA: 0x00062888 File Offset: 0x00060C88
		[Option('m', "mission", HelpText = "Mission name or id used to filter and set room's mission", MutuallyExclusiveSet = "mission_name")]
		public string Mission { get; set; }

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x060017A1 RID: 6049 RVA: 0x00062891 File Offset: 0x00060C91
		// (set) Token: 0x060017A2 RID: 6050 RVA: 0x00062899 File Offset: 0x00060C99
		[Option('g', "game_mode", HelpText = "Game mode used to filter and set room's mission (default is 'tdm')", DefaultValue = "tdm", MutuallyExclusiveSet = "game_mode")]
		public string GameMode { get; set; }

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x060017A3 RID: 6051 RVA: 0x000628A2 File Offset: 0x00060CA2
		// (set) Token: 0x060017A4 RID: 6052 RVA: 0x000628AA File Offset: 0x00060CAA
		[Option('t', "room_type", HelpText = "Room type')", Required = true)]
		public GameRoomType RoomType { get; set; }
	}
}
