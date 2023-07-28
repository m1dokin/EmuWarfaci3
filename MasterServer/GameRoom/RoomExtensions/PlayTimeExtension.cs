using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Users;

namespace MasterServer.GameRoom.RoomExtensions
{
	// Token: 0x020004CE RID: 1230
	[RoomExtension]
	internal class PlayTimeExtension : RoomExtensionBase
	{
		// Token: 0x06001A9E RID: 6814 RVA: 0x0006D398 File Offset: 0x0006B798
		public PlayTimeExtension(ISessionStorage sessionStorage)
		{
			this.m_sessionStorage = sessionStorage;
		}

		// Token: 0x06001A9F RID: 6815 RVA: 0x0006D3A8 File Offset: 0x0006B7A8
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			room.tr_player_joined_session += this.OnPlayerJoined;
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.SessionStarted += this.OnSessionStarted;
		}

		// Token: 0x06001AA0 RID: 6816 RVA: 0x0006D3EC File Offset: 0x0006B7EC
		public override void Close()
		{
			base.Room.tr_player_joined_session -= this.OnPlayerJoined;
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.SessionStarted -= this.OnSessionStarted;
			base.Close();
		}

		// Token: 0x06001AA1 RID: 6817 RVA: 0x0006D434 File Offset: 0x0006B834
		public void UpdateClassPresence(ClassPresenceData data)
		{
			PlayersInTeamPlayTime playersInTeamPlayTime;
			if (!this.TryGetSessionPlaytimeData(data.sessionId, out playersInTeamPlayTime))
			{
				return;
			}
			foreach (KeyValuePair<ulong, List<ClassPresenceData.PresenceData>> keyValuePair in data.presence)
			{
				ulong key = keyValuePair.Key;
				List<ClassPresenceData.PresenceData> value = keyValuePair.Value;
				int num = value.Sum((ClassPresenceData.PresenceData x) => x.PlayedTimeSec);
				playersInTeamPlayTime.UpdatePlayer(key, TimeSpan.FromSeconds((double)num));
			}
		}

		// Token: 0x06001AA2 RID: 6818 RVA: 0x0006D4E4 File Offset: 0x0006B8E4
		private void OnSessionStarted(IGameRoom room, string sessionId)
		{
			PlayersInTeamPlayTime data = new PlayersInTeamPlayTime();
			this.m_sessionStorage.AddData(sessionId, ESessionData.PlayTime, data);
		}

		// Token: 0x06001AA3 RID: 6819 RVA: 0x0006D508 File Offset: 0x0006B908
		private void OnPlayerJoined(ulong profileId)
		{
			RoomPlayer player = base.Room.GetPlayer(profileId);
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			PlayersInTeamPlayTime playersInTeamPlayTime;
			if (!this.TryGetSessionPlaytimeData(extension.SessionID, out playersInTeamPlayTime))
			{
				throw new PlayTimeNotFoundException(extension.SessionID, profileId);
			}
			playersInTeamPlayTime.AddPlayer(player);
		}

		// Token: 0x06001AA4 RID: 6820 RVA: 0x0006D555 File Offset: 0x0006B955
		private bool TryGetSessionPlaytimeData(string sessionId, out PlayersInTeamPlayTime playersInTeamPlayTime)
		{
			playersInTeamPlayTime = this.GetPlayTimes(sessionId);
			return playersInTeamPlayTime != null;
		}

		// Token: 0x06001AA5 RID: 6821 RVA: 0x0006D568 File Offset: 0x0006B968
		private PlayersInTeamPlayTime GetPlayTimes(string sessionId)
		{
			return this.m_sessionStorage.GetData<PlayersInTeamPlayTime>(sessionId, ESessionData.PlayTime);
		}

		// Token: 0x04000CBC RID: 3260
		private readonly ISessionStorage m_sessionStorage;
	}
}
