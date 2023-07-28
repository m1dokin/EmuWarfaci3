using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.SponsorSystem
{
	// Token: 0x020007C2 RID: 1986
	internal class UnlockItemInfo
	{
		// Token: 0x060028B8 RID: 10424 RVA: 0x000B0873 File Offset: 0x000AEC73
		internal UnlockItemInfo(ulong userId, string itemName, ILogGroup logGroup)
		{
			this.UserId = userId;
			this.ItemName = itemName;
			this.LoggingGroup = logGroup;
		}

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x060028B9 RID: 10425 RVA: 0x000B0890 File Offset: 0x000AEC90
		// (set) Token: 0x060028BA RID: 10426 RVA: 0x000B0898 File Offset: 0x000AEC98
		internal ulong UserId { get; private set; }

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x060028BB RID: 10427 RVA: 0x000B08A1 File Offset: 0x000AECA1
		// (set) Token: 0x060028BC RID: 10428 RVA: 0x000B08A9 File Offset: 0x000AECA9
		internal string ItemName { get; private set; }

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x060028BD RID: 10429 RVA: 0x000B08B2 File Offset: 0x000AECB2
		// (set) Token: 0x060028BE RID: 10430 RVA: 0x000B08BA File Offset: 0x000AECBA
		internal ILogGroup LoggingGroup { get; private set; }
	}
}
