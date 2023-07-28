using System;
using MasterServer.DAL.CustomRules;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002D6 RID: 726
	[CustomRuleStateSerializer("scheduled_reward", 1, typeof(ScheduledRewardsRule))]
	internal class ScheduledRewardsRuleStateSerializer : ICustomRuleStateSerializer
	{
		// Token: 0x06000F89 RID: 3977 RVA: 0x0003EB7C File Offset: 0x0003CF7C
		public CustomRuleRawState Serialize(object stateobj)
		{
			ScheduledRewardsRuleState scheduledRewardsRuleState = (ScheduledRewardsRuleState)stateobj;
			return new CustomRuleRawState
			{
				Key = scheduledRewardsRuleState.Key
			};
		}

		// Token: 0x06000F8A RID: 3978 RVA: 0x0003EBA4 File Offset: 0x0003CFA4
		public CustomRuleState Deserialize(CustomRuleRawState rawState)
		{
			return new ScheduledRewardsRuleState
			{
				Key = rawState.Key,
				LastActivationTime = rawState.LastUpdateTimeUtc
			};
		}
	}
}
