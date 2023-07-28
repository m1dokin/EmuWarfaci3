using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.GameModes;
using MasterServer.GameLogic.MissionAccessLimitation;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020005FF RID: 1535
	[RoomExtension]
	internal class MissionExtension : RoomExtensionBase
	{
		// Token: 0x060020B8 RID: 8376 RVA: 0x0008602F File Offset: 0x0008442F
		public MissionExtension(IMissionAccessLimitationService limitationService, IMissionSystem missionSystem, IGameModesSystem gameModesSystem)
		{
			this.m_limitationService = limitationService;
			this.m_missionSystem = missionSystem;
			this.m_gameModesSystem = gameModesSystem;
		}

		// Token: 0x14000082 RID: 130
		// (add) Token: 0x060020B9 RID: 8377 RVA: 0x0008604C File Offset: 0x0008444C
		// (remove) Token: 0x060020BA RID: 8378 RVA: 0x00086084 File Offset: 0x00084484
		internal event MissionExtension.TrOnSetMissionInfoDeleg TrSetMissionInfoEnded;

		// Token: 0x060020BB RID: 8379 RVA: 0x000860BC File Offset: 0x000844BC
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_can_start += this.OnSessionCanStart;
		}

		// Token: 0x060020BC RID: 8380 RVA: 0x000860F0 File Offset: 0x000844F0
		protected override void OnDisposing()
		{
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_can_start -= this.OnSessionCanStart;
			base.OnDisposing();
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x060020BD RID: 8381 RVA: 0x00086121 File Offset: 0x00084521
		public MissionContext Mission
		{
			get
			{
				return base.Room.GetState<MissionState>(AccessMode.ReadOnly).Mission;
			}
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x060020BE RID: 8382 RVA: 0x00086134 File Offset: 0x00084534
		public string MissionKey
		{
			get
			{
				return base.Room.GetState<MissionState>(AccessMode.ReadOnly).Mission.uid;
			}
		}

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x060020BF RID: 8383 RVA: 0x0008614C File Offset: 0x0008454C
		public int MaxPlayers
		{
			get
			{
				return base.Room.GetState<CustomParams>(AccessMode.ReadOnly).MaxPlayers;
			}
		}

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x060020C0 RID: 8384 RVA: 0x0008615F File Offset: 0x0008455F
		public bool NoTeamsMode
		{
			get
			{
				return base.Room.GetState<MissionState>(AccessMode.ReadOnly).Mission.noTeamsMode;
			}
		}

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x060020C1 RID: 8385 RVA: 0x00086177 File Offset: 0x00084577
		public MissionType MissionType
		{
			get
			{
				return base.Room.GetState<MissionState>(AccessMode.ReadOnly).Mission.missionType;
			}
		}

		// Token: 0x17000357 RID: 855
		// (get) Token: 0x060020C2 RID: 8386 RVA: 0x0008618F File Offset: 0x0008458F
		public string MissionDifficulty
		{
			get
			{
				return base.Room.GetState<MissionState>(AccessMode.ReadOnly).Mission.difficulty;
			}
		}

		// Token: 0x060020C3 RID: 8387 RVA: 0x000861A8 File Offset: 0x000845A8
		public GameRoomRetCode SetMission(string mission_key)
		{
			MissionState state = base.Room.GetState<MissionState>(AccessMode.ReadWrite);
			bool flag = this.m_missionSystem.IsMissionExpired(mission_key);
			if (flag)
			{
				return GameRoomRetCode.MISSION_EXPIRED;
			}
			if (base.Room.GetExtension<SessionExtension>().Started)
			{
				Log.Error<ulong, string, string>("Can't change mission after session being started. Room id {0}, old mission {1}, new mission {2}", base.Room.ID, state.Mission.uid, mission_key);
				return GameRoomRetCode.ERROR;
			}
			MissionContext mission = this.m_missionSystem.GetMission(mission_key);
			if (!base.Room.IsAutoStartMode() && !MissionSystem.MissionAvailable(mission))
			{
				Log.Error<string>("Mission {0} is unavailable on this channel", mission_key);
				return GameRoomRetCode.ERROR;
			}
			if (mission.tutorialMission != 0)
			{
				Log.Error<string, ulong>("Tutorial mission {0} isn't allowed for room {1}", mission_key, base.Room.ID);
				return GameRoomRetCode.ERROR;
			}
			if (base.Room.IsPveMode() && !mission.IsPveMode())
			{
				Log.Error<string, ulong>("Not PvE mission {0} isn't allowed for room {1}", mission_key, base.Room.ID);
				return GameRoomRetCode.ERROR;
			}
			if (base.Room.IsPvpMode() && mission.IsPveMode())
			{
				Log.Error<string, ulong>("Not PvP mission {0} isn't allowed for room {1}", mission_key, base.Room.ID);
				return GameRoomRetCode.ERROR;
			}
			if (base.Room.IsClanWarMode() && !mission.clanWarMission)
			{
				Log.Error<string, ulong>("Not ClanWar mission {0} isn't allowed for room {1}", mission_key, base.Room.ID);
				return GameRoomRetCode.ERROR;
			}
			if (base.Room.Type != GameRoomType.PvP_ClanWar && mission.onlyClanWarMission)
			{
				Log.Error<string, ulong>("ClanWar mission {0} isn't allowed for room {1}", mission_key, base.Room.ID);
				return GameRoomRetCode.ERROR;
			}
			GameModeSetting gameModeSetting = this.m_gameModesSystem.GetGameModeSetting(mission);
			if (gameModeSetting == null)
			{
				Log.Error<string>("Unknown game mode '{0}', discarding", mission.gameMode);
				return GameRoomRetCode.ERROR;
			}
			int minReadyPlayers;
			if (!gameModeSetting.GetSetting(base.Room.Type, ERoomSetting.MIN_PLAYERS_READY, out minReadyPlayers))
			{
				Log.Error<string>("Can't retrieve 'min_players_ready' setting for game mode '{0}'", mission.gameMode);
				return GameRoomRetCode.ERROR;
			}
			int teamsReadyPlayersDiff;
			if (!gameModeSetting.GetSetting(base.Room.Type, ERoomSetting.TEAMS_READY_PLAYERS_DIFF, out teamsReadyPlayersDiff))
			{
				Log.Error<string>("Can't retrieve 'teams_ready_players_diff' setting for game mode '{0}'", mission.gameMode);
				return GameRoomRetCode.ERROR;
			}
			bool flag2 = string.IsNullOrEmpty(state.Mission.uid);
			state.Mission = mission;
			base.Room.MinReadyPlayers = minReadyPlayers;
			base.Room.TeamsReadyPlayersDiff = teamsReadyPlayersDiff;
			CustomParamsExtension extension = base.Room.GetExtension<CustomParamsExtension>();
			if (flag2)
			{
				extension.SetDefaultValues();
			}
			if (!base.Room.IsAutoStartMode())
			{
				Dictionary<ulong, RoomPlayer> players = base.Room.GetState<CoreState>(AccessMode.ReadWrite).Players;
				foreach (RoomPlayer roomPlayer in players.Values)
				{
					RoomPlayer.EStatus roomStatus = (!base.Room.IsPveMode() || (roomPlayer.ProfileProgression.IsMissionTypeUnlocked(base.Room.MissionType.Name) && roomPlayer.CanJoinMission(this.m_limitationService, base.Room.MissionType.Name))) ? RoomPlayer.EStatus.NotReady : RoomPlayer.EStatus.CantBeReady;
					roomPlayer.RoomStatus = roomStatus;
				}
			}
			base.Room.SignalPlayersChanged();
			bool flag3;
			if (!gameModeSetting.GetSetting(base.Room.Type, ERoomSetting.NO_TEAMS_MODE, out flag3))
			{
				Log.Error<string>("Can't retrieve 'no_teams_mode' setting for game mode '{0}'", mission.gameMode);
				return GameRoomRetCode.ERROR;
			}
			if (this.TrSetMissionInfoEnded != null)
			{
				this.TrSetMissionInfoEnded(this.Mission);
			}
			Log.Verbose(Log.Group.GameRoom, "Room {0} current mission changed to {1}", new object[]
			{
				base.Room.ID,
				state.Mission.name
			});
			return GameRoomRetCode.OK;
		}

		// Token: 0x060020C4 RID: 8388 RVA: 0x00086558 File Offset: 0x00084958
		private GameRoomRetCode OnSessionCanStart()
		{
			MissionState state = base.Room.GetState<MissionState>(AccessMode.ReadWrite);
			string uid = state.Mission.uid;
			if (string.IsNullOrEmpty(uid))
			{
				throw new ApplicationException("No mission specified when starting session");
			}
			bool flag = this.m_missionSystem.IsMissionExpired(uid);
			if (flag)
			{
				Log.Warning<string>("Mission '{0}' expired", uid);
				this.HandleExpiredMission();
				return GameRoomRetCode.MISSION_EXPIRED;
			}
			return GameRoomRetCode.OK;
		}

		// Token: 0x060020C5 RID: 8389 RVA: 0x000865BC File Offset: 0x000849BC
		private void HandleExpiredMission()
		{
			List<RoomPlayer> list = new List<RoomPlayer>(base.Room.Players);
			foreach (RoomPlayer roomPlayer in list)
			{
				base.Room.RemovePlayer(roomPlayer.ProfileID, GameRoomPlayerRemoveReason.Left);
				QueryManager.RequestSt("gameroom_on_expired", roomPlayer.OnlineID, new object[0]);
			}
		}

		// Token: 0x060020C6 RID: 8390 RVA: 0x00086648 File Offset: 0x00084A48
		public override XmlElement SerializeStateChanges(RoomUpdate.Context ctx)
		{
			MissionState missionState = (MissionState)ctx.new_state;
			XmlElement xmlElement = ctx.factory.CreateElement("mission");
			MissionExtension.SerializeMission(xmlElement, missionState.Mission, ctx.target, RoomUpdate.InformationType.UiBaseInfo | RoomUpdate.InformationType.CrownRewardsInfo);
			return xmlElement;
		}

		// Token: 0x060020C7 RID: 8391 RVA: 0x00086688 File Offset: 0x00084A88
		public static void SerializeMission(XmlElement el, MissionContext m, RoomUpdate.Target target, RoomUpdate.InformationType informationType)
		{
			el.SetAttribute("mission_key", m.uid);
			el.SetAttribute("no_teams", (!m.noTeamsMode) ? "0" : "1");
			if (target == RoomUpdate.Target.Server)
			{
				byte[] bytes = Encoding.ASCII.GetBytes(m.data);
				string value = Convert.ToBase64String(bytes);
				el.SetAttribute("mode", m.gameMode);
				el.SetAttribute("data", value);
				el.SetAttribute("type", m.missionType.Name);
			}
			else
			{
				if (informationType.HasFlag(RoomUpdate.InformationType.UiBaseInfo))
				{
					el.SetAttribute("name", m.name);
					el.SetAttribute("setting", m.baseLevel.name);
					el.SetAttribute("mode", m.gameMode);
					el.SetAttribute("mode_name", m.UIInfo.GameModeText);
					el.SetAttribute("mode_icon", m.UIInfo.GameModeIcon);
					el.SetAttribute("description", m.UIInfo.DescriptionText);
					el.SetAttribute("image", m.UIInfo.DescriptionIcon);
					el.SetAttribute("difficulty", m.difficulty);
					el.SetAttribute("type", m.missionType.Name);
					el.SetAttribute("time_of_day", m.timeOfDay);
					XmlElement xmlElement = el.OwnerDocument.CreateElement("objectives");
					xmlElement.SetAttribute("factor", Math.Max(1, m.subLevels.Count).ToString(CultureInfo.InvariantCulture));
					el.AppendChild(xmlElement);
					foreach (MissionObjective missionObjective in m.objectives)
					{
						XmlElement xmlElement2 = xmlElement.OwnerDocument.CreateElement("objective");
						xmlElement2.SetAttribute("id", missionObjective.id.ToString(CultureInfo.InvariantCulture));
						xmlElement2.SetAttribute("type", missionObjective.type);
						xmlElement.AppendChild(xmlElement2);
					}
				}
				if (informationType.HasFlag(RoomUpdate.InformationType.CrownRewardsInfo))
				{
					ConfigSection section = Resources.Rewards.GetSection("CrownRewards");
					foreach (ConfigSection configSection in section.GetSections("Reward"))
					{
						if (configSection.Get("type") == m.missionType.Name)
						{
							CrownRewardPool crownRewardPool = new CrownRewardPool();
							crownRewardPool.CalculateTreshold(m);
							if (crownRewardPool.IsValid())
							{
								XmlElement xmlElement3 = el.OwnerDocument.CreateElement("CrownRewardsThresholds");
								IEnumerator enumerator3 = Enum.GetValues(typeof(CrownRewardThreshold.PerformanceCategory)).GetEnumerator();
								try
								{
									while (enumerator3.MoveNext())
									{
										object obj = enumerator3.Current;
										CrownRewardThreshold.PerformanceCategory performanceCategory = (CrownRewardThreshold.PerformanceCategory)obj;
										XmlElement xmlElement4 = el.OwnerDocument.CreateElement(performanceCategory.ToString());
										LeagueThresholdBasic leagueThresholdBasic;
										uint num;
										if (crownRewardPool.TryGetThreshold(performanceCategory, out leagueThresholdBasic) && leagueThresholdBasic.TryGetValue(League.BRONZE, out num))
										{
											xmlElement4.SetAttribute("bronze", num.ToString(CultureInfo.InvariantCulture));
										}
										if (crownRewardPool.TryGetThreshold(performanceCategory, out leagueThresholdBasic) && leagueThresholdBasic.TryGetValue(League.SILVER, out num))
										{
											xmlElement4.SetAttribute("silver", num.ToString(CultureInfo.InvariantCulture));
										}
										if (crownRewardPool.TryGetThreshold(performanceCategory, out leagueThresholdBasic) && leagueThresholdBasic.TryGetValue(League.GOLD, out num))
										{
											xmlElement4.SetAttribute("gold", num.ToString(CultureInfo.InvariantCulture));
										}
										xmlElement3.AppendChild(xmlElement4);
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
								el.AppendChild(xmlElement3);
								XmlElement xmlElement5 = el.OwnerDocument.CreateElement("CrownRewards");
								xmlElement5.SetAttribute(League.BRONZE.ToString().ToLower(), configSection.Get("bronze").ToString(CultureInfo.InvariantCulture));
								xmlElement5.SetAttribute(League.SILVER.ToString().ToLower(), configSection.Get("silver").ToString(CultureInfo.InvariantCulture));
								xmlElement5.SetAttribute(League.GOLD.ToString().ToLower(), configSection.Get("gold").ToString(CultureInfo.InvariantCulture));
								el.AppendChild(xmlElement5);
							}
							break;
						}
					}
				}
			}
		}

		// Token: 0x0400100A RID: 4106
		private readonly IMissionAccessLimitationService m_limitationService;

		// Token: 0x0400100B RID: 4107
		private readonly IMissionSystem m_missionSystem;

		// Token: 0x0400100C RID: 4108
		private readonly IGameModesSystem m_gameModesSystem;

		// Token: 0x02000600 RID: 1536
		// (Invoke) Token: 0x060020C9 RID: 8393
		internal delegate void TrOnSetMissionInfoDeleg(MissionContext mission);
	}
}
