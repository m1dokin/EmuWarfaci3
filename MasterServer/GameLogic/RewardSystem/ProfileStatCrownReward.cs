using System;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005A0 RID: 1440
	internal class ProfileStatCrownReward
	{
		// Token: 0x06001EF5 RID: 7925 RVA: 0x0007DA65 File Offset: 0x0007BE65
		public ProfileStatCrownReward(uint stat, ulong reward, ulong statValue)
		{
			this.Stat = stat;
			this.Reward = reward;
			this.StatValue = statValue;
		}

		// Token: 0x06001EF6 RID: 7926 RVA: 0x0007DA84 File Offset: 0x0007BE84
		public static ProfileStatCrownReward operator +(ProfileStatCrownReward a, ProfileStatCrownReward b)
		{
			if (a.Stat != b.Stat)
			{
				throw new CrownRewardException(string.Format("Can't sum reward for different stats({0} and {1})", a.Stat, b.Stat));
			}
			if (a.StatValue != b.StatValue)
			{
				throw new CrownRewardException(string.Format("Can't sum reward for different statValue ({0} and {1})", a.StatValue, b.StatValue));
			}
			return new ProfileStatCrownReward(a.Stat, a.Reward + b.Reward, a.StatValue);
		}

		// Token: 0x04000F15 RID: 3861
		public readonly uint Stat;

		// Token: 0x04000F16 RID: 3862
		public readonly ulong Reward;

		// Token: 0x04000F17 RID: 3863
		public readonly ulong StatValue;
	}
}
