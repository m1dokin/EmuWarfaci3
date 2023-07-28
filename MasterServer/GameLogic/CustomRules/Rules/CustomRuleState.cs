using System;
using MasterServer.DAL.CustomRules;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002CA RID: 714
	public abstract class CustomRuleState
	{
		// Token: 0x06000F3A RID: 3898 RVA: 0x0000C92C File Offset: 0x0000AD2C
		protected CustomRuleState()
		{
			this.LastActivationTime = DateTime.MinValue;
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06000F3B RID: 3899 RVA: 0x0000C93F File Offset: 0x0000AD3F
		// (set) Token: 0x06000F3C RID: 3900 RVA: 0x0000C947 File Offset: 0x0000AD47
		public CustomRuleRawState.KeyData Key { get; set; }

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000F3D RID: 3901 RVA: 0x0000C950 File Offset: 0x0000AD50
		// (set) Token: 0x06000F3E RID: 3902 RVA: 0x0000C958 File Offset: 0x0000AD58
		public DateTime LastActivationTime { get; set; }

		// Token: 0x06000F3F RID: 3903 RVA: 0x0000C964 File Offset: 0x0000AD64
		public override string ToString()
		{
			return string.Format("CustomRuleState: Rule Id: {0}, Profile ID {1}, Time:{2}", this.Key.RuleID, this.Key.ProfileID, this.LastActivationTime);
		}
	}
}
