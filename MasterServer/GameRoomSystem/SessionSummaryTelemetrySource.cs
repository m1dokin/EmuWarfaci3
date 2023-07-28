using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.GameLogic.SkillSystem;
using MasterServer.Telemetry;
using StatsDataSource.Storage;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000631 RID: 1585
	internal class SessionSummaryTelemetrySource : IDisposable
	{
		// Token: 0x0600220A RID: 8714 RVA: 0x0008DB20 File Offset: 0x0008BF20
		public SessionSummaryTelemetrySource(ISessionSummaryService summaryService, ITelemetryService telemetryService, ITelemetryStreamService telemetryStreamService)
		{
			this.m_summaryService = summaryService;
			this.m_telemetryService = telemetryService;
			this.m_telemetryStreamService = telemetryStreamService;
			this.m_telemetryStreamService.OnSessionTelemetry += this.OnSessionTelemetry;
		}

		// Token: 0x0600220B RID: 8715 RVA: 0x0008DB54 File Offset: 0x0008BF54
		public void Dispose()
		{
			this.m_telemetryStreamService.OnSessionTelemetry -= this.OnSessionTelemetry;
		}

		// Token: 0x0600220C RID: 8716 RVA: 0x0008DB70 File Offset: 0x0008BF70
		private void OnSessionTelemetry(TelemetryStreamService.SessionData sessionTelemetryData, List<DataUpdate> telemetry)
		{
			this.m_summaryService.Contribute(sessionTelemetryData.SessionID, "Telemetry summary", delegate(SessionSummary summ)
			{
				this.OnTelemetrySummary(summ, sessionTelemetryData, telemetry);
			});
		}

		// Token: 0x0600220D RID: 8717 RVA: 0x0008DBC0 File Offset: 0x0008BFC0
		private void OnTelemetrySummary(SessionSummary data, TelemetryStreamService.SessionData sessionTelemetryData, List<DataUpdate> updates)
		{
			int num = 0;
			while (num != updates.Count)
			{
				DataUpdate dataUpdate = updates[num];
				string text = dataUpdate.Dimensions["stat"];
				if (text == "_ss_info")
				{
					data.Host = dataUpdate.Dimensions["host"];
					data.MasterServer = dataUpdate.Dimensions["masterserver"];
					data.SessionLog = sessionTelemetryData.SessionFile;
					data.Mode = dataUpdate.Dimensions["mode"];
					data.SubMode = dataUpdate.Dimensions["submode"];
					int num2;
					int.TryParse(dataUpdate.Dimensions["clanwar"], out num2);
					data.ClanWar = (num2 != 0);
					data.EndOutcome = dataUpdate.Dimensions["end_outcome"];
					data.EndReason = dataUpdate.Dimensions["end_reason"];
					data.EndWinningTeam = dataUpdate.Dimensions["end_winner"];
				}
				else if (text == "session_time")
				{
					data.SessionTime = (int)(float.Parse(dataUpdate.Value) / 10f);
				}
				else if (text == "_ss_player_playtime")
				{
					ulong @uint = SessionSummaryTelemetrySource.GetUInt64(dataUpdate.Dimensions, "profile");
					SessionSummary.PlayerData player = data.GetPlayer(@uint);
					player.Nickname = dataUpdate.Dimensions["nickname"];
					int rank;
					if (!int.TryParse(dataUpdate.Dimensions["rank"], out rank))
					{
						Log.Warning("Can't parse rank value to write to LogServer.");
					}
					player.Rank = rank;
					double num3;
					if (!double.TryParse(dataUpdate.Dimensions["skill_value"], out num3))
					{
						Log.Warning(string.Format("Can't parse player(pid: {0}) skill value: '{1}' to write to LogServer.", @uint, num3));
					}
					SkillType skillType;
					if (!Enum.TryParse<SkillType>(dataUpdate.Dimensions["skill_type"], true, out skillType))
					{
						Log.Warning(string.Format("Can't parse player(pid: {0}) skill type: '{1}' to write to LogServer.", @uint, skillType));
					}
					player.Skill = new Skill(skillType, num3, 0.0);
					int team;
					int.TryParse(dataUpdate.Dimensions["team"], out team);
					string klass = dataUpdate.Dimensions["class"];
					int playtime = (int)(float.Parse(dataUpdate.Value) / 10f);
					player.AddPlaytime(team, klass, playtime);
				}
				else if (text == "_ss_observer_playtime")
				{
					ulong uint2 = SessionSummaryTelemetrySource.GetUInt64(dataUpdate.Dimensions, "profile");
					SessionSummary.ObserverData observer = data.GetObserver(uint2);
					observer.Nickname = dataUpdate.Dimensions["nickname"];
					int rank2;
					if (!int.TryParse(dataUpdate.Dimensions["rank"], out rank2))
					{
						Log.Warning("Can't parse rank value to write to LogServer.");
					}
					observer.Rank = rank2;
					int playtime2 = (int)(float.Parse(dataUpdate.Value) / 10f);
					observer.AddPlaytime(playtime2);
				}
				else if (text == "_ss_rounds")
				{
					SessionSummary.RoundData roundData = data.AddRound(int.Parse(dataUpdate.Dimensions["id"]));
					ulong uint3 = SessionSummaryTelemetrySource.GetUInt64(dataUpdate.Dimensions, "time_zero");
					ulong uint4 = SessionSummaryTelemetrySource.GetUInt64(dataUpdate.Dimensions, "begin");
					ulong uint5 = SessionSummaryTelemetrySource.GetUInt64(dataUpdate.Dimensions, "end");
					roundData.BeginTime = data.StartTime.AddMilliseconds(uint4 - uint3);
					roundData.EndTime = data.StartTime.AddMilliseconds(uint5 - uint3);
					int.TryParse(dataUpdate.Dimensions["winner"], out roundData.WinningTeamId);
				}
				else if (text == "_ss_leaderboard")
				{
					foreach (KeyValuePair<string, string> keyValuePair in dataUpdate.Dimensions)
					{
						if (keyValuePair.Key != "stat")
						{
							data.Leaderboard[keyValuePair.Key] = keyValuePair.Value;
						}
					}
				}
				else if (text == "_ss_player_min_checkpoint")
				{
					ulong profileId = ulong.Parse(dataUpdate.Dimensions["profile"]);
					data.AddPlayerStat(profileId, "first_checkpoint", dataUpdate.Value);
				}
				else if (text == "_ss_player_max_checkpoint")
				{
					ulong profileId2 = ulong.Parse(dataUpdate.Dimensions["profile"]);
					data.AddPlayerStat(profileId2, "last_checkpoint", dataUpdate.Value);
				}
				else if (text == "player_resurrected_by_coin")
				{
					ulong uint6 = SessionSummaryTelemetrySource.GetUInt64(dataUpdate.Dimensions, "profile");
					data.AddPlayerStat(uint6, "coins_used", dataUpdate.Value);
				}
				else if (text == "player_kills_player" || text == "player_kills_ai")
				{
					ulong uint7 = SessionSummaryTelemetrySource.GetUInt64(dataUpdate.Dimensions, "profile");
					data.AddPlayerStat(uint7, "kill", dataUpdate.Value);
				}
				else if (text == "player_kills_player_friendly")
				{
					ulong uint8 = SessionSummaryTelemetrySource.GetUInt64(dataUpdate.Dimensions, "profile");
					data.AddPlayerStat(uint8, "kill_friendly", dataUpdate.Value);
				}
				else if (text == "player_headshots" || text == "player_melee_headshots")
				{
					ulong uint9 = SessionSummaryTelemetrySource.GetUInt64(dataUpdate.Dimensions, "profile");
					data.AddPlayerStat(uint9, "headshot", dataUpdate.Value);
				}
				else if (text == "player_kills_melee")
				{
					ulong uint10 = SessionSummaryTelemetrySource.GetUInt64(dataUpdate.Dimensions, "profile");
					data.AddPlayerStat(uint10, "melee", dataUpdate.Value);
				}
				else if (text == "player_kills_claymore")
				{
					ulong uint11 = SessionSummaryTelemetrySource.GetUInt64(dataUpdate.Dimensions, "profile");
					data.AddPlayerStat(uint11, "claymore", dataUpdate.Value);
				}
				else if (text == "_ss_player_kills_frag")
				{
					ulong uint12 = SessionSummaryTelemetrySource.GetUInt64(dataUpdate.Dimensions, "profile");
					data.AddPlayerStat(uint12, "frag", dataUpdate.Value);
				}
				else if (text == "_ss_player_kills_weapon_butt")
				{
					ulong uint13 = SessionSummaryTelemetrySource.GetUInt64(dataUpdate.Dimensions, "profile");
					data.AddPlayerStat(uint13, "weapon_butt", dataUpdate.Value);
				}
				else if (text == "_ss_player_kills_slide")
				{
					ulong uint14 = SessionSummaryTelemetrySource.GetUInt64(dataUpdate.Dimensions, "profile");
					data.AddPlayerStat(uint14, "slide_kill", dataUpdate.Value);
				}
				else if (text == "player_deaths")
				{
					ulong uint15 = SessionSummaryTelemetrySource.GetUInt64(dataUpdate.Dimensions, "profile");
					data.AddPlayerStat(uint15, "death", dataUpdate.Value);
				}
				if (text.StartsWith("_ss"))
				{
					updates.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
			data.AddMeasure(this.m_telemetryService.MeasureFromUpdates(updates));
			data.TelemetryContributed = true;
		}

		// Token: 0x0600220E RID: 8718 RVA: 0x0008E344 File Offset: 0x0008C744
		private static ulong GetUInt64(IDictionary<string, string> dimensions, string key)
		{
			ulong result;
			if (!ulong.TryParse(dimensions[key], out result))
			{
				throw new ArgumentException(string.Format("Argument missing at '{0}' stat", dimensions["stat"]), key);
			}
			return result;
		}

		// Token: 0x040010C4 RID: 4292
		private readonly ISessionSummaryService m_summaryService;

		// Token: 0x040010C5 RID: 4293
		private readonly ITelemetryService m_telemetryService;

		// Token: 0x040010C6 RID: 4294
		private readonly ITelemetryStreamService m_telemetryStreamService;
	}
}
