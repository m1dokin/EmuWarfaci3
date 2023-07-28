using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services.Configuration;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SkillSystem;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000629 RID: 1577
	internal class SessionSummaryLogReporter
	{
		// Token: 0x060021D0 RID: 8656 RVA: 0x0008AEE0 File Offset: 0x000892E0
		public SessionSummaryLogReporter(ISessionSummaryService summaryService, ILogService logService, IDALService dalService, ICatalogService catalogService, ISkillService skillService, IClanService clanService, IConfigurationService configurationService)
		{
			this.m_summaryService = summaryService;
			this.m_logService = logService;
			this.m_dalService = dalService;
			this.m_catalogService = catalogService;
			this.m_skillService = skillService;
			this.m_clanService = clanService;
			this.m_configurationService = configurationService;
			this.m_summaryService.SessionSummaryFinalized += this.WriteSessionSummaryToLog;
		}

		// Token: 0x060021D1 RID: 8657 RVA: 0x0008AF40 File Offset: 0x00089340
		private string CombinePlayerPlaytimeResults(IEnumerable<SessionSummary.PlayerPlaytime> playerPlaytime)
		{
			if (playerPlaytime.Any<SessionSummary.PlayerPlaytime>())
			{
				StringBuilder stringBuilder = new StringBuilder();
				string arg = string.Empty;
				foreach (IGrouping<string, SessionSummary.PlayerPlaytime> grouping in from x in playerPlaytime
				group x by x.Class)
				{
					stringBuilder.AppendFormat("{0}{1}={2}", arg, ProfileProgressionInfo.ClassNameLongToShort(grouping.Key), grouping.Sum((SessionSummary.PlayerPlaytime p) => p.Playtime));
					arg = ";";
				}
				return stringBuilder.ToString();
			}
			return "NA";
		}

		// Token: 0x060021D2 RID: 8658 RVA: 0x0008B018 File Offset: 0x00089418
		private void WriteSessionSummaryToLog(SessionSummary data)
		{
			try
			{
				uint num = 0U;
				uint num2 = 0U;
				uint num3 = 0U;
				foreach (SessionSummary.PlayerData playerData2 in data.Players.Values)
				{
					TimeSpan playerTime = TimeSpan.FromSeconds((double)(from x in playerData2.Playtimes
					select x.Playtime).Sum());
					string playerClass = this.CombinePlayerPlaytimeResults(playerData2.Playtimes);
					SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(playerData2.ProfileId);
					ClanInfo clanInfoByPid = this.m_clanService.GetClanInfoByPid(playerData2.ProfileId);
					ulong gameMoney = this.GetGameMoney(profileInfo.UserID);
					uint num4 = 0U;
					uint num5 = 0U;
					uint num6 = 0U;
					uint meleeCount = 0U;
					uint headshotCount = 0U;
					uint fragCount = 0U;
					uint claymoreCount = 0U;
					uint buttCount = 0U;
					uint slideCount = 0U;
					uint firstCheckpoint = 0U;
					uint lastCheckpoint = 0U;
					foreach (KeyValuePair<string, string> keyValuePair in playerData2.Stats)
					{
						string key = keyValuePair.Key;
						if (key == "coins_used")
						{
							num4 = uint.Parse(keyValuePair.Value);
							num += num4;
						}
						else if (key == "kill")
						{
							num5 = uint.Parse(keyValuePair.Value);
							num2 += num5;
						}
						else if (key == "melee")
						{
							meleeCount = uint.Parse(keyValuePair.Value);
						}
						else if (key == "headshot")
						{
							headshotCount = uint.Parse(keyValuePair.Value);
						}
						else if (key == "frag")
						{
							fragCount = uint.Parse(keyValuePair.Value);
						}
						else if (key == "claymore")
						{
							claymoreCount = uint.Parse(keyValuePair.Value);
						}
						else if (key == "weapon_butt")
						{
							buttCount = uint.Parse(keyValuePair.Value);
						}
						else if (key == "slide_kill")
						{
							slideCount = uint.Parse(keyValuePair.Value);
						}
						else if (key == "death")
						{
							num6 = uint.Parse(keyValuePair.Value);
							num3 += num6;
						}
						else if (key == "first_checkpoint")
						{
							firstCheckpoint = uint.Parse(keyValuePair.Value);
						}
						else if (key == "last_checkpoint")
						{
							lastCheckpoint = uint.Parse(keyValuePair.Value);
						}
					}
					uint num7 = 0U;
					uint num8 = 0U;
					uint gainedSponsorPoint = 0U;
					uint sponsorId = 0U;
					uint num9 = 0U;
					SRewardMultiplier empty = SRewardMultiplier.Empty;
					foreach (SessionSummary.PlayerReward playerReward in playerData2.Rewards)
					{
						if (playerReward.Name == "experience")
						{
							num7 = uint.Parse(playerReward.Value);
						}
						else if (playerReward.Name == "game_money")
						{
							num8 = uint.Parse(playerReward.Value);
						}
						else if (playerReward.Name == "clan_points")
						{
							num9 = uint.Parse(playerReward.Value);
						}
						else if (playerReward.Name == "sponsor_points")
						{
							gainedSponsorPoint = uint.Parse(playerReward.Value);
							sponsorId = uint.Parse(playerReward.Attrs["sponsor"]);
						}
						else if (playerReward.Name == "experience_dynamic_multiplier")
						{
							empty.ExperienceMultiplier = float.Parse(playerReward.Value);
						}
						else if (playerReward.Name == "money_dynamic_multiplier")
						{
							empty.MoneyMultiplier = float.Parse(playerReward.Value);
						}
						else if (playerReward.Name == "sponsor_points_dynamic_multiplier")
						{
							empty.SponsorPointsMultiplier = float.Parse(playerReward.Value);
						}
						else if (playerReward.Name == "crown_dynamic_multiplier")
						{
							empty.CrownMultiplier = float.Parse(playerReward.Value);
						}
						else if (playerReward.Name == "dynamic_multiplier_providers")
						{
							empty.ProviderID = playerReward.Value;
						}
					}
					int sessionStatus = 0;
					switch (playerData2.OverallResult)
					{
					case SessionOutcome.Won:
						sessionStatus = 1;
						break;
					case SessionOutcome.Lost:
						sessionStatus = 0;
						break;
					case SessionOutcome.Draw:
						goto IL_4EB;
					case SessionOutcome.DNF:
						sessionStatus = 2;
						break;
					case SessionOutcome.Stopped:
						sessionStatus = 3;
						break;
					default:
						goto IL_4EB;
					}
					IL_4F3:
					SkillType skillTypeByRoomType = SkillTypeHelper.GetSkillTypeByRoomType(data.RoomType);
					Skill skill = this.m_skillService.GetSkill(profileInfo.Id, skillTypeByRoomType);
					uint skill2 = (uint)skill.Value;
					this.m_logService.Event.SessionDetailsExLog(profileInfo.UserID, data.SessionId, data.StartTime, playerTime, data.Duration, data.RoomType, data.SubMode, (ulong)num7, profileInfo.RankInfo.Points, (long)((ulong)num8), gameMoney, playerData2.Rank, playerData2.ProfileId, data.RoomId, data.RoomName, sponsorId, gainedSponsorPoint, data.Mission.name, data.Mission.setting ?? string.Empty, data.Mission.difficulty, data.Mission.missionType.Name, sessionStatus, data.Mode, playerClass, num4, num5, num6, skill2, headshotCount, meleeCount, fragCount, claymoreCount, buttCount, slideCount, empty.ExperienceMultiplier.ToString(CultureInfo.InvariantCulture), empty.MoneyMultiplier.ToString(CultureInfo.InvariantCulture), empty.SponsorPointsMultiplier.ToString(CultureInfo.InvariantCulture), empty.CrownMultiplier.ToString(CultureInfo.InvariantCulture), empty.ProviderID, firstCheckpoint, lastCheckpoint, skill.Type, playerData2.FirstWin, (clanInfoByPid == null) ? 0UL : clanInfoByPid.ClanID, (clanInfoByPid == null) ? string.Empty : clanInfoByPid.Name, (clanInfoByPid == null) ? 0U : num9);
					continue;
					IL_4EB:
					sessionStatus = 0;
					goto IL_4F3;
				}
				foreach (SessionSummary.ObserverData observerData in data.Observers.Values)
				{
					TimeSpan playerTime2 = TimeSpan.FromSeconds((double)observerData.Playtime);
					SProfileInfo profileInfo2 = this.m_dalService.ProfileSystem.GetProfileInfo(observerData.ProfileId);
					this.m_logService.Event.SessionDetailsObserverExLog(profileInfo2.UserID, data.SessionId, data.StartTime, playerTime2, data.Duration, data.RoomType, data.SubMode, profileInfo2.RankInfo.Points, observerData.Rank, observerData.ProfileId, data.RoomId, data.RoomName, data.Mission.name, data.Mission.setting ?? string.Empty, data.Mission.difficulty, data.Mission.missionType.Name, data.Mode);
				}
				if (data.Players.Count > 0)
				{
					SRewardMultiplier missionTypeMultiplier = ApplyMultipliersCalculator.GetMissionTypeMultiplier(this.m_configurationService, data.Mission.missionType.Name);
					List<int> source = (from team in (from playtime in data.Players.Values.SelectMany((SessionSummary.PlayerData playerData) => playerData.Playtimes)
					select playtime.TeamId).Distinct<int>()
					select new
					{
						TeamId = team,
						RoundsCount = data.Rounds.Count((SessionSummary.RoundData r) => r.WinningTeamId == team)
					} into rpt
					orderby rpt.TeamId
					select rpt.RoundsCount).ToList<int>();
					Exception e;
					List<int> source2 = (from playerData in data.Players.Values
					select new
					{
						playerData = playerData,
						kills = playerData.Stats.GetValueOrDefault("kill", 0) - playerData.Stats.GetValueOrDefault("kill_friendly", 0)
					} into <>__TranspIdent9
					where <>__TranspIdent9.playerData.Playtimes.Any<SessionSummary.PlayerPlaytime>()
					select new
					{
						<>__TranspIdent9 = <>__TranspIdent9,
						mainTeam = (from playtime in <>__TranspIdent9.playerData.Playtimes
						group playtime by playtime.TeamId into grouping
						select new
						{
							TeamId = grouping.Key,
							PlayTime = grouping.Sum((SessionSummary.PlayerPlaytime e) => e.Playtime)
						} into g
						orderby g.PlayTime
						select g).Last()
					} into <>__TranspIdent10
					select new
					{
						TeamId = <>__TranspIdent10.mainTeam.TeamId,
						Kills = <>__TranspIdent10.<>__TranspIdent9.kills
					} into playerKills
					group playerKills by playerKills.TeamId into grouping
					select new
					{
						TeamId = grouping.Key,
						TotalKills = grouping.Sum(g => g.Kills)
					} into kpt
					orderby kpt.TeamId
					select kpt.TotalKills).ToList<int>();
					this.m_logService.Event.SessionTotalExLog(data.RoomType, data.SubMode, data.StartTime, data.Duration, data.SessionId, data.RoomId, data.RoomName, data.Players.Count, Math.Max(1, data.Mission.subLevels.Count), data.Mission.levelGraph ?? string.Empty, data.Mode, data.Mission.name, data.Mission.setting ?? string.Empty, data.Mission.difficulty, data.Mission.missionType.Name, num3, num, missionTypeMultiplier.ExperienceMultiplier.ToString(CultureInfo.InvariantCulture), missionTypeMultiplier.MoneyMultiplier.ToString(CultureInfo.InvariantCulture), missionTypeMultiplier.SponsorPointsMultiplier.ToString(CultureInfo.InvariantCulture), data.IsAutobalanced, source.FirstOrDefault<int>(), source.LastOrDefault<int>(), source2.FirstOrDefault<int>(), source2.LastOrDefault<int>());
				}
			}
			catch (Exception e)
			{
				Log.Warning("Failed to log rewards.");
				Exception e;
				Log.Warning(e);
			}
		}

		// Token: 0x060021D3 RID: 8659 RVA: 0x0008BC78 File Offset: 0x0008A078
		private ulong GetGameMoney(ulong userId)
		{
			return this.m_catalogService.GetCustomerAccount(userId, Currency.GameMoney).Money;
		}

		// Token: 0x04001095 RID: 4245
		private readonly ISessionSummaryService m_summaryService;

		// Token: 0x04001096 RID: 4246
		private readonly ILogService m_logService;

		// Token: 0x04001097 RID: 4247
		private readonly IDALService m_dalService;

		// Token: 0x04001098 RID: 4248
		private readonly ICatalogService m_catalogService;

		// Token: 0x04001099 RID: 4249
		private readonly ISkillService m_skillService;

		// Token: 0x0400109A RID: 4250
		private IConfigurationService m_configurationService;

		// Token: 0x0400109B RID: 4251
		private readonly IClanService m_clanService;
	}
}
