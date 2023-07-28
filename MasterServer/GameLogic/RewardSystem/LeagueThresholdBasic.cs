using System;
using System.Collections.Generic;
using System.Text;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x0200059B RID: 1435
	public class LeagueThresholdBasic : ICloneable
	{
		// Token: 0x1700032F RID: 815
		public uint this[League key]
		{
			get
			{
				uint result;
				if (!this.m_leagueThresholds.TryGetValue(key, out result))
				{
					throw new Exception(string.Format("League {0} doesn't have threshold", key));
				}
				return result;
			}
			set
			{
				this.m_leagueThresholds[key] = value;
			}
		}

		// Token: 0x06001EE5 RID: 7909 RVA: 0x0007D4D2 File Offset: 0x0007B8D2
		public bool IsValid()
		{
			return this.m_leagueThresholds[League.BRONZE] < this.m_leagueThresholds[League.SILVER] && this.m_leagueThresholds[League.SILVER] < this.m_leagueThresholds[League.GOLD];
		}

		// Token: 0x06001EE6 RID: 7910 RVA: 0x0007D510 File Offset: 0x0007B910
		public bool IsThresholdPassed(League league, uint oldValue, uint newValue)
		{
			uint num;
			return this.m_leagueThresholds.TryGetValue(league, out num) && oldValue < num && num <= newValue;
		}

		// Token: 0x06001EE7 RID: 7911 RVA: 0x0007D544 File Offset: 0x0007B944
		public static LeagueThresholdBasic operator +(LeagueThresholdBasic a, LeagueThresholdBasic b)
		{
			LeagueThresholdBasic leagueThresholdBasic = (LeagueThresholdBasic)a.Clone();
			List<League> list = new List<League>
			{
				League.BRONZE,
				League.SILVER,
				League.GOLD
			};
			foreach (League key in list)
			{
				uint num;
				a.m_leagueThresholds.TryGetValue(key, out num);
				uint num2;
				b.m_leagueThresholds.TryGetValue(key, out num2);
				leagueThresholdBasic[key] = num + num2;
			}
			return leagueThresholdBasic;
		}

		// Token: 0x06001EE8 RID: 7912 RVA: 0x0007D5EC File Offset: 0x0007B9EC
		public LeagueThresholdBasic Decrease(uint value)
		{
			LeagueThresholdBasic leagueThresholdBasic = new LeagueThresholdBasic();
			foreach (KeyValuePair<League, uint> keyValuePair in this.m_leagueThresholds)
			{
				leagueThresholdBasic[keyValuePair.Key] = keyValuePair.Value - value;
			}
			return leagueThresholdBasic;
		}

		// Token: 0x06001EE9 RID: 7913 RVA: 0x0007D660 File Offset: 0x0007BA60
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<League, uint> keyValuePair in this.m_leagueThresholds)
			{
				stringBuilder.AppendFormat("{0} - {1}\n", keyValuePair.Key, keyValuePair.Value);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001EEA RID: 7914 RVA: 0x0007D6E8 File Offset: 0x0007BAE8
		public bool TryGetValue(League league, out uint threshold)
		{
			return this.m_leagueThresholds.TryGetValue(league, out threshold);
		}

		// Token: 0x06001EEB RID: 7915 RVA: 0x0007D6F8 File Offset: 0x0007BAF8
		public object Clone()
		{
			return new LeagueThresholdBasic
			{
				m_leagueThresholds = new Dictionary<League, uint>(this.m_leagueThresholds)
			};
		}

		// Token: 0x04000F0E RID: 3854
		public const uint PerformanceRevertBase = 4194304U;

		// Token: 0x04000F0F RID: 3855
		private Dictionary<League, uint> m_leagueThresholds = new Dictionary<League, uint>();
	}
}
