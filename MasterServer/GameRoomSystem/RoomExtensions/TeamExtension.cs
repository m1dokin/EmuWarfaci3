using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoom.RoomExtensions.Reconnect;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020004FF RID: 1279
	[RoomExtension]
	internal class TeamExtension : RoomExtensionBase
	{
		// Token: 0x06001B8E RID: 7054 RVA: 0x0006F832 File Offset: 0x0006DC32
		public TeamExtension(IAutoTeamBalanceLogic autoTeamBalanceLogic, ILogService logService)
		{
			this.m_autoTeamBalanceLogic = autoTeamBalanceLogic;
			this.m_logService = logService;
		}

		// Token: 0x06001B8F RID: 7055 RVA: 0x0006F848 File Offset: 0x0006DC48
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_starting += this.OnSessionStarting;
			MissionExtension extension2 = base.Room.GetExtension<MissionExtension>();
			extension2.TrSetMissionInfoEnded += this.OnSetMissionEnded;
			CustomParamsExtension extension3 = base.Room.GetExtension<CustomParamsExtension>();
			extension3.customParamsChanged += this.OnCustomParamsChanged;
			base.Room.tr_player_add_check += this.OnTrPlayerAddCheck;
			base.Room.tr_player_added += this.OnTrPlayerAdded;
			base.Room.tr_player_joined_session += this.OnTrPlayerJoinedSession;
			base.Room.tr_player_removed += this.OnTrPlayerRemoved;
		}

		// Token: 0x06001B90 RID: 7056 RVA: 0x0006F914 File Offset: 0x0006DD14
		protected override void OnDisposing()
		{
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_starting -= this.OnSessionStarting;
			MissionExtension extension2 = base.Room.GetExtension<MissionExtension>();
			extension2.TrSetMissionInfoEnded -= this.OnSetMissionEnded;
			CustomParamsExtension extension3 = base.Room.GetExtension<CustomParamsExtension>();
			extension3.customParamsChanged -= this.OnCustomParamsChanged;
			base.Room.tr_player_add_check -= this.OnTrPlayerAddCheck;
			base.Room.tr_player_added -= this.OnTrPlayerAdded;
			base.Room.tr_player_joined_session -= this.OnTrPlayerJoinedSession;
			base.OnDisposing();
		}

		// Token: 0x06001B91 RID: 7057 RVA: 0x0006F9C8 File Offset: 0x0006DDC8
		public void SwitchTeam(RoomPlayer player, int teamId = -1)
		{
			if (player.Observer)
			{
				teamId = 0;
			}
			else if (teamId <= 0)
			{
				teamId = this.ChooseTeam(player);
			}
			if (player.TeamID == teamId)
			{
				return;
			}
			int num = base.Room.Players.Count((RoomPlayer x) => x.TeamID == teamId);
			if (player.Observer || num < base.Room.MaxTeamSize)
			{
				player.TeamID = teamId;
				Log.Verbose(Log.Group.GameRoom, "Player profileId: {0}, observer: {1}, switched to team: {2} in roomId: {3}", new object[]
				{
					player.ProfileID,
					player.Observer,
					teamId,
					base.Room.ID
				});
			}
		}

		// Token: 0x06001B92 RID: 7058 RVA: 0x0006FAB8 File Offset: 0x0006DEB8
		public int ChooseTeam(RoomPlayer player)
		{
			if (player.Observer)
			{
				return 0;
			}
			List<RoomPlayer> players = (from x in base.Room.Players
			where !x.Observer
			select x).ToList<RoomPlayer>();
			return this.m_autoTeamBalanceLogic.ChooseTeam(player, base.Room, players);
		}

		// Token: 0x06001B93 RID: 7059 RVA: 0x0006FB1A File Offset: 0x0006DF1A
		private void BalanceReadyPlayers()
		{
			this.BalancePlayers((RoomPlayer x) => x.RoomStatus == RoomPlayer.EStatus.Ready && !x.Observer);
		}

		// Token: 0x06001B94 RID: 7060 RVA: 0x0006FB3F File Offset: 0x0006DF3F
		private void BalanceAllPlayers()
		{
			this.BalancePlayers((RoomPlayer x) => !x.Observer);
		}

		// Token: 0x06001B95 RID: 7061 RVA: 0x0006FB64 File Offset: 0x0006DF64
		private void PutNotReadyPlayerToEmptySlots()
		{
			List<RoomPlayer> list = base.Room.Players.ToList<RoomPlayer>();
			Dictionary<int, TeamInfo> teams = this.m_autoTeamBalanceLogic.BalancePlayers(base.Room, list);
			this.AssignTeams(list, teams);
		}

		// Token: 0x06001B96 RID: 7062 RVA: 0x0006FBA0 File Offset: 0x0006DFA0
		private void AssignTeams(IEnumerable<RoomPlayer> readyPlayers, Dictionary<int, TeamInfo> teams)
		{
			foreach (RoomPlayer roomPlayer in readyPlayers)
			{
				foreach (TeamInfo teamInfo in teams.Values)
				{
					if (teamInfo.HasPlayer(roomPlayer))
					{
						roomPlayer.TeamID = teamInfo.TeamId;
					}
				}
			}
		}

		// Token: 0x06001B97 RID: 7063 RVA: 0x0006FC4C File Offset: 0x0006E04C
		public bool IsEnoughPlayersToStart()
		{
			CoreState state = base.Room.GetState<CoreState>(AccessMode.ReadOnly);
			MissionState state2 = base.Room.GetState<MissionState>(AccessMode.ReadOnly);
			List<RoomPlayer> list = (from x in base.Room.Players
			where !x.Observer
			select x).ToList<RoomPlayer>();
			if (string.IsNullOrEmpty(state2.Mission.gameMode))
			{
				string text = "undefined";
				try
				{
					ServerExtension extension = base.Room.GetExtension<ServerExtension>();
					text = extension.Status.ToString();
				}
				catch
				{
				}
				string format = "There is no game mode in room {0} with mission '{1}'. Room type: {2}, Players count: {3}, Room server status: {4}";
				Log.Warning(format, new object[]
				{
					base.Room.ID,
					state2.Mission.missionName,
					base.Room.Type.ToString(),
					list.Count,
					text
				});
			}
			Dictionary<int, TeamInfo> dictionary = this.m_autoTeamBalanceLogic.BalancePlayers(base.Room, list);
			int num = dictionary[1].Count((RoomPlayer x) => x.RoomStatus == RoomPlayer.EStatus.Ready);
			int num2 = dictionary[2].Count((RoomPlayer x) => x.RoomStatus == RoomPlayer.EStatus.Ready);
			return Math.Abs(num - num2) <= state.TeamsReadyPlayersDiff;
		}

		// Token: 0x06001B98 RID: 7064 RVA: 0x0006FDE8 File Offset: 0x0006E1E8
		private void BalancePlayers(Func<RoomPlayer, bool> action)
		{
			this.ResetTeams();
			List<RoomPlayer> list = base.Room.Players.Where(action).ToList<RoomPlayer>();
			Dictionary<int, TeamInfo> teams = this.m_autoTeamBalanceLogic.BalancePlayers(base.Room, list);
			this.AssignTeams(list, teams);
		}

		// Token: 0x06001B99 RID: 7065 RVA: 0x0006FE30 File Offset: 0x0006E230
		private void OnSessionStarting()
		{
			CustomParams state = base.Room.GetState<CustomParams>(AccessMode.ReadOnly);
			if (state.Autobalance)
			{
				this.BalanceReadyPlayers();
				this.PutNotReadyPlayerToEmptySlots();
				this.LogAutoBalanceEvent();
			}
		}

		// Token: 0x06001B9A RID: 7066 RVA: 0x0006FE67 File Offset: 0x0006E267
		private void OnCustomParamsChanged(CustomParams oldState, CustomParams newState)
		{
			if (oldState.MaxPlayers > newState.MaxPlayers)
			{
				this.BalanceAllPlayers();
				this.LimitPlayers();
			}
			if (newState.Autobalance && !oldState.Autobalance)
			{
				this.BalanceAllPlayers();
			}
		}

		// Token: 0x06001B9B RID: 7067 RVA: 0x0006FEA4 File Offset: 0x0006E2A4
		private GameRoomRetCode OnTrPlayerAddCheck(RoomPlayer player)
		{
			return (this.ChooseTeam(player) != 0 || base.Room.NoTeamsMode) ? GameRoomRetCode.OK : GameRoomRetCode.FULL;
		}

		// Token: 0x06001B9C RID: 7068 RVA: 0x0006FED8 File Offset: 0x0006E2D8
		private void OnTrPlayerAdded(ulong profileId, GameRoomPlayerAddReason reason)
		{
			ServerExtension extension = base.Room.GetExtension<ServerExtension>();
			RoomPlayer player = base.Room.GetPlayer(profileId);
			if (base.Room.Autobalance && !extension.GameRunning)
			{
				this.BalanceAllPlayers();
			}
			else
			{
				player.TeamID = this.GetTeamId(player);
			}
			if (!base.Room.NoTeamsMode && player.TeamID == 0)
			{
				throw new ApplicationException(string.Format("Team balancing error in room {0} {1}, for {2}", base.Room.ID, base.Room.Type, player.OnlineID));
			}
		}

		// Token: 0x06001B9D RID: 7069 RVA: 0x0006FF84 File Offset: 0x0006E384
		private int GetTeamId(RoomPlayer player)
		{
			ReconnectInfo reconnectInfo = null;
			if (base.Room.AllowReconnect)
			{
				ReconnectExtension extension = base.Room.GetExtension<ReconnectExtension>();
				reconnectInfo = extension.GetReconnectInfo(player.ProfileID);
			}
			int teamId;
			if (reconnectInfo != null)
			{
				teamId = reconnectInfo.TeamId;
				int num = base.Room.Players.Count((RoomPlayer p) => p.TeamID == teamId);
				if (num >= base.Room.MaxTeamSize)
				{
					if (base.Room.PlayerCount < base.Room.MaxPlayers)
					{
						teamId = ((teamId != 1) ? 1 : 2);
					}
					else
					{
						teamId = 0;
					}
				}
			}
			else
			{
				teamId = this.ChooseTeam(player);
			}
			return teamId;
		}

		// Token: 0x06001B9E RID: 7070 RVA: 0x0007005C File Offset: 0x0006E45C
		private void OnTrPlayerRemoved(RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			ServerExtension extension = base.Room.GetExtension<ServerExtension>();
			if (base.Room.Autobalance && !extension.GameRunning)
			{
				this.BalanceAllPlayers();
			}
		}

		// Token: 0x06001B9F RID: 7071 RVA: 0x00070098 File Offset: 0x0006E498
		private void OnTrPlayerJoinedSession(ulong profileId)
		{
			RoomPlayer player = base.Room.GetPlayer(profileId);
			if (player.TeamID == 0)
			{
				this.SwitchTeam(player, -1);
			}
		}

		// Token: 0x06001BA0 RID: 7072 RVA: 0x000700C5 File Offset: 0x0006E4C5
		private void OnSetMissionEnded(MissionContext mission)
		{
			if (base.Room.Autobalance)
			{
				this.BalanceAllPlayers();
			}
		}

		// Token: 0x06001BA1 RID: 7073 RVA: 0x000700DD File Offset: 0x0006E4DD
		public void DebugBalanceReadyPlayers()
		{
			if (!Resources.DebugQueriesEnabled)
			{
				throw new InvalidOperationException("This operation should only be called when MS is started in debug mode");
			}
			this.BalanceReadyPlayers();
		}

		// Token: 0x06001BA2 RID: 7074 RVA: 0x000700FA File Offset: 0x0006E4FA
		public void DebugBalanceAllPlayers()
		{
			if (!Resources.DebugQueriesEnabled)
			{
				throw new InvalidOperationException("This operation should only be called when MS is started in debug mode");
			}
			base.Room.CheckAccessMode(AccessMode.ReadWrite);
			if (!base.Room.GetState<CustomParams>(AccessMode.ReadOnly).Autobalance)
			{
				return;
			}
			this.BalanceAllPlayers();
		}

		// Token: 0x06001BA3 RID: 7075 RVA: 0x0007013C File Offset: 0x0006E53C
		private void ResetTeams()
		{
			foreach (RoomPlayer roomPlayer in base.Room.Players)
			{
				roomPlayer.TeamID = 0;
			}
		}

		// Token: 0x06001BA4 RID: 7076 RVA: 0x0007019C File Offset: 0x0006E59C
		private void LimitPlayers()
		{
			List<RoomPlayer> list = (from x in base.Room.Players
			where x.TeamID == 0 && !x.Observer
			select x).ToList<RoomPlayer>();
			foreach (RoomPlayer roomPlayer in list)
			{
				base.Room.RemovePlayer(roomPlayer.ProfileID, GameRoomPlayerRemoveReason.KickOverflow);
			}
		}

		// Token: 0x06001BA5 RID: 7077 RVA: 0x00070234 File Offset: 0x0006E634
		private void LogAutoBalanceEvent()
		{
			List<RoomPlayer> list = (from x in base.Room.Players
			where x.TeamID == 1
			select x).ToList<RoomPlayer>();
			List<RoomPlayer> list2 = (from x in base.Room.Players
			where x.TeamID == 2
			select x).ToList<RoomPlayer>();
			double num = list.Sum((RoomPlayer x) => x.Skill.Value);
			double num2 = list2.Sum((RoomPlayer x) => x.Skill.Value);
			string team1Skill = HttpUtility.UrlEncode(num.ToString());
			string team2Skill = HttpUtility.UrlEncode(num2.ToString());
			this.m_logService.Event.RoomAutoBalanceResult(base.Room.ID, list.Count, team1Skill, list2.Count, team2Skill);
		}

		// Token: 0x04000D36 RID: 3382
		private readonly IAutoTeamBalanceLogic m_autoTeamBalanceLogic;

		// Token: 0x04000D37 RID: 3383
		private readonly ILogService m_logService;
	}
}
