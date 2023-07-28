using System;
using CommandLine;

namespace MasterServer.GameLogic.RewardSystem.ConsoleCommands
{
	// Token: 0x020000CD RID: 205
	internal class GetMissionTypeMultipliersCmdParams
	{
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000351 RID: 849 RVA: 0x0000F3AF File Offset: 0x0000D7AF
		// (set) Token: 0x06000352 RID: 850 RVA: 0x0000F3B7 File Offset: 0x0000D7B7
		[Option('t', "type", Required = true, HelpText = "Mission type")]
		public string MissionType { get; set; }
	}
}
