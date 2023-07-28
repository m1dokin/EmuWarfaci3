using System;
using CommandLine;

namespace MasterServer.Core.Commands
{
	// Token: 0x02000108 RID: 264
	internal class RoomCommandParams
	{
		// Token: 0x17000092 RID: 146
		// (get) Token: 0x0600044A RID: 1098 RVA: 0x00012891 File Offset: 0x00010C91
		// (set) Token: 0x0600044B RID: 1099 RVA: 0x00012899 File Offset: 0x00010C99
		[Option('p', "profile", HelpText = "profile id to find game room")]
		public ulong ProfileId { get; set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x0600044C RID: 1100 RVA: 0x000128A2 File Offset: 0x00010CA2
		// (set) Token: 0x0600044D RID: 1101 RVA: 0x000128AA File Offset: 0x00010CAA
		[Option('c', "command", HelpText = "command to execute for room player")]
		public string Command { get; set; }
	}
}
