using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005A1 RID: 1441
	internal class ProfileCrownReward : IEnumerable<ProfileStatCrownReward>, IEnumerable
	{
		// Token: 0x06001EF7 RID: 7927 RVA: 0x0007DB1D File Offset: 0x0007BF1D
		public ProfileCrownReward()
		{
			this.m_crownRewards = new List<ProfileStatCrownReward>();
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06001EF8 RID: 7928 RVA: 0x0007DB30 File Offset: 0x0007BF30
		public int Count
		{
			get
			{
				return this.m_crownRewards.Count;
			}
		}

		// Token: 0x06001EF9 RID: 7929 RVA: 0x0007DB40 File Offset: 0x0007BF40
		public void Add(ProfileStatCrownReward reward)
		{
			int num = this.m_crownRewards.FindIndex((ProfileStatCrownReward x) => x.Stat == reward.Stat);
			if (num == -1)
			{
				this.m_crownRewards.Add(reward);
			}
			else
			{
				List<ProfileStatCrownReward> crownRewards;
				int index;
				(crownRewards = this.m_crownRewards)[index = num] = crownRewards[index] + reward;
			}
		}

		// Token: 0x06001EFA RID: 7930 RVA: 0x0007DBB1 File Offset: 0x0007BFB1
		public ulong GetAccumulatedReward()
		{
			return this.m_crownRewards.Aggregate(0UL, (ulong current, ProfileStatCrownReward x) => current + x.Reward);
		}

		// Token: 0x06001EFB RID: 7931 RVA: 0x0007DBDD File Offset: 0x0007BFDD
		IEnumerator<ProfileStatCrownReward> IEnumerable<ProfileStatCrownReward>.GetEnumerator()
		{
			return this.m_crownRewards.GetEnumerator();
		}

		// Token: 0x06001EFC RID: 7932 RVA: 0x0007DBEF File Offset: 0x0007BFEF
		public IEnumerator GetEnumerator()
		{
			return this.m_crownRewards.GetEnumerator();
		}

		// Token: 0x04000F18 RID: 3864
		private readonly List<ProfileStatCrownReward> m_crownRewards;
	}
}
