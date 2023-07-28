using System;
using CommandLine;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x0200045A RID: 1114
	internal class DebugCheckRoomBalanceCmdParams
	{
		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06001790 RID: 6032 RVA: 0x000625FE File Offset: 0x000609FE
		// (set) Token: 0x06001791 RID: 6033 RVA: 0x00062606 File Offset: 0x00060A06
		[Option('r', "room_id", Required = true, HelpText = "Room's id we want to check balance in.")]
		public ulong RoomId { get; set; }

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x06001792 RID: 6034 RVA: 0x0006260F File Offset: 0x00060A0F
		// (set) Token: 0x06001793 RID: 6035 RVA: 0x00062617 File Offset: 0x00060A17
		[Option('a', "all", HelpText = "Affects all/ready players.", DefaultValue = false)]
		public bool All { get; set; }

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06001794 RID: 6036 RVA: 0x00062620 File Offset: 0x00060A20
		// (set) Token: 0x06001795 RID: 6037 RVA: 0x00062628 File Offset: 0x00060A28
		[Option('f', "force", HelpText = "Forces autobalance option of the room.", DefaultValue = false)]
		public bool Force { get; set; }
	}
}
