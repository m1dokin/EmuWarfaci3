using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MasterServer.Core;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005B8 RID: 1464
	internal class SurvivalRewardPool : IRewardPool
	{
		// Token: 0x06001F6F RID: 8047 RVA: 0x0007FC9C File Offset: 0x0007E09C
		public bool TryParse(XmlTextReader reader)
		{
			this.m_stageRewards.Clear();
			bool flag = reader.Name.Equals("RewardPools", StringComparison.InvariantCultureIgnoreCase);
			while (flag && reader.Read())
			{
				XmlNodeType nodeType = reader.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType == XmlNodeType.EndElement)
					{
						if (reader.LocalName.Equals("RewardPools", StringComparison.InvariantCultureIgnoreCase))
						{
							flag = this.IsValid();
							if (!flag)
							{
								this.m_stageRewards.Clear();
							}
							this.m_lastStage = ((this.m_stageRewards.Keys.Count <= 0) ? 0U : this.m_stageRewards.Keys.Max<uint>());
							return flag;
						}
					}
				}
				else
				{
					uint key;
					flag &= uint.TryParse(reader.GetAttribute("name"), out key);
					uint value;
					flag &= uint.TryParse(reader.GetAttribute("value"), out value);
					this.m_stageRewards.Add(key, value);
				}
			}
			return flag;
		}

		// Token: 0x06001F70 RID: 8048 RVA: 0x0007FD9C File Offset: 0x0007E19C
		public void Combine(SurvivalRewardPool pool)
		{
			uint num = (this.m_lastStage <= 0U) ? this.m_lastStage : (this.m_lastStage + 1U);
			foreach (KeyValuePair<uint, uint> keyValuePair in pool.m_stageRewards)
			{
				this.m_stageRewards.Add(num + keyValuePair.Key, keyValuePair.Value);
			}
			this.m_lastStage = ((this.m_stageRewards.Keys.Count <= 0) ? 0U : this.m_stageRewards.Keys.Max<uint>());
		}

		// Token: 0x06001F71 RID: 8049 RVA: 0x0007FE60 File Offset: 0x0007E260
		public void Dump()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<uint, uint> keyValuePair in this.m_stageRewards)
			{
				stringBuilder.AppendFormat("Stage - {0}, Reward - {1} \n", keyValuePair.Key, keyValuePair.Value);
			}
			Log.Info(stringBuilder.ToString());
		}

		// Token: 0x06001F72 RID: 8050 RVA: 0x0007FEEC File Offset: 0x0007E2EC
		public bool IsValid()
		{
			int count = this.m_stageRewards.Count;
			uint num = 0U;
			while ((ulong)num < (ulong)((long)count))
			{
				if (!this.m_stageRewards.ContainsKey(num))
				{
					return false;
				}
				num += 1U;
			}
			return true;
		}

		// Token: 0x06001F73 RID: 8051 RVA: 0x0007FF2D File Offset: 0x0007E32D
		public void ToDefault()
		{
			this.m_stageRewards.Clear();
		}

		// Token: 0x06001F74 RID: 8052 RVA: 0x0007FF3C File Offset: 0x0007E33C
		public uint CalculateReward(uint firstStage, uint lastStage, string missionName, SessionOutcome outcome)
		{
			if (firstStage > lastStage)
			{
				throw new ArgumentException("First stage can't be bigger than last stage");
			}
			Log.Info<string, uint, uint>("Calculating rewards for mission {0}, stages ({1} - {2})", missionName, firstStage, lastStage);
			uint num = 0U;
			for (uint num2 = firstStage; num2 <= lastStage; num2 += 1U)
			{
				uint num3;
				if (!this.m_stageRewards.TryGetValue(num2, out num3))
				{
					Log.Warning<uint, string>("Can't find rewards for stage {0} in mission {1}", num2, missionName);
				}
				num += num3;
			}
			if (outcome == SessionOutcome.Won && this.m_lastStage > lastStage)
			{
				Log.Warning<string, uint>("Reward pool for mission {0} has more stages than checkpoints, last stage is {1}", missionName, lastStage);
			}
			return num;
		}

		// Token: 0x04000F44 RID: 3908
		private readonly Dictionary<uint, uint> m_stageRewards = new Dictionary<uint, uint>();

		// Token: 0x04000F45 RID: 3909
		private uint m_lastStage;
	}
}
