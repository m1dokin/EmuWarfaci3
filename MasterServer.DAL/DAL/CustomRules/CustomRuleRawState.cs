using System;

namespace MasterServer.DAL.CustomRules
{
	// Token: 0x02000029 RID: 41
	[Serializable]
	public class CustomRuleRawState
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000063 RID: 99 RVA: 0x0000336A File Offset: 0x0000176A
		// (set) Token: 0x06000064 RID: 100 RVA: 0x00003372 File Offset: 0x00001772
		public CustomRuleRawState.KeyData Key { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000065 RID: 101 RVA: 0x0000337B File Offset: 0x0000177B
		// (set) Token: 0x06000066 RID: 102 RVA: 0x00003383 File Offset: 0x00001783
		public DateTime LastUpdateTimeUtc { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000067 RID: 103 RVA: 0x0000338C File Offset: 0x0000178C
		// (set) Token: 0x06000068 RID: 104 RVA: 0x00003394 File Offset: 0x00001794
		public byte RuleType { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000069 RID: 105 RVA: 0x0000339D File Offset: 0x0000179D
		// (set) Token: 0x0600006A RID: 106 RVA: 0x000033A5 File Offset: 0x000017A5
		public byte[] Data { get; set; }

		// Token: 0x0200002A RID: 42
		[Serializable]
		public struct KeyData
		{
			// Token: 0x0600006B RID: 107 RVA: 0x000033AE File Offset: 0x000017AE
			public KeyData(ulong profileId, ulong ruleId, uint version)
			{
				this.ProfileID = profileId;
				this.RuleID = ruleId;
				this.Version = version;
			}

			// Token: 0x0400006C RID: 108
			public ulong ProfileID;

			// Token: 0x0400006D RID: 109
			public ulong RuleID;

			// Token: 0x0400006E RID: 110
			public uint Version;
		}
	}
}
