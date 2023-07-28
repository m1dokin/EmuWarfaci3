using System;

namespace MasterServer.DAL.CustomRules
{
	// Token: 0x02000026 RID: 38
	[Serializable]
	public class CustomRuleInfo
	{
		// Token: 0x04000060 RID: 96
		public ulong RuleID;

		// Token: 0x04000061 RID: 97
		public CustomRuleInfo.RuleSource Source;

		// Token: 0x04000062 RID: 98
		public DateTime CreatedAtUTC;

		// Token: 0x04000063 RID: 99
		public bool Enabled;

		// Token: 0x04000064 RID: 100
		public string Data;

		// Token: 0x02000027 RID: 39
		public enum RuleSource
		{
			// Token: 0x04000066 RID: 102
			Static,
			// Token: 0x04000067 RID: 103
			Dynamic
		}
	}
}
