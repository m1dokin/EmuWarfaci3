using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020005FD RID: 1533
	[RoomExtension]
	internal class LogRoomEvents : RoomExtensionBase
	{
		// Token: 0x060020AC RID: 8364 RVA: 0x00085B0F File Offset: 0x00083F0F
		public LogRoomEvents(ILogService logService, IClanService clanService)
		{
			this.m_logService = logService;
			this.m_clanService = clanService;
		}

		// Token: 0x060020AD RID: 8365 RVA: 0x00085B30 File Offset: 0x00083F30
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			base.Room.tr_player_added += this.OnPlayerAdded;
			base.Room.tr_player_removed += this.OnPlayerRemoved;
			base.Room.tr_player_status += this.OnPlayerStatusChanged;
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_ended += this.OnSessionEnded;
			KickExtension extension2 = base.Room.GetExtension<KickExtension>();
			extension2.tr_player_kick += this.OnPlayerKick;
		}

		// Token: 0x060020AE RID: 8366 RVA: 0x00085BC8 File Offset: 0x00083FC8
		protected override void OnDisposing()
		{
			base.Room.tr_player_added -= this.OnPlayerAdded;
			base.Room.tr_player_removed -= this.OnPlayerRemoved;
			base.Room.tr_player_status -= this.OnPlayerStatusChanged;
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_ended -= this.OnSessionEnded;
			KickExtension extension2 = base.Room.GetExtension<KickExtension>();
			extension2.tr_player_kick -= this.OnPlayerKick;
			base.OnDisposing();
		}

		// Token: 0x060020AF RID: 8367 RVA: 0x00085C5C File Offset: 0x0008405C
		private void OnPlayerKick(RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			this.m_logService.Event.CharacterKickLog(player.UserID, player.ProfileID, reason, player.RegionId, base.Room.MissionType.Name, base.Room.MissionName);
		}

		// Token: 0x060020B0 RID: 8368 RVA: 0x00085C9C File Offset: 0x0008409C
		private void OnPlayerAdded(ulong profileID, GameRoomPlayerAddReason reason)
		{
			RoomPlayer player = base.Room.GetPlayer(profileID);
			this.m_logService.Event.RoomJoinedLog(player.UserID, player.ProfileID, player.GroupID, base.Room.ID, base.Room.RoomName, reason.ToString(), base.Room.Type, this.GetPlayerClanName(player.ProfileID));
		}

		// Token: 0x060020B1 RID: 8369 RVA: 0x00085D14 File Offset: 0x00084114
		private void OnPlayerRemoved(RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			this.ReportSessionEnded(player.ProfileID, extension.SessionID);
			TimeSpan timeInRoom = DateTime.UtcNow - player.JoinTime;
			this.m_logService.Event.RoomLeftLog(player.UserID, player.ProfileID, player.GroupID, base.Room.ID, reason, player.RoomStatus, base.Room.RoomName, timeInRoom, base.Room.Type, this.GetPlayerClanName(player.ProfileID));
		}

		// Token: 0x060020B2 RID: 8370 RVA: 0x00085DA8 File Offset: 0x000841A8
		private void OnPlayerStatusChanged(ulong profileId, UserStatus oldStatus, UserStatus newStatus)
		{
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			if (!UserStatuses.IsInGame(oldStatus) && UserStatuses.IsInGame(newStatus))
			{
				RoomPlayer player = base.Room.GetPlayer(profileId);
				if (extension.Started && player != null)
				{
					this.ReportSessionStarted(player, extension.SessionID);
				}
			}
			if (UserStatuses.IsInGame(oldStatus) && !UserStatuses.IsInGame(newStatus))
			{
				this.ReportSessionEnded(profileId, extension.SessionID);
			}
		}

		// Token: 0x060020B3 RID: 8371 RVA: 0x00085E28 File Offset: 0x00084228
		private void OnSessionEnded(string sessionId, bool abnormal)
		{
			Dictionary<ulong, RoomPlayer> dictionary = new Dictionary<ulong, RoomPlayer>(this.m_joinedToSession);
			foreach (KeyValuePair<ulong, RoomPlayer> keyValuePair in dictionary)
			{
				this.ReportSessionEnded(keyValuePair.Key, sessionId);
			}
			dictionary.Clear();
			this.m_joinedToSession.Clear();
		}

		// Token: 0x060020B4 RID: 8372 RVA: 0x00085EA4 File Offset: 0x000842A4
		private void ReportSessionStarted(RoomPlayer player, string sessionId)
		{
			this.ReportSessionEnded(player.ProfileID, sessionId);
			TimeSpan gameLeadTime = DateTime.UtcNow - player.GameWaitStartTime;
			this.m_logService.Event.SessionStartedLog(player.UserID, player.ProfileID, player.GroupID, base.Room.ID, base.Room.Type, sessionId, base.Room.MissionType.Name, player.Observer, gameLeadTime, ProfileProgressionInfo.ClassToClassChar[ProfileProgressionInfo.PlayerClassFromClassId(player.ClassID)].ToString(), player.RegionId, this.GetPlayerClanName(player.ProfileID));
			this.m_joinedToSession.Add(player.ProfileID, player);
		}

		// Token: 0x060020B5 RID: 8373 RVA: 0x00085F68 File Offset: 0x00084368
		private void ReportSessionEnded(ulong profileId, string sessionId)
		{
			RoomPlayer roomPlayer;
			if (this.m_joinedToSession.TryGetValue(profileId, out roomPlayer))
			{
				this.m_logService.Event.SessionEndedLog(roomPlayer.UserID, roomPlayer.ProfileID, base.Room.ID, base.Room.Type, sessionId, base.Room.MissionType.Name, roomPlayer.Observer);
				roomPlayer.GameWaitStartTime = DateTime.UtcNow;
				this.m_joinedToSession.Remove(profileId);
			}
		}

		// Token: 0x060020B6 RID: 8374 RVA: 0x00085FEC File Offset: 0x000843EC
		private string GetPlayerClanName(ulong profileId)
		{
			ClanInfo clanInfoByPid = this.m_clanService.GetClanInfoByPid(profileId);
			return (clanInfoByPid != null) ? clanInfoByPid.Name : string.Empty;
		}

		// Token: 0x04001005 RID: 4101
		private readonly ILogService m_logService;

		// Token: 0x04001006 RID: 4102
		private readonly IClanService m_clanService;

		// Token: 0x04001007 RID: 4103
		private readonly Dictionary<ulong, RoomPlayer> m_joinedToSession = new Dictionary<ulong, RoomPlayer>();
	}
}
