using System;
using CommandLine;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000053 RID: 83
	internal class ShopPurchaseItemCmdParams
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000140 RID: 320 RVA: 0x00009B42 File Offset: 0x00007F42
		// (set) Token: 0x06000141 RID: 321 RVA: 0x00009B4A File Offset: 0x00007F4A
		[Option('u', "userId", Required = true, HelpText = "userId")]
		public ulong UserId { get; set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000142 RID: 322 RVA: 0x00009B53 File Offset: 0x00007F53
		// (set) Token: 0x06000143 RID: 323 RVA: 0x00009B5B File Offset: 0x00007F5B
		[Option('s', "supplierId", Required = true, HelpText = "supplierId")]
		public int SupplierId { get; set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000144 RID: 324 RVA: 0x00009B64 File Offset: 0x00007F64
		// (set) Token: 0x06000145 RID: 325 RVA: 0x00009B6C File Offset: 0x00007F6C
		[Option('o', "offerId", Required = true, HelpText = "offerId")]
		public ulong OfferId { get; set; }
	}
}
