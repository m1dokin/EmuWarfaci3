using System;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002B2 RID: 690
	public class CustomRuleStateSerializerAttribute : Attribute
	{
		// Token: 0x06000ED0 RID: 3792 RVA: 0x0003B64C File Offset: 0x00039A4C
		public CustomRuleStateSerializerAttribute(string name, byte id, Type type)
		{
			this.StateInfo = new CustomRuleStateInfo
			{
				Name = name,
				TypeID = id,
				StateType = type
			};
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06000ED1 RID: 3793 RVA: 0x0003B681 File Offset: 0x00039A81
		// (set) Token: 0x06000ED2 RID: 3794 RVA: 0x0003B689 File Offset: 0x00039A89
		public CustomRuleStateInfo StateInfo { get; private set; }
	}
}
