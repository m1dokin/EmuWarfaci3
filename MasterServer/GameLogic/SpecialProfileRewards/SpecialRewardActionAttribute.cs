using System;

namespace MasterServer.GameLogic.SpecialProfileRewards
{
	// Token: 0x020005C9 RID: 1481
	public class SpecialRewardActionAttribute : Attribute
	{
		// Token: 0x06001FAA RID: 8106 RVA: 0x00081348 File Offset: 0x0007F748
		public SpecialRewardActionAttribute(string name)
		{
			this.Name = name;
		}

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x06001FAB RID: 8107 RVA: 0x00081357 File Offset: 0x0007F757
		// (set) Token: 0x06001FAC RID: 8108 RVA: 0x0008135F File Offset: 0x0007F75F
		public string Name { get; private set; }
	}
}
