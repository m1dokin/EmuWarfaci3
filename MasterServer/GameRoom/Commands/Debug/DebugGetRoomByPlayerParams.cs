using System;
using CommandLine;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000462 RID: 1122
	internal class DebugGetRoomByPlayerParams
	{
		// Token: 0x17000239 RID: 569
		// (get) Token: 0x060017AD RID: 6061 RVA: 0x00062A90 File Offset: 0x00060E90
		// (set) Token: 0x060017AE RID: 6062 RVA: 0x00062A98 File Offset: 0x00060E98
		[Option('p', "profile_id", Required = true, HelpText = "Profile id of player we want to get room for.")]
		public ulong ProfileId { get; set; }
	}
}
