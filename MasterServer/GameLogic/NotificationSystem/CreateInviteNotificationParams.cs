using System;
using CommandLine;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003D1 RID: 977
	internal class CreateInviteNotificationParams
	{
		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06001573 RID: 5491 RVA: 0x0005A2A6 File Offset: 0x000586A6
		// (set) Token: 0x06001574 RID: 5492 RVA: 0x0005A2AE File Offset: 0x000586AE
		[Option('i', "initiator", Required = true, HelpText = "initiator nickname")]
		public string Initiator { get; set; }

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06001575 RID: 5493 RVA: 0x0005A2B7 File Offset: 0x000586B7
		// (set) Token: 0x06001576 RID: 5494 RVA: 0x0005A2BF File Offset: 0x000586BF
		[Option('t', "target", Required = true, HelpText = "target nickname")]
		public string Target { get; set; }

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06001577 RID: 5495 RVA: 0x0005A2C8 File Offset: 0x000586C8
		// (set) Token: 0x06001578 RID: 5496 RVA: 0x0005A2D0 File Offset: 0x000586D0
		[Option('c', "count", Required = true, HelpText = "count of notification to create")]
		public uint Count { get; set; }
	}
}
