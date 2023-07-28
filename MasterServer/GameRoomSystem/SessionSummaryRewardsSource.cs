using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.Telemetry;
using OLAPHypervisor;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200062A RID: 1578
	internal class SessionSummaryRewardsSource : IDisposable
	{
		// Token: 0x060021EA RID: 8682 RVA: 0x0008BEDC File Offset: 0x0008A2DC
		public SessionSummaryRewardsSource(ISessionSummaryService summaryService, IRewardService rewardService, ITelemetryService telemetryService)
		{
			this.m_summaryService = summaryService;
			this.m_rewardService = rewardService;
			this.m_telemetryService = telemetryService;
			this.m_rewardService.OnRewardsGiven += this.OnRewardsGiven;
		}

		// Token: 0x060021EB RID: 8683 RVA: 0x0008BF10 File Offset: 0x0008A310
		public void Dispose()
		{
			this.m_rewardService.OnRewardsGiven -= this.OnRewardsGiven;
		}

		// Token: 0x060021EC RID: 8684 RVA: 0x0008BF2C File Offset: 0x0008A32C
		private void OnRewardsGiven(string sessionid, List<RewardUpdateData> usersRewards)
		{
			RewardUpdateData[] rewards = usersRewards.ToArray();
			this.m_summaryService.Contribute(sessionid, "Rewards summary", delegate(SessionSummary summ)
			{
				this.OnRewardsSummary(summ, rewards);
			});
		}

		// Token: 0x060021ED RID: 8685 RVA: 0x0008BF6F File Offset: 0x0008A36F
		private void OnRewardsSummary(SessionSummary summary, RewardUpdateData[] rewards)
		{
			this.ReportRewardStatisticsToTelemetry(summary, rewards);
			this.ReportRewardStatisticsToSummary(summary, rewards);
			summary.RewardsContributed = true;
		}

		// Token: 0x060021EE RID: 8686 RVA: 0x0008BF88 File Offset: 0x0008A388
		private void ReportRewardStatisticsToSummary(SessionSummary summaryData, IEnumerable<RewardUpdateData> rewards)
		{
			foreach (RewardUpdateData rewardUpdateData in rewards)
			{
				SessionSummary.PlayerData player = summaryData.GetPlayer(rewardUpdateData.rewards.profileId);
				player.OverallResult = rewardUpdateData.rewards.outcome;
				player.FirstWin = rewardUpdateData.rewards.firstWin;
				if (rewardUpdateData.rewards.gainedExp > 0U)
				{
					player.AddReward("experience", rewardUpdateData.rewards.gainedExp.ToString(), new string[0]);
				}
				if (rewardUpdateData.rewards.percentExpBooster > 0f)
				{
					player.AddReward("experience_booster", rewardUpdateData.rewards.percentExpBooster.ToString(), new string[0]);
				}
				if (rewardUpdateData.rewards.gainedMoney > 0U)
				{
					player.AddReward("game_money", rewardUpdateData.rewards.gainedMoney.ToString(), new string[0]);
				}
				if (rewardUpdateData.rewards.percentMoneyBooster > 0f)
				{
					player.AddReward("game_money_booster", rewardUpdateData.rewards.percentMoneyBooster.ToString(), new string[0]);
				}
				if (rewardUpdateData.rewards.gainedSponsorPoints > 0U)
				{
					player.AddReward("sponsor_points", rewardUpdateData.rewards.gainedSponsorPoints.ToString(), new string[]
					{
						"sponsor",
						rewardUpdateData.rewards.SponsorData.sponsorId.ToString()
					});
				}
				if (rewardUpdateData.rewards.percentSponsorPointsBooster > 0f)
				{
					player.AddReward("sponsor_points_booster", rewardUpdateData.rewards.percentSponsorPointsBooster.ToString(), new string[0]);
				}
				if (rewardUpdateData.rewards.gainedClanPoints > 0U)
				{
					player.AddReward("clan_points", rewardUpdateData.rewards.gainedClanPoints.ToString(), new string[0]);
				}
				if (rewardUpdateData.rewards.isVip)
				{
					player.AddReward("is_vip", "1", new string[0]);
				}
				if (rewardUpdateData.rewards.SponsorData.unlockedItems.Count > 0)
				{
					player.AddReward("unlocks", rewardUpdateData.rewards.SponsorData.unlockedItems.Count.ToString(), new string[0]);
				}
				if (!rewardUpdateData.rewards.dynamicMultiplier.IsEmpty())
				{
					player.AddReward("experience_dynamic_multiplier", rewardUpdateData.rewards.dynamicMultiplier.ExperienceMultiplier.ToString(CultureInfo.InvariantCulture), new string[0]);
					player.AddReward("money_dynamic_multiplier", rewardUpdateData.rewards.dynamicMultiplier.MoneyMultiplier.ToString(CultureInfo.InvariantCulture), new string[0]);
					player.AddReward("sponsor_points_dynamic_multiplier", rewardUpdateData.rewards.dynamicMultiplier.SponsorPointsMultiplier.ToString(CultureInfo.InvariantCulture), new string[0]);
					player.AddReward("crown_dynamic_multiplier", rewardUpdateData.rewards.dynamicMultiplier.CrownMultiplier.ToString(CultureInfo.InvariantCulture), new string[0]);
					player.AddReward("dynamic_multiplier_providers", HttpUtility.UrlEncode(rewardUpdateData.rewards.dynamicMultiplier.ProviderID), new string[0]);
				}
			}
		}

		// Token: 0x060021EF RID: 8687 RVA: 0x0008C32C File Offset: 0x0008A72C
		private void ReportRewardStatisticsToTelemetry(SessionSummary summary, RewardUpdateData[] rewards)
		{
			if (!rewards.Any<RewardUpdateData>())
			{
				return;
			}
			string text = DateTime.Now.ToString("yyyy-MM-dd");
			string name = summary.Mission.name;
			int num = Math.Max(1, summary.Mission.subLevels.Count);
			string text2 = summary.Mission.missionType.Name.ToLower();
			string text3 = SessionSummaryRewardsSource.GameMode.ModeName(summary.Mission.gameMode);
			string text4 = SessionSummaryRewardsSource.GameMode.SubModeName(summary.Mission.gameMode);
			bool flag = false;
			int num2 = 0;
			double num3 = 0.0;
			double num4 = 0.0;
			Dictionary<uint, SessionSummaryRewardsSource.SponsorStatHlp> dictionary = new Dictionary<uint, SessionSummaryRewardsSource.SponsorStatHlp>();
			List<Measure> list = new List<Measure>();
			foreach (RewardUpdateData rewardUpdateData in rewards)
			{
				num2++;
				flag = rewardUpdateData.rewards.isClanWar;
				string text5 = "player_sessions_" + rewardUpdateData.rewards.outcome.ToString().ToLower();
				list.Add(this.m_telemetryService.MakeMeasure(1L, new object[]
				{
					"stat",
					text5,
					"mode",
					text3,
					"submode",
					text4,
					"clan_war",
					(!flag) ? "0" : "1",
					"difficulty",
					text2,
					"profile",
					rewardUpdateData.rewards.profileId
				}));
				list.Add(this.m_telemetryService.MakeMeasure((long)((ulong)rewardUpdateData.rewards.gainedMoney), new object[]
				{
					"stat",
					"player_gained_money",
					"profile",
					rewardUpdateData.rewards.profileId
				}));
				num3 += rewardUpdateData.rewards.gainedExp;
				num4 += rewardUpdateData.rewards.gainedMoney;
				SessionSummaryRewardsSource.SponsorStatHlp sponsorStatHlp;
				if (!dictionary.TryGetValue(rewardUpdateData.rewards.SponsorData.sponsorId, out sponsorStatHlp))
				{
					dictionary.Add(rewardUpdateData.rewards.SponsorData.sponsorId, new SessionSummaryRewardsSource.SponsorStatHlp(1, rewardUpdateData.rewards.SponsorData.unlockedItems.Count, rewardUpdateData.rewards.gainedSponsorPoints));
				}
				else
				{
					dictionary[rewardUpdateData.rewards.SponsorData.sponsorId] = new SessionSummaryRewardsSource.SponsorStatHlp(sponsorStatHlp.users + 1, sponsorStatHlp.unlocks + rewardUpdateData.rewards.SponsorData.unlockedItems.Count, sponsorStatHlp.points + rewardUpdateData.rewards.gainedSponsorPoints);
				}
			}
			num3 /= (double)num2;
			num4 /= (double)num2;
			if (summary.Mission.subLevels.Count == 0)
			{
				this.ReportRewards(num3, num4, text3, (!flag) ? "0" : "1", text4, text2, name, num, null, null, text, dictionary, ref list);
			}
			else
			{
				num3 /= (double)summary.Mission.subLevels.Count;
				num4 /= (double)summary.Mission.subLevels.Count;
				foreach (SubLevel subLevel in summary.Mission.subLevels)
				{
					this.ReportRewards(num3, num4, text3, (!flag) ? "0" : "1", text4, text2, name, num, subLevel.name, subLevel.flow, text, dictionary, ref list);
				}
			}
			foreach (KeyValuePair<uint, SessionSummaryRewardsSource.SponsorStatHlp> keyValuePair in dictionary)
			{
				SessionSummaryRewardsSource.SponsorStatHlp value = keyValuePair.Value;
				list.Add(this.m_telemetryService.MakeMeasure((long)value.unlocks, new object[]
				{
					"stat",
					"map_unlocks",
					"mode",
					text3,
					"clan_war",
					(!flag) ? "0" : "1",
					"submode",
					text4,
					"difficulty",
					text2,
					"map",
					name,
					"map_nparts",
					num,
					"sponsor",
					keyValuePair.Key,
					"date",
					text
				}));
			}
			summary.AddMeasure(list);
		}

		// Token: 0x060021F0 RID: 8688 RVA: 0x0008C828 File Offset: 0x0008AC28
		private void ReportRewards(double avgExp, double avgMoney, string mode, string clanwar, string submode, string missionType, string mapName, int nparts, string part, string flow, string date, Dictionary<uint, SessionSummaryRewardsSource.SponsorStatHlp> avgSp, ref List<Measure> measures)
		{
			measures.Add(this.m_telemetryService.MakeMeasure((long)avgExp, new object[]
			{
				"stat",
				"map_gained_exp_avg",
				"mode",
				mode,
				"clan_war",
				clanwar,
				"submode",
				submode,
				"difficulty",
				missionType,
				"map",
				mapName,
				"map_nparts",
				nparts,
				"part",
				part,
				"flow",
				flow,
				"date",
				date
			}));
			measures.Add(this.m_telemetryService.MakeMeasure((long)avgMoney, new object[]
			{
				"stat",
				"map_gained_money_avg",
				"mode",
				mode,
				"clan_war",
				clanwar,
				"submode",
				submode,
				"difficulty",
				missionType,
				"map",
				mapName,
				"map_nparts",
				nparts,
				"part",
				part,
				"flow",
				flow,
				"date",
				date
			}));
			foreach (KeyValuePair<uint, SessionSummaryRewardsSource.SponsorStatHlp> keyValuePair in avgSp)
			{
				SessionSummaryRewardsSource.SponsorStatHlp value = keyValuePair.Value;
				measures.Add(this.m_telemetryService.MakeMeasure((long)(value.points / (double)value.users / (double)nparts), new object[]
				{
					"stat",
					"map_gained_sponsor_pts_avg",
					"mode",
					mode,
					"clan_war",
					clanwar,
					"submode",
					submode,
					"difficulty",
					missionType,
					"map",
					mapName,
					"map_nparts",
					nparts,
					"part",
					part,
					"flow",
					flow,
					"sponsor",
					keyValuePair.Key,
					"date",
					date
				}));
			}
		}

		// Token: 0x040010B2 RID: 4274
		private readonly ISessionSummaryService m_summaryService;

		// Token: 0x040010B3 RID: 4275
		private readonly IRewardService m_rewardService;

		// Token: 0x040010B4 RID: 4276
		private readonly ITelemetryService m_telemetryService;

		// Token: 0x0200062B RID: 1579
		private static class GameMode
		{
			// Token: 0x060021F1 RID: 8689 RVA: 0x0008CACC File Offset: 0x0008AECC
			public static string ModeName(string mode)
			{
				if (mode.ToUpper() == "PVE")
				{
					return "PVE";
				}
				return "PVP";
			}

			// Token: 0x060021F2 RID: 8690 RVA: 0x0008CAEE File Offset: 0x0008AEEE
			public static string SubModeName(string mode)
			{
				return mode.ToUpper();
			}
		}

		// Token: 0x0200062C RID: 1580
		private struct SponsorStatHlp
		{
			// Token: 0x060021F3 RID: 8691 RVA: 0x0008CAF6 File Offset: 0x0008AEF6
			public SponsorStatHlp(int usr, int unl, uint pts)
			{
				this.users = usr;
				this.unlocks = unl;
				this.points = pts;
			}

			// Token: 0x040010B5 RID: 4277
			public int users;

			// Token: 0x040010B6 RID: 4278
			public int unlocks;

			// Token: 0x040010B7 RID: 4279
			public uint points;
		}
	}
}
