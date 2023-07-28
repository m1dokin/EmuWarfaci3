using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MasterServer.Common;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x0200059C RID: 1436
	public class CrownRewardThreshold
	{
		// Token: 0x06001EEC RID: 7916 RVA: 0x0007D71D File Offset: 0x0007BB1D
		public CrownRewardThreshold()
		{
			this.m_categoryThresholds = new Dictionary<CrownRewardThreshold.PerformanceCategory, LeagueThresholdBasic>();
		}

		// Token: 0x06001EED RID: 7917 RVA: 0x0007D730 File Offset: 0x0007BB30
		public bool TryGetThreshold(CrownRewardThreshold.PerformanceCategory performanceCategory, out LeagueThresholdBasic threshold)
		{
			return this.m_categoryThresholds.TryGetValue(performanceCategory, out threshold);
		}

		// Token: 0x06001EEE RID: 7918 RVA: 0x0007D740 File Offset: 0x0007BB40
		public bool TryParse(XmlReader reader)
		{
			this.m_categoryThresholds.Clear();
			bool flag = true;
			while (flag && reader.Read())
			{
				XmlNodeType nodeType = reader.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType == XmlNodeType.EndElement)
					{
						if (reader.LocalName.Equals("CrownRewardsThresholds", StringComparison.InvariantCultureIgnoreCase))
						{
							flag = this.IsValid();
							if (!flag)
							{
								this.m_categoryThresholds.Clear();
							}
							return flag;
						}
					}
				}
				else
				{
					string localName = reader.LocalName;
					CrownRewardThreshold.PerformanceCategory performanceCategory;
					flag &= Utils.TryParse<CrownRewardThreshold.PerformanceCategory>(localName, out performanceCategory);
					uint num;
					flag &= uint.TryParse(reader.GetAttribute("bronze"), out num);
					uint num2;
					flag &= uint.TryParse(reader.GetAttribute("silver"), out num2);
					uint num3;
					flag &= uint.TryParse(reader.GetAttribute("gold"), out num3);
					bool flag2 = performanceCategory == CrownRewardThreshold.PerformanceCategory.Time;
					LeagueThresholdBasic leagueThresholdBasic = new LeagueThresholdBasic();
					leagueThresholdBasic[League.BRONZE] = ((!flag2) ? num : (4194304U - num));
					leagueThresholdBasic[League.SILVER] = ((!flag2) ? num2 : (4194304U - num2));
					leagueThresholdBasic[League.GOLD] = ((!flag2) ? num3 : (4194304U - num3));
					this.m_categoryThresholds.Add(performanceCategory, leagueThresholdBasic);
				}
			}
			if (flag)
			{
				flag = this.IsValid();
			}
			if (!flag)
			{
				this.m_categoryThresholds.Clear();
			}
			return flag;
		}

		// Token: 0x06001EEF RID: 7919 RVA: 0x0007D8AA File Offset: 0x0007BCAA
		public bool IsValid()
		{
			return this.m_categoryThresholds.Aggregate(this.m_categoryThresholds.Count > 0, (bool current, KeyValuePair<CrownRewardThreshold.PerformanceCategory, LeagueThresholdBasic> categoryThreshold) => current & categoryThreshold.Value.IsValid());
		}

		// Token: 0x06001EF0 RID: 7920 RVA: 0x0007D8E4 File Offset: 0x0007BCE4
		public static CrownRewardThreshold operator +(CrownRewardThreshold a, CrownRewardThreshold b)
		{
			CrownRewardThreshold crownRewardThreshold = new CrownRewardThreshold();
			IEnumerator enumerator = Enum.GetValues(typeof(CrownRewardThreshold.PerformanceCategory)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					CrownRewardThreshold.PerformanceCategory performanceCategory = (CrownRewardThreshold.PerformanceCategory)obj;
					LeagueThresholdBasic a2;
					LeagueThresholdBasic b2;
					if (!a.m_categoryThresholds.TryGetValue(performanceCategory, out a2) || !b.m_categoryThresholds.TryGetValue(performanceCategory, out b2))
					{
						return new CrownRewardThreshold();
					}
					bool flag = performanceCategory == CrownRewardThreshold.PerformanceCategory.Time;
					LeagueThresholdBasic leagueThresholdBasic = a2 + b2;
					if (flag)
					{
						leagueThresholdBasic = leagueThresholdBasic.Decrease(4194304U);
					}
					crownRewardThreshold.m_categoryThresholds[performanceCategory] = leagueThresholdBasic;
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return crownRewardThreshold;
		}

		// Token: 0x06001EF1 RID: 7921 RVA: 0x0007D9BC File Offset: 0x0007BDBC
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<CrownRewardThreshold.PerformanceCategory, LeagueThresholdBasic> keyValuePair in this.m_categoryThresholds)
			{
				stringBuilder.AppendLine(keyValuePair.Key.ToString());
				stringBuilder.AppendFormat("\t{0}", keyValuePair.Value);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04000F10 RID: 3856
		private readonly Dictionary<CrownRewardThreshold.PerformanceCategory, LeagueThresholdBasic> m_categoryThresholds;

		// Token: 0x0200059D RID: 1437
		public enum PerformanceCategory
		{
			// Token: 0x04000F13 RID: 3859
			TotalPerformance,
			// Token: 0x04000F14 RID: 3860
			Time = 5
		}
	}
}
