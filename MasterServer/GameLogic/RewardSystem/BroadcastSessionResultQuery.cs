using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.RatingSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020007BC RID: 1980
	[QueryAttributes(TagName = "broadcast_session_result")]
	internal class BroadcastSessionResultQuery : BaseQuery
	{
		// Token: 0x06002898 RID: 10392 RVA: 0x000AEAC0 File Offset: 0x000ACEC0
		public BroadcastSessionResultQuery(IRatingSeasonService ratingSeasonService)
		{
			this.m_ratingSeasonService = ratingSeasonService;
		}

		// Token: 0x06002899 RID: 10393 RVA: 0x000AEAD0 File Offset: 0x000ACED0
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "BroadcastSessionResultQuery"))
			{
				IEnumerable<string> enumerable = (IEnumerable<string>)queryParams[0];
				IEnumerable<RewardUpdateData> enumerable2 = queryParams[1] as IEnumerable<RewardUpdateData>;
				string text = string.Empty;
				foreach (string str in enumerable)
				{
					if (text.Length != 0)
					{
						text += ',';
					}
					text += str;
				}
				request.SetAttribute("bcast_receivers", text);
				foreach (RewardUpdateData rewardUpdateData in enumerable2)
				{
					XmlElement xmlElement = request.OwnerDocument.CreateElement("player_result");
					ulong accumulatedReward = rewardUpdateData.rewards.crownReward.GetAccumulatedReward();
					Rating playerRating = this.m_ratingSeasonService.GetPlayerRating(rewardUpdateData.rewards.profileId);
					uint winStreakBonus = rewardUpdateData.rewards.winStreakBonus;
					xmlElement.SetAttribute("nickname", rewardUpdateData.nickname);
					xmlElement.SetAttribute("experience", rewardUpdateData.rewards.gainedExp.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("pvp_rating_points", playerRating.Points.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("pvp_rating_win_streak_bonus", winStreakBonus.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("money", rewardUpdateData.rewards.gainedMoney.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("gained_crown_money", accumulatedReward.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("no_crown_rewards", (rewardUpdateData.rewards.crownReward.Count != 0) ? "0" : "1");
					xmlElement.SetAttribute("sponsor_points", rewardUpdateData.rewards.gainedSponsorPoints.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("clan_points", rewardUpdateData.rewards.gainedClanPoints.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("bonus_experience", rewardUpdateData.rewards.bonusExp.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("bonus_money", rewardUpdateData.rewards.bonusMoney.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("bonus_sponsor_points", rewardUpdateData.rewards.bonusSponsorPoints.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("experience_boost", rewardUpdateData.rewards.gainedExpBooster.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("money_boost", rewardUpdateData.rewards.gainedMoneyBooster.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("sponsor_points_boost", rewardUpdateData.rewards.gainedSponsorPointsBooster.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("experience_boost_percent", rewardUpdateData.rewards.percentExpBooster.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("money_boost_percent", rewardUpdateData.rewards.percentMoneyBooster.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("sponsor_points_boost_percent", rewardUpdateData.rewards.percentSponsorPointsBooster.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("completed_stages", rewardUpdateData.rewards.completedStages.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("is_vip", (!rewardUpdateData.rewards.isVip) ? "0" : "1");
					xmlElement.SetAttribute("score", rewardUpdateData.rewards.score.ToString(CultureInfo.InvariantCulture));
					xmlElement.SetAttribute("first_win", (!rewardUpdateData.rewards.firstWin) ? "0" : "1");
					byte[] bytes = Encoding.UTF8.GetBytes(rewardUpdateData.rewards.dynamicMultiplier.Description);
					string value = Convert.ToBase64String(bytes);
					xmlElement.SetAttribute("dynamic_multipliers_info", value);
					xmlElement.SetAttribute("dynamic_crown_multiplier", rewardUpdateData.rewards.dynamicMultiplier.CrownMultiplier.ToString(CultureInfo.InvariantCulture));
					XmlElement newChild = rewardUpdateData.rewards.progression.ToXml(xmlElement.OwnerDocument, true);
					xmlElement.AppendChild(newChild);
					request.AppendChild(xmlElement);
				}
				RewardUpdateData rewardUpdateData2 = enumerable2.FirstOrDefault((RewardUpdateData x) => x.rewards.crownReward.Count > 0);
				if (rewardUpdateData2 != null)
				{
					XmlElement xmlElement2 = request.OwnerDocument.CreateElement("players_performance");
					IEnumerator enumerator3 = rewardUpdateData2.rewards.crownReward.GetEnumerator();
					try
					{
						while (enumerator3.MoveNext())
						{
							object obj = enumerator3.Current;
							ProfileStatCrownReward profileStatCrownReward = (ProfileStatCrownReward)obj;
							XmlElement xmlElement3 = request.OwnerDocument.CreateElement("stat");
							xmlElement3.SetAttribute("id", profileStatCrownReward.Stat.ToString(CultureInfo.InvariantCulture));
							xmlElement3.SetAttribute("value", profileStatCrownReward.StatValue.ToString(CultureInfo.InvariantCulture));
							xmlElement2.AppendChild(xmlElement3);
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator3 as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
					request.AppendChild(xmlElement2);
				}
			}
		}

		// Token: 0x0400158B RID: 5515
		private readonly IRatingSeasonService m_ratingSeasonService;
	}
}
