using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005A2 RID: 1442
	internal class CrownRewardInfo : IEnumerable
	{
		// Token: 0x17000331 RID: 817
		public uint this[League index]
		{
			get
			{
				uint result;
				if (!this.m_rewards.TryGetValue(index, out result))
				{
					throw new CrownRewardException(string.Format("Reward for league {0}, doesn't exist", index));
				}
				return result;
			}
			set
			{
				this.m_rewards[index] = value;
			}
		}

		// Token: 0x06001F01 RID: 7937 RVA: 0x0007DC82 File Offset: 0x0007C082
		public IEnumerator GetEnumerator()
		{
			return this.m_rewards.GetEnumerator();
		}

		// Token: 0x06001F02 RID: 7938 RVA: 0x0007DC94 File Offset: 0x0007C094
		public bool Validate()
		{
			return this.m_rewards.All((KeyValuePair<League, uint> reward) => reward.Value != 0U);
		}

		// Token: 0x04000F1A RID: 3866
		private readonly Dictionary<League, uint> m_rewards = new Dictionary<League, uint>();
	}
}
