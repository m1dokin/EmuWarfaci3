using System;
using CommandLine;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003CE RID: 974
	internal class CleanUpNotificationParams
	{
		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x0600156C RID: 5484 RVA: 0x0005A13A File Offset: 0x0005853A
		// (set) Token: 0x0600156D RID: 5485 RVA: 0x0005A142 File Offset: 0x00058542
		[Option('p', "profile_id", Required = true, HelpText = "profile id")]
		public ulong ProfileId { get; set; }
	}
}
