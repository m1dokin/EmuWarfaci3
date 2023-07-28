using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using MasterServer.GameLogic.ItemsSystem.RandomBoxChoiceLimitation;
using MasterServer.GameLogic.SpecialProfileRewards;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002C3 RID: 707
	internal class ConsecutiveLoginBonusRewardSet
	{
		// Token: 0x06000F1C RID: 3868 RVA: 0x0003C964 File Offset: 0x0003AD64
		private ConsecutiveLoginBonusRewardSet(List<List<string>> rewards)
		{
			this.m_rewards = rewards;
		}

		// Token: 0x06000F1D RID: 3869 RVA: 0x0003C974 File Offset: 0x0003AD74
		[Obsolete("Used for unit testing purposes only!")]
		public bool IsValid(ISpecialProfileRewardService profileRewardService, IRandomBoxChoiceLimitationService choiceLimitation)
		{
			if (!this.IsAllStreakContainReward())
			{
				return false;
			}
			IEnumerable<string> rewardSetsWhichDoesNotContainSingleReward = this.GetRewardSetsWhichDoesNotContainSingleReward(profileRewardService);
			if (rewardSetsWhichDoesNotContainSingleReward != null && rewardSetsWhichDoesNotContainSingleReward.Any<string>())
			{
				return false;
			}
			IEnumerable<Tuple<string, string>> source = this.FindRegularItemsInRewards(choiceLimitation, profileRewardService);
			return !source.Any<Tuple<string, string>>();
		}

		// Token: 0x06000F1E RID: 3870 RVA: 0x0003C9C0 File Offset: 0x0003ADC0
		public bool IsAllStreakContainReward()
		{
			bool result;
			if (this.m_rewards.Any<List<string>>())
			{
				IEnumerable<List<string>> rewards = this.m_rewards;
				if (ConsecutiveLoginBonusRewardSet.<>f__mg$cache0 == null)
				{
					ConsecutiveLoginBonusRewardSet.<>f__mg$cache0 = new Func<List<string>, bool>(Enumerable.Any<string>);
				}
				result = rewards.All(ConsecutiveLoginBonusRewardSet.<>f__mg$cache0);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000F1F RID: 3871 RVA: 0x0003CA00 File Offset: 0x0003AE00
		public IEnumerable<Tuple<string, string>> FindRegularItemsInRewards(IRandomBoxChoiceLimitationService choiceLimitation, ISpecialProfileRewardService profileRewardService)
		{
			List<Tuple<string, string>> list = new List<Tuple<string, string>>();
			IEnumerable<RewardSet> enumerable = this.m_rewards.SelectMany((List<string> set) => set).Select(new Func<string, RewardSet>(profileRewardService.GetRewardSet));
			foreach (RewardSet rewardSet in enumerable)
			{
				foreach (string text in rewardSet.Prizes())
				{
					if (choiceLimitation.IsRegularItemInBox(text))
					{
						list.Add(new Tuple<string, string>(rewardSet.Name, text));
					}
				}
			}
			return list;
		}

		// Token: 0x06000F20 RID: 3872 RVA: 0x0003CAF8 File Offset: 0x0003AEF8
		public IEnumerable<string> GetRewardSetsWhichDoesNotContainSingleReward(ISpecialProfileRewardService profileRewardService)
		{
			List<string> list = new List<string>();
			foreach (List<string> source in this.m_rewards)
			{
				List<string> collection = (from reward in source
				where !ConsecutiveLoginBonusRewardSet.OnlyOneRewardInSet(reward, profileRewardService)
				select reward).ToList<string>();
				list.AddRange(collection);
			}
			return list;
		}

		// Token: 0x06000F21 RID: 3873 RVA: 0x0003CB84 File Offset: 0x0003AF84
		private static bool OnlyOneRewardInSet(string setName, ISpecialProfileRewardService profileRewardService)
		{
			if (profileRewardService == null || string.IsNullOrEmpty(setName))
			{
				throw new ArgumentException("Method's parameter is null or empty");
			}
			RewardSet rewardSet = profileRewardService.GetRewardSet(setName);
			return rewardSet != null && rewardSet.Count() == 1;
		}

		// Token: 0x06000F22 RID: 3874 RVA: 0x0003CBC8 File Offset: 0x0003AFC8
		public ConsecutiveLoginBonusRewardSet.Result GetNextReward(int prevStreak, int prevReward, bool expired)
		{
			bool flag = prevStreak >= this.m_rewards.Count - 1;
			if (flag)
			{
				prevStreak = this.m_rewards.Count - 1;
			}
			bool flag2 = prevStreak == this.m_rewards.Count - 1;
			bool flag3 = prevReward >= this.m_rewards[prevStreak].Count - 1;
			bool flag4 = prevReward >= this.m_rewards[prevStreak].Count || prevReward < 0;
			int num = (!expired && !flag3 && !flag4) ? (prevReward + 1) : 0;
			int num2 = ((!flag3 || !flag2) && flag3) ? (prevStreak + 1) : prevStreak;
			return new ConsecutiveLoginBonusRewardSet.Result
			{
				Reward = this.m_rewards[num2][num],
				CurrStreak = num2,
				CurrReward = num,
				InputWasInvalidated = (flag || flag4)
			};
		}

		// Token: 0x06000F23 RID: 3875 RVA: 0x0003CCCC File Offset: 0x0003B0CC
		public static ConsecutiveLoginBonusRewardSet Parse(XmlElement source)
		{
			List<List<string>> list = new List<List<string>>();
			IEnumerator enumerator = source.GetElementsByTagName("streak").GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						List<string> list2 = new List<string>();
						list.Add(list2);
						XmlElement xmlElement = xmlNode as XmlElement;
						IEnumerator enumerator2 = xmlElement.GetElementsByTagName("reward").GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								XmlNode xmlNode2 = (XmlNode)obj2;
								XmlAttribute xmlAttribute = xmlNode2.Attributes["name"];
								if (xmlAttribute != null)
								{
									list2.Add(xmlAttribute.Value);
								}
							}
						}
						finally
						{
							IDisposable disposable;
							if ((disposable = (enumerator2 as IDisposable)) != null)
							{
								disposable.Dispose();
							}
						}
					}
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
			return new ConsecutiveLoginBonusRewardSet(list);
		}

		// Token: 0x06000F24 RID: 3876 RVA: 0x0003CDE4 File Offset: 0x0003B1E4
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (List<string> values in this.m_rewards)
			{
				stringBuilder.AppendLine("Streak:");
				stringBuilder.AppendLine(string.Join(",", values));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x040006F5 RID: 1781
		private readonly List<List<string>> m_rewards;

		// Token: 0x040006F6 RID: 1782
		[CompilerGenerated]
		private static Func<List<string>, bool> <>f__mg$cache0;

		// Token: 0x020002C4 RID: 708
		public struct Result
		{
			// Token: 0x040006F8 RID: 1784
			public string Reward;

			// Token: 0x040006F9 RID: 1785
			public int CurrStreak;

			// Token: 0x040006FA RID: 1786
			public int CurrReward;

			// Token: 0x040006FB RID: 1787
			public bool InputWasInvalidated;
		}
	}
}
