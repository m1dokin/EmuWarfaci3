using System;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002B0 RID: 688
	public class CustomRuleStateInfo
	{
		// Token: 0x17000182 RID: 386
		// (get) Token: 0x06000EC8 RID: 3784 RVA: 0x0003B616 File Offset: 0x00039A16
		// (set) Token: 0x06000EC9 RID: 3785 RVA: 0x0003B61E File Offset: 0x00039A1E
		public string Name { get; set; }

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000ECA RID: 3786 RVA: 0x0003B627 File Offset: 0x00039A27
		// (set) Token: 0x06000ECB RID: 3787 RVA: 0x0003B62F File Offset: 0x00039A2F
		public byte TypeID { get; set; }

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06000ECC RID: 3788 RVA: 0x0003B638 File Offset: 0x00039A38
		// (set) Token: 0x06000ECD RID: 3789 RVA: 0x0003B640 File Offset: 0x00039A40
		public Type StateType { get; set; }
	}
}
