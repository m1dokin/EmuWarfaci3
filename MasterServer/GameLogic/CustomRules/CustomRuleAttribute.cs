using System;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002A2 RID: 674
	public class CustomRuleAttribute : Attribute
	{
		// Token: 0x06000E76 RID: 3702 RVA: 0x0003A4AA File Offset: 0x000388AA
		public CustomRuleAttribute(string name)
		{
			this.Name = name;
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06000E77 RID: 3703 RVA: 0x0003A4B9 File Offset: 0x000388B9
		// (set) Token: 0x06000E78 RID: 3704 RVA: 0x0003A4C1 File Offset: 0x000388C1
		public string Name { get; private set; }
	}
}
