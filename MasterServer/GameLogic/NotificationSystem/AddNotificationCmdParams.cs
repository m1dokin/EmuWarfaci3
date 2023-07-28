using System;
using CommandLine;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020000B4 RID: 180
	internal class AddNotificationCmdParams
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060002DF RID: 735 RVA: 0x0000DF18 File Offset: 0x0000C318
		// (set) Token: 0x060002E0 RID: 736 RVA: 0x0000DF20 File Offset: 0x0000C320
		[Option('p', "profileId", Required = true, HelpText = "profileId")]
		public ulong ProfileId { get; set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060002E1 RID: 737 RVA: 0x0000DF29 File Offset: 0x0000C329
		// (set) Token: 0x060002E2 RID: 738 RVA: 0x0000DF31 File Offset: 0x0000C331
		[Option('m', "message", Required = true, HelpText = "message")]
		public string Message { get; set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x0000DF3A File Offset: 0x0000C33A
		// (set) Token: 0x060002E4 RID: 740 RVA: 0x0000DF42 File Offset: 0x0000C342
		[Option('d', "delivery", Required = true, HelpText = "delivery type, possible values: SendNow,SendNowOrLater,SendOnCheckPoint")]
		public EDeliveryType DeliveryType { get; set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060002E5 RID: 741 RVA: 0x0000DF4B File Offset: 0x0000C34B
		// (set) Token: 0x060002E6 RID: 742 RVA: 0x0000DF53 File Offset: 0x0000C353
		[Option('c', "confirmation", Required = true, HelpText = "confirmation type, possible values: None, Confirmation")]
		public EConfirmationType ConfirmationType { get; set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060002E7 RID: 743 RVA: 0x0000DF5C File Offset: 0x0000C35C
		// (set) Token: 0x060002E8 RID: 744 RVA: 0x0000DF64 File Offset: 0x0000C364
		[Option('e', "expiration", Required = true, HelpText = "expiration")]
		public string Expiration { get; set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060002E9 RID: 745 RVA: 0x0000DF6D File Offset: 0x0000C36D
		// (set) Token: 0x060002EA RID: 746 RVA: 0x0000DF75 File Offset: 0x0000C375
		[Option('t', "type", Required = false, HelpText = "notification type", DefaultValue = ENotificationType.Message)]
		public ENotificationType Type { get; set; }
	}
}
