using System;
using CommandLine;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000458 RID: 1112
	internal class DebugChangePlayersStatusCmdParams
	{
		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06001784 RID: 6020 RVA: 0x00062166 File Offset: 0x00060566
		// (set) Token: 0x06001785 RID: 6021 RVA: 0x0006216E File Offset: 0x0006056E
		[Option('p', "profile_id", Required = true, HelpText = "Player's id we want to change players status.", MutuallyExclusiveSet = "profile")]
		public ulong ProfileId { get; set; }

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06001786 RID: 6022 RVA: 0x00062177 File Offset: 0x00060577
		// (set) Token: 0x06001787 RID: 6023 RVA: 0x0006217F File Offset: 0x0006057F
		[Option('r', "room_id", Required = true, HelpText = "Room's id we want to change players status in.", MutuallyExclusiveSet = "room")]
		public ulong RoomId { get; set; }

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x06001788 RID: 6024 RVA: 0x00062188 File Offset: 0x00060588
		// (set) Token: 0x06001789 RID: 6025 RVA: 0x00062190 File Offset: 0x00060590
		[Option('a', "affected_players", Required = true, HelpText = "Which players are affected (1 - all, 2 - all except master).", MutuallyExclusiveSet = "room")]
		public byte PlayersAffected { get; set; }

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x0600178A RID: 6026 RVA: 0x00062199 File Offset: 0x00060599
		// (set) Token: 0x0600178B RID: 6027 RVA: 0x000621A1 File Offset: 0x000605A1
		[Option('s', "status", Required = true, HelpText = "Players's status we want to set (2 - can't be ready, 1 - ready, or 0 - not ready).")]
		public byte Status { get; set; }
	}
}
