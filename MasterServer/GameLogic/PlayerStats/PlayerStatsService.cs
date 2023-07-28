using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.DAL.PlayerStats;
using MasterServer.Database;
using MasterServer.Telemetry.Metrics;
using OLAPHypervisor;

namespace MasterServer.GameLogic.PlayerStats
{
	// Token: 0x020003FA RID: 1018
	[Service]
	[Singleton]
	internal class PlayerStatsService : ServiceModule, IPlayerStatsService, IDebugPlayerStatsService, IPlayerStatsFactory
	{
		// Token: 0x060015FD RID: 5629 RVA: 0x0005C1E4 File Offset: 0x0005A5E4
		public PlayerStatsService(IDALService dal, ITelemetryDALService telemetryDal, IProcessingQueueMetricsTracker processingQueueMetricsTracker)
		{
			this.m_dal = dal;
			this.m_telemetryDal = telemetryDal;
			this.m_processingQueueMetricsTracker = processingQueueMetricsTracker;
		}

		// Token: 0x060015FE RID: 5630 RVA: 0x0005C2CC File Offset: 0x0005A6CC
		public List<Measure> PvPKillDeathRatioReset(ulong profileId)
		{
			List<Measure> list = new List<Measure>();
			foreach (string value in this.m_submodes)
			{
				foreach (string value2 in this.m_clanwars)
				{
					list.Add(new Measure
					{
						Value = 0L,
						RowCount = 1L,
						AggregateOp = EAggOperation.Override,
						Dimensions = new SortedList<string, string>
						{
							{
								"profile",
								profileId.ToString(CultureInfo.InvariantCulture)
							},
							{
								"mode",
								"PVP"
							},
							{
								"clan_war",
								value2
							},
							{
								"submode",
								value
							},
							{
								"stat",
								"player_kills_player"
							}
						}
					});
					list.Add(new Measure
					{
						Value = 0L,
						RowCount = 1L,
						AggregateOp = EAggOperation.Override,
						Dimensions = new SortedList<string, string>
						{
							{
								"profile",
								profileId.ToString(CultureInfo.InvariantCulture)
							},
							{
								"mode",
								"PVP"
							},
							{
								"clan_war",
								value2
							},
							{
								"submode",
								value
							},
							{
								"stat",
								"player_kills_player_friendly"
							}
						}
					});
					list.Add(new Measure
					{
						Value = 0L,
						RowCount = 1L,
						AggregateOp = EAggOperation.Override,
						Dimensions = new SortedList<string, string>
						{
							{
								"profile",
								profileId.ToString(CultureInfo.InvariantCulture)
							},
							{
								"mode",
								"PVP"
							},
							{
								"clan_war",
								value2
							},
							{
								"submode",
								value
							},
							{
								"stat",
								"player_deaths"
							}
						}
					});
				}
			}
			return list;
		}

		// Token: 0x060015FF RID: 5631 RVA: 0x0005C530 File Offset: 0x0005A930
		public List<Measure> PvPWinLoseRatioReset(ulong profileId)
		{
			List<Measure> list = new List<Measure>();
			foreach (string value in this.m_submodes)
			{
				foreach (string value2 in this.m_clanwars)
				{
					foreach (string value3 in this.m_pvpDiff)
					{
						list.Add(new Measure
						{
							Value = 0L,
							RowCount = 1L,
							AggregateOp = EAggOperation.Override,
							Dimensions = new SortedList<string, string>
							{
								{
									"profile",
									profileId.ToString(CultureInfo.InvariantCulture)
								},
								{
									"mode",
									"PVP"
								},
								{
									"difficulty",
									value3
								},
								{
									"clan_war",
									value2
								},
								{
									"submode",
									value
								},
								{
									"stat",
									"player_sessions_lost"
								}
							}
						});
						list.Add(new Measure
						{
							Value = 0L,
							RowCount = 1L,
							AggregateOp = EAggOperation.Override,
							Dimensions = new SortedList<string, string>
							{
								{
									"profile",
									profileId.ToString(CultureInfo.InvariantCulture)
								},
								{
									"mode",
									"PVP"
								},
								{
									"difficulty",
									value3
								},
								{
									"clan_war",
									value2
								},
								{
									"submode",
									value
								},
								{
									"stat",
									"player_sessions_won"
								}
							}
						});
					}
				}
			}
			return list;
		}

		// Token: 0x06001600 RID: 5632 RVA: 0x0005C774 File Offset: 0x0005AB74
		public override void Start()
		{
			this.m_updateQueue = new PlayerStatsUpdateProcessingQueue(500, this, this.m_processingQueueMetricsTracker);
			ConfigSection moduleSettings = Resources.ModuleSettings;
			moduleSettings.OnConfigChanged += this.OnConfigChanged;
			this.m_enablePlayerStats = (int.Parse(moduleSettings.Get("EnablePlayerStats")) > 0);
			MasterServer.Core.Log.Info<bool>("Starting player stats with enable option {0}", this.m_enablePlayerStats);
		}

		// Token: 0x06001601 RID: 5633 RVA: 0x0005C7D9 File Offset: 0x0005ABD9
		private void OnConfigChanged(ConfigEventArgs args)
		{
			if (args.Name.Equals("EnablePlayerStats", StringComparison.InvariantCultureIgnoreCase))
			{
				this.m_enablePlayerStats = (args.iValue > 0);
			}
		}

		// Token: 0x06001602 RID: 5634 RVA: 0x0005C800 File Offset: 0x0005AC00
		public override void Stop()
		{
			if (this.m_updateQueue != null)
			{
				this.m_updateQueue.Stop();
				this.m_updateQueue = null;
			}
		}

		// Token: 0x06001603 RID: 5635 RVA: 0x0005C820 File Offset: 0x0005AC20
		public void InitPlayerStats(ulong profileId)
		{
			List<Measure> list = this.GetPlayerStatsAggregated(profileId);
			if (list.Count == 0)
			{
				list = new List<Measure>(this.m_telemetryDal.TelemetrySystem.GetPlayerStatsRaw(profileId));
				this.TelemetryToPlayerStats(list);
				this.FlushPlayerStats(profileId, list);
			}
		}

		// Token: 0x06001604 RID: 5636 RVA: 0x0005C868 File Offset: 0x0005AC68
		private void SavePlayerStats(ulong profileId, List<Measure> playerStats)
		{
			string[] dimensions = new string[0];
			string[] array = new string[]
			{
				"mode"
			};
			string[] dimensions2 = new string[]
			{
				"class",
				"mode"
			};
			string[] dimensions3 = new string[]
			{
				"mode",
				"difficulty"
			};
			this.Aggregate(profileId, playerStats, "player_heal", dimensions);
			this.Aggregate(profileId, playerStats, "player_ammo_restored", dimensions);
			this.Aggregate(profileId, playerStats, "player_repair", dimensions);
			this.Aggregate(profileId, playerStats, "player_online_time", dimensions);
			this.Aggregate(profileId, playerStats, "player_sessions_won", dimensions3);
			this.Aggregate(profileId, playerStats, "player_sessions_lost", dimensions3);
			this.Aggregate(profileId, playerStats, "player_sessions_draw", dimensions3);
			this.Aggregate(profileId, playerStats, "player_sessions_left", array);
			this.Aggregate(profileId, playerStats, "player_sessions_kicked", array);
			string statName = "player_kill_streak";
			IEnumerable<string> dimensions4 = array;
			if (PlayerStatsService.<>f__mg$cache0 == null)
			{
				PlayerStatsService.<>f__mg$cache0 = new PlayerStatsService.OpDeleg(PlayerStatsService.OpMax);
			}
			this.Aggregate(profileId, playerStats, statName, dimensions4, PlayerStatsService.<>f__mg$cache0);
			this.Aggregate(profileId, playerStats, "player_kills_ai", array);
			this.Aggregate(profileId, playerStats, "player_kills_melee", array);
			this.Aggregate(profileId, playerStats, "player_kills_claymore", array);
			this.Aggregate(profileId, playerStats, "player_kills_player", array);
			this.Aggregate(profileId, playerStats, "player_kills_player_friendly", array);
			this.Aggregate(profileId, playerStats, "player_sessions_lost_connection", array);
			this.Aggregate(profileId, playerStats, "player_deaths", array);
			this.Aggregate(profileId, playerStats, "player_shots", dimensions2);
			this.Aggregate(profileId, playerStats, "player_hits", dimensions2);
			this.Aggregate(profileId, playerStats, "player_headshots", dimensions2);
			this.Aggregate(profileId, playerStats, "player_melee_headshots", dimensions2);
			this.Aggregate(profileId, playerStats, "player_playtime", dimensions2);
			if (!this.m_enablePlayerStats)
			{
				this.PreagregateWeaponStats(playerStats);
				return;
			}
			this.m_dal.PlayerStatSystem.UpdatePlayerStats(profileId, playerStats);
		}

		// Token: 0x06001605 RID: 5637 RVA: 0x0005CA30 File Offset: 0x0005AE30
		private void TelemetryToPlayerStats(List<Measure> playerStats)
		{
			foreach (Measure measure in playerStats)
			{
				if ((measure.Dimensions["stat"] == "player_sessions_won" || measure.Dimensions["stat"] == "player_sessions_lost" || measure.Dimensions["stat"] == "player_sessions_draw") && measure.Dimensions["mode"] == "PVP" && !measure.Dimensions.ContainsKey("difficulty"))
				{
					measure.Dimensions.Add("difficulty", string.Empty);
				}
			}
		}

		// Token: 0x06001606 RID: 5638 RVA: 0x0005CB2C File Offset: 0x0005AF2C
		public void UpdatePlayerStats(List<Measure> data)
		{
			if (!this.m_enablePlayerStats)
			{
				return;
			}
			this.m_updateQueue.Add(data);
		}

		// Token: 0x06001607 RID: 5639 RVA: 0x0005CB46 File Offset: 0x0005AF46
		public void FlushPlayerStats(List<Measure> data)
		{
			this.FlushPlayerStats(0UL, data);
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x0005CB54 File Offset: 0x0005AF54
		public void FlushPlayerStats(ulong profileId, List<Measure> data)
		{
			if (!this.m_enablePlayerStats)
			{
				return;
			}
			Dictionary<ulong, List<Measure>> dictionary = new Dictionary<ulong, List<Measure>>();
			for (int i = 0; i < data.Count; i++)
			{
				Measure item = new Measure(data[i]);
				if (item.Dimensions["stat"].StartsWith("player_"))
				{
					ulong num = profileId;
					if (item.Dimensions.ContainsKey("profile"))
					{
						num = ulong.Parse(item.Dimensions["profile"]);
					}
					if (num == 0UL)
					{
						MasterServer.Core.Log.Error<string>("Measure of stat {0} doesn't contain profile id", item.Dimensions["stat"]);
					}
					else
					{
						if (profileId != 0UL && num != profileId)
						{
							throw new ArgumentException(string.Format("Measure profile id {0} is differ than expected {1}", num, profileId));
						}
						List<Measure> list;
						if (!dictionary.TryGetValue(num, out list))
						{
							list = new List<Measure>();
							dictionary.Add(num, list);
						}
						item.Dimensions.Remove("profile");
						list.Add(item);
					}
				}
			}
			foreach (KeyValuePair<ulong, List<Measure>> keyValuePair in dictionary)
			{
				this.SavePlayerStats(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x06001609 RID: 5641 RVA: 0x0005CCDC File Offset: 0x0005B0DC
		public List<Measure> GetPlayerStats(ulong profileId)
		{
			if (!this.m_enablePlayerStats)
			{
				return new List<Measure>();
			}
			PlayerStatistics playerStats = this.m_dal.PlayerStatSystem.GetPlayerStats(profileId);
			return playerStats.Measures;
		}

		// Token: 0x0600160A RID: 5642 RVA: 0x0005CD14 File Offset: 0x0005B114
		public List<Measure> GetPlayerStatsAggregated(ulong profileId)
		{
			if (!this.m_enablePlayerStats)
			{
				return new List<Measure>();
			}
			PlayerStatistics playerStats = this.m_dal.PlayerStatSystem.GetPlayerStats(profileId);
			this.PreagregateWeaponStats(playerStats.Measures);
			return playerStats.Measures;
		}

		// Token: 0x0600160B RID: 5643 RVA: 0x0005CD56 File Offset: 0x0005B156
		public void ResetPlayerStats(ulong profileId)
		{
			if (!this.m_enablePlayerStats)
			{
				return;
			}
			this.m_dal.PlayerStatSystem.ResetPlayerStats(profileId);
		}

		// Token: 0x0600160C RID: 5644 RVA: 0x0005CD75 File Offset: 0x0005B175
		private static Measure OpAdd(Measure val1, Measure val2)
		{
			val1.Value += val2.Value;
			return val1;
		}

		// Token: 0x0600160D RID: 5645 RVA: 0x0005CD8D File Offset: 0x0005B18D
		private static Measure OpMax(Measure val1, Measure val2)
		{
			val1.Value = Math.Max(val1.Value, val2.Value);
			return val1;
		}

		// Token: 0x0600160E RID: 5646 RVA: 0x0005CDAA File Offset: 0x0005B1AA
		private void Aggregate(ulong profileId, List<Measure> playerStats, string statName, IEnumerable<string> dimensions)
		{
			if (PlayerStatsService.<>f__mg$cache1 == null)
			{
				PlayerStatsService.<>f__mg$cache1 = new PlayerStatsService.OpDeleg(PlayerStatsService.OpAdd);
			}
			this.Aggregate(profileId, playerStats, statName, dimensions, PlayerStatsService.<>f__mg$cache1);
		}

		// Token: 0x0600160F RID: 5647 RVA: 0x0005CDD4 File Offset: 0x0005B1D4
		private void Aggregate(ulong profileId, List<Measure> playerStats, string statName, IEnumerable<string> dimensions, PlayerStatsService.OpDeleg op)
		{
			List<Measure> list = new List<Measure>();
			Set<string> set = new Set<string>();
			int num = 0;
			while (num != playerStats.Count)
			{
				string a = playerStats[num].Dimensions["stat"];
				if (a != statName)
				{
					num++;
				}
				else
				{
					bool flag = true;
					foreach (string text in dimensions)
					{
						if (!playerStats[num].Dimensions.ContainsKey(text))
						{
							MasterServer.Core.Log.Warning<string, string, ulong>("Missing dimension {0} in stat {1} during profile {2} stats aggregation", text, statName, profileId);
							flag = false;
						}
					}
					if (!flag)
					{
						playerStats.RemoveAt(num);
					}
					else
					{
						set.Add(playerStats[num].Dimensions.Keys);
						set.Remove("stat");
						set.Remove(dimensions);
						foreach (string key in ((IEnumerable<string>)set))
						{
							playerStats[num].Dimensions.Remove(key);
						}
						set.Clear();
						int num2;
						for (num2 = 0; num2 != list.Count; num2++)
						{
							if (list[num2].DimensionsEqual(playerStats[num].Dimensions))
							{
								break;
							}
						}
						if (num2 != list.Count)
						{
							list[num2] = op(list[num2], playerStats[num]);
						}
						else
						{
							list.Add(playerStats[num]);
						}
						playerStats.RemoveAt(num);
					}
				}
			}
			foreach (Measure item in list)
			{
				playerStats.Add(item);
			}
		}

		// Token: 0x06001610 RID: 5648 RVA: 0x0005D01C File Offset: 0x0005B41C
		private void PreagregateWeaponStats(List<Measure> playerStats)
		{
			Dictionary<string, Measure> dictionary = new Dictionary<string, Measure>();
			int num = 0;
			while (num != playerStats.Count)
			{
				if (playerStats[num].Dimensions["stat"] == "player_wpn_usage")
				{
					string key = playerStats[num].Dimensions["class"];
					Measure measure;
					if (dictionary.TryGetValue(key, out measure))
					{
						if (playerStats[num].Value > measure.Value)
						{
							dictionary[key] = playerStats[num];
						}
					}
					else
					{
						dictionary.Add(key, playerStats[num]);
					}
					playerStats.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
			playerStats.AddRange(dictionary.Values);
		}

		// Token: 0x04000A8A RID: 2698
		private readonly List<string> m_submodes = new List<string>
		{
			"TDM",
			"CTF",
			"PTB",
			"STM",
			"FFA",
			"DST",
			"DMN",
			"HNT",
			"TBS"
		};

		// Token: 0x04000A8B RID: 2699
		private readonly List<string> m_pvpDiff = new List<string>
		{
			string.Empty,
			"normal"
		};

		// Token: 0x04000A8C RID: 2700
		private readonly List<string> m_clanwars = new List<string>
		{
			"0",
			"1"
		};

		// Token: 0x04000A8D RID: 2701
		private const string PLAYER_WPN_USAGE = "player_wpn_usage";

		// Token: 0x04000A8E RID: 2702
		private const string PLAYER_SESSIONS_WON = "player_sessions_won";

		// Token: 0x04000A8F RID: 2703
		private const string PLAYER_SESSIONS_LOST = "player_sessions_lost";

		// Token: 0x04000A90 RID: 2704
		private const string PLAYER_SESSIONS_DRAW = "player_sessions_draw";

		// Token: 0x04000A91 RID: 2705
		private const string PLAYER_SESSIONS_LEFT = "player_sessions_left";

		// Token: 0x04000A92 RID: 2706
		private const string PLAYER_SESSIONS_KICKED = "player_sessions_kicked";

		// Token: 0x04000A93 RID: 2707
		private const string PLAYER_HEADSHOTS = "player_headshots";

		// Token: 0x04000A94 RID: 2708
		private const string PLAYER_MELEE_HEADSHOTS = "player_melee_headshots";

		// Token: 0x04000A95 RID: 2709
		private const string PLAYER_HEAL = "player_heal";

		// Token: 0x04000A96 RID: 2710
		private const string PLAYER_AMMO_RESTORED = "player_ammo_restored";

		// Token: 0x04000A97 RID: 2711
		private const string PLAYER_REPAIR = "player_repair";

		// Token: 0x04000A98 RID: 2712
		private const string PLAYER_PLAYTIME = "player_playtime";

		// Token: 0x04000A99 RID: 2713
		private const string PLAYER_SHOTS = "player_shots";

		// Token: 0x04000A9A RID: 2714
		private const string PLAYER_HITS = "player_hits";

		// Token: 0x04000A9B RID: 2715
		private const string PLAYER_KILL_STREAK = "player_kill_streak";

		// Token: 0x04000A9C RID: 2716
		private const string PLAYER_KILLS_AI = "player_kills_ai";

		// Token: 0x04000A9D RID: 2717
		private const string PLAYER_KILLS_MELEE = "player_kills_melee";

		// Token: 0x04000A9E RID: 2718
		private const string PLAYER_KILLS_CLAYMORE = "player_kills_claymore";

		// Token: 0x04000A9F RID: 2719
		private const string PLAYER_KILLS_PLAYER = "player_kills_player";

		// Token: 0x04000AA0 RID: 2720
		private const string PLAYER_KILLS_PLAYER_FRIENDLY = "player_kills_player_friendly";

		// Token: 0x04000AA1 RID: 2721
		private const string PLAYER_SESSIONS_LOST_CONNECTION = "player_sessions_lost_connection";

		// Token: 0x04000AA2 RID: 2722
		private const string PLAYER_DEATHS = "player_deaths";

		// Token: 0x04000AA3 RID: 2723
		private const string PLAYER_ONLINE_TIME = "player_online_time";

		// Token: 0x04000AA4 RID: 2724
		private const int DEF_PLAYER_STATS_UPDATE_QUEUE_LIMIT = 500;

		// Token: 0x04000AA5 RID: 2725
		private bool m_enablePlayerStats = true;

		// Token: 0x04000AA6 RID: 2726
		private readonly IDALService m_dal;

		// Token: 0x04000AA7 RID: 2727
		private readonly ITelemetryDALService m_telemetryDal;

		// Token: 0x04000AA8 RID: 2728
		private readonly IProcessingQueueMetricsTracker m_processingQueueMetricsTracker;

		// Token: 0x04000AA9 RID: 2729
		private PlayerStatsUpdateProcessingQueue m_updateQueue;

		// Token: 0x04000AAA RID: 2730
		[CompilerGenerated]
		private static PlayerStatsService.OpDeleg <>f__mg$cache0;

		// Token: 0x04000AAB RID: 2731
		[CompilerGenerated]
		private static PlayerStatsService.OpDeleg <>f__mg$cache1;

		// Token: 0x02000535 RID: 1333
		// (Invoke) Token: 0x06001CFF RID: 7423
		private delegate Measure OpDeleg(Measure val1, Measure val2);
	}
}
