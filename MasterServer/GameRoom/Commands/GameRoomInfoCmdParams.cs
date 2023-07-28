using System;
using CommandLine;

namespace MasterServer.GameRoom.Commands
{
	// Token: 0x0200046B RID: 1131
	internal class GameRoomInfoCmdParams
	{
		// Token: 0x17000250 RID: 592
		// (get) Token: 0x060017E6 RID: 6118 RVA: 0x0006313C File Offset: 0x0006153C
		// (set) Token: 0x060017E7 RID: 6119 RVA: 0x00063144 File Offset: 0x00061544
		[Option('r', "room_id", Required = true, HelpText = "Room's id we want to get info from.")]
		public ulong RoomId { get; set; }

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x060017E8 RID: 6120 RVA: 0x0006314D File Offset: 0x0006154D
		// (set) Token: 0x060017E9 RID: 6121 RVA: 0x00063155 File Offset: 0x00061555
		[Option('e', "dump_equipment", Required = false, DefaultValue = false, HelpText = "Flag to enable equipment dump.")]
		public bool DumpEquipment { get; set; }
	}
}
