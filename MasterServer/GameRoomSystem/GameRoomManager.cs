using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Jobs;
using MasterServer.DAL;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SkillSystem;
using MasterServer.GameRoom.RoomExtensions;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.ServerInfo;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200081C RID: 2076
	[Service]
	[Singleton]
	internal class GameRoomManager : ServiceModule, IGameRoomManager, IGameRoomManagerRegistry, IDebugGameRoomService
	{
		// Token: 0x06002A9E RID: 10910 RVA: 0x000B73F8 File Offset: 0x000B57F8
		public GameRoomManager(IUserStatusProxy userStatusProxy, IUserRepository userRepository, IJobSchedulerService jobSchedulerService, ILogService logService, ISkillService skillService, IRankSystem rankSystem, IClassPresenceService classPresenceService)
		{
			this.m_userStatusProxy = userStatusProxy;
			this.m_userRepository = userRepository;
			this.m_jobSchedulerService = jobSchedulerService;
			this.m_logService = logService;
			this.m_skillService = skillService;
			this.m_rankSystem = rankSystem;
			this.m_classPresenceService = classPresenceService;
		}

		// Token: 0x140000BA RID: 186
		// (add) Token: 0x06002A9F RID: 10911 RVA: 0x000B7484 File Offset: 0x000B5884
		// (remove) Token: 0x06002AA0 RID: 10912 RVA: 0x000B74BC File Offset: 0x000B58BC
		public event OnRoomOpenedDeleg RoomOpened;

		// Token: 0x140000BB RID: 187
		// (add) Token: 0x06002AA1 RID: 10913 RVA: 0x000B74F4 File Offset: 0x000B58F4
		// (remove) Token: 0x06002AA2 RID: 10914 RVA: 0x000B752C File Offset: 0x000B592C
		public event OnRoomClosedDeleg RoomClosed;

		// Token: 0x140000BC RID: 188
		// (add) Token: 0x06002AA3 RID: 10915 RVA: 0x000B7564 File Offset: 0x000B5964
		// (remove) Token: 0x06002AA4 RID: 10916 RVA: 0x000B759C File Offset: 0x000B599C
		public event OnSessionStartedDeleg SessionStarted;

		// Token: 0x140000BD RID: 189
		// (add) Token: 0x06002AA5 RID: 10917 RVA: 0x000B75D4 File Offset: 0x000B59D4
		// (remove) Token: 0x06002AA6 RID: 10918 RVA: 0x000B760C File Offset: 0x000B5A0C
		public event OnSessionEndedDeleg SessionEnded;

		// Token: 0x06002AA7 RID: 10919 RVA: 0x000B7644 File Offset: 0x000B5A44
		public override void Start()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("GameRoom");
			this.m_emptyRoomLifetime = TimeSpan.FromSeconds((double)int.Parse(section.Get("EmptyRoomLifetimeSec")));
			section.OnConfigChanged += this.OnConfigChanged;
			this.m_jobSchedulerService.AddJob("process_room_cleanup");
			this.m_userRepository.UserLoggedOut += this.OnUserLoggedOut;
			this.m_userStatusProxy.OnUserStatus += this.OnUserStatus;
			this.m_skillService.SkillChanged += this.OnSkillChanged;
			this.m_rankSystem.OnProfileRankChanged += this.OnProfileRankChanged;
			this.m_classPresenceService.ClassPresenceReceived += this.OnClassPresenceReceived;
		}

		// Token: 0x06002AA8 RID: 10920 RVA: 0x000B7714 File Offset: 0x000B5B14
		public override void Stop()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("GameRoom");
			section.OnConfigChanged -= this.OnConfigChanged;
			this.m_userRepository.UserLoggedOut -= this.OnUserLoggedOut;
			this.m_userStatusProxy.OnUserStatus -= this.OnUserStatus;
			this.m_skillService.SkillChanged -= this.OnSkillChanged;
			this.m_rankSystem.OnProfileRankChanged -= this.OnProfileRankChanged;
			this.m_classPresenceService.ClassPresenceReceived -= this.OnClassPresenceReceived;
			this.RoomOpened = null;
			this.RoomClosed = null;
			this.SessionStarted = null;
			this.SessionEnded = null;
		}

		// Token: 0x06002AA9 RID: 10921 RVA: 0x000B77D4 File Offset: 0x000B5BD4
		public IGameRoom GetRoom(ulong room_id)
		{
			object @lock = this.m_lock;
			IGameRoom result;
			lock (@lock)
			{
				IGameRoom gameRoom;
				this.m_rooms.TryGetValue(room_id, out gameRoom);
				result = gameRoom;
			}
			return result;
		}

		// Token: 0x06002AAA RID: 10922 RVA: 0x000B7824 File Offset: 0x000B5C24
		public IGameRoom GetRoom(string idString)
		{
			IGameRoom gameRoom = null;
			ulong num;
			if (!ulong.TryParse(idString, out num))
			{
				Log.Warning<string>("Argument '{0}' is not a valid room id", idString);
			}
			else
			{
				gameRoom = this.GetRoom(num);
				if (gameRoom == null)
				{
					Log.Warning<ulong>("There is no room with id {0}", num);
				}
			}
			return gameRoom;
		}

		// Token: 0x06002AAB RID: 10923 RVA: 0x000B786C File Offset: 0x000B5C6C
		public List<IGameRoom> GetRooms(Predicate<IGameRoom> pred)
		{
			object @lock = this.m_lock;
			List<IGameRoom> source;
			lock (@lock)
			{
				source = this.m_rooms.Values.ToList<IGameRoom>();
			}
			return (from r in source
			where pred(r)
			select r).ToList<IGameRoom>();
		}

		// Token: 0x06002AAC RID: 10924 RVA: 0x000B78E0 File Offset: 0x000B5CE0
		public int TotalPlayersCount(Predicate<IGameRoom> pred)
		{
			List<IGameRoom> rooms = this.GetRooms(pred);
			int playersCount = 0;
			rooms.SafeForEach(delegate(IGameRoom room)
			{
				room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
				{
					playersCount += r.PlayerCount;
				});
			});
			return playersCount;
		}

		// Token: 0x06002AAD RID: 10925 RVA: 0x000B791C File Offset: 0x000B5D1C
		public IGameRoom GetRoomByPlayer(ulong profile_id)
		{
			object @lock = this.m_lock;
			IGameRoom result;
			lock (@lock)
			{
				IGameRoom gameRoom;
				this.m_profile_to_room.TryGetValue(profile_id, out gameRoom);
				result = gameRoom;
			}
			return result;
		}

		// Token: 0x06002AAE RID: 10926 RVA: 0x000B796C File Offset: 0x000B5D6C
		public IGameRoom GetRoomByServer(string server_id)
		{
			object @lock = this.m_lock;
			IGameRoom result;
			lock (@lock)
			{
				IGameRoom gameRoom;
				this.m_server_to_room.TryGetValue(server_id, out gameRoom);
				result = gameRoom;
			}
			return result;
		}

		// Token: 0x06002AAF RID: 10927 RVA: 0x000B79BC File Offset: 0x000B5DBC
		public IGameRoom GetRoomByRoomRef(RoomReference roomRef)
		{
			object @lock = this.m_lock;
			IGameRoom result;
			lock (@lock)
			{
				IGameRoom gameRoom;
				this.m_roomRefToRoom.TryGetValue(roomRef, out gameRoom);
				result = gameRoom;
			}
			return result;
		}

		// Token: 0x06002AB0 RID: 10928 RVA: 0x000B7A0C File Offset: 0x000B5E0C
		public void RemovePlayerByRoomRef(RoomReference roomRef, IEnumerable<PlayerInfoForRoomOffer> players)
		{
			List<ulong> playerData = new List<ulong>();
			foreach (PlayerInfoForRoomOffer playerInfoForRoomOffer in players)
			{
				UserInfo.User user = (!playerInfoForRoomOffer.IsNicknameUsed) ? this.m_userRepository.GetUser(playerInfoForRoomOffer.ProfileId) : this.m_userRepository.GetUser(playerInfoForRoomOffer.Nickname);
				if (user == null)
				{
					Log.Warning<ulong, string>("Player with profile_id: {0} or with nickname: {1} is offline", playerInfoForRoomOffer.ProfileId, playerInfoForRoomOffer.Nickname);
				}
				else
				{
					playerData.Add(user.ProfileID);
				}
			}
			if (playerData.Count == 0)
			{
				throw new GameRoomManagerException("There was no online players from list specified by GI command");
			}
			IGameRoom roomByRoomRef = this.GetRoomByRoomRef(roomRef);
			if (roomByRoomRef == null)
			{
				throw new GameRoomManagerException(string.Format("There is no room with specified roomRef: {0}", roomRef));
			}
			roomByRoomRef.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				foreach (ulong profileId in playerData)
				{
					r.RemovePlayer(profileId, GameRoomPlayerRemoveReason.KickAdmin);
				}
			});
		}

		// Token: 0x06002AB1 RID: 10929 RVA: 0x000B7B24 File Offset: 0x000B5F24
		public void StartRoomByRoomRef(RoomReference roomRef, int team1Score = 0, int team2Score = 0)
		{
			IGameRoom roomByRoomRef = this.GetRoomByRoomRef(roomRef);
			if (roomByRoomRef == null)
			{
				throw new GameRoomManagerException(string.Format("There was no room found with roomRef: {0}", roomRef.Reference));
			}
			roomByRoomRef.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				SessionState state = r.GetState<SessionState>(AccessMode.ReadWrite);
				state.Team1StartScore = team1Score;
				state.Team2StartScore = team2Score;
				r.GetExtension<AutoStartExtension>().TriggerManualStart();
			});
		}

		// Token: 0x06002AB2 RID: 10930 RVA: 0x000B7B7C File Offset: 0x000B5F7C
		public void CleanEmptyRooms()
		{
			foreach (IGameRoom gameRoom in this.GetRooms((IGameRoom r) => DateTime.UtcNow - r.CreationTime > this.m_emptyRoomLifetime))
			{
				try
				{
					gameRoom.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						if (r.IsEmpty)
						{
							r.Close();
						}
					});
				}
				catch (RoomClosedException)
				{
				}
				catch (Exception e)
				{
					Log.Error<ulong>("Error while closing room {0}", gameRoom.ID);
					Log.Error(e);
				}
			}
		}

		// Token: 0x06002AB3 RID: 10931 RVA: 0x000B7C40 File Offset: 0x000B6040
		public void RegisterPlayer(IGameRoom room, ulong profileID)
		{
			object @lock = this.m_lock;
			IGameRoom gameRoom;
			lock (@lock)
			{
				this.m_profile_to_room.TryGetValue(profileID, out gameRoom);
			}
			if (gameRoom != null && gameRoom.ID != room.ID)
			{
				try
				{
					gameRoom.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						r.RemovePlayer(profileID, GameRoomPlayerRemoveReason.Left);
					});
				}
				catch (RoomClosedException)
				{
					Log.Verbose(Log.Group.GameRoom, "Failed to remove user from already closed room", new object[0]);
				}
			}
			object lock2 = this.m_lock;
			lock (lock2)
			{
				this.m_profile_to_room[profileID] = room;
			}
		}

		// Token: 0x06002AB4 RID: 10932 RVA: 0x000B7D34 File Offset: 0x000B6134
		public void UnregisterPlayer(IGameRoom room, ulong profileID)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_profile_to_room.Remove(profileID);
			}
		}

		// Token: 0x06002AB5 RID: 10933 RVA: 0x000B7D80 File Offset: 0x000B6180
		public bool ContainsRoom(RoomReference roomRef)
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				result = this.m_roomRefToRoom.ContainsKey(roomRef);
			}
			return result;
		}

		// Token: 0x06002AB6 RID: 10934 RVA: 0x000B7DCC File Offset: 0x000B61CC
		public bool RegisterRoom(IGameRoom room)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (!room.Reference.Equals(RoomReference.EmptyReference) && this.ContainsRoom(room.Reference))
				{
					Log.Warning(string.Format("Room with same room ref is already created {0}", room.Reference));
					return false;
				}
				this.m_rooms.Add(room.ID, room);
				if (!room.Reference.Equals(RoomReference.EmptyReference))
				{
					this.m_roomRefToRoom.Add(room.Reference, room);
				}
			}
			SessionExtension extension = room.GetExtension<SessionExtension>();
			extension.SessionStarted += this.OnSessionStarted;
			extension.SessionEnded += this.OnSessionEnded;
			ServerExtension extension2 = room.GetExtension<ServerExtension>();
			extension2.ServerBound += this.RoomServerBound;
			extension2.ServerUnbound += this.RoomServerUnbound;
			Log.Info<ulong, GameRoomType>("Game room {0} {1} opened", room.ID, room.Type);
			if (this.RoomOpened != null)
			{
				this.RoomOpened(room);
			}
			return true;
		}

		// Token: 0x06002AB7 RID: 10935 RVA: 0x000B7F10 File Offset: 0x000B6310
		public void UnregisterRoom(IGameRoom room)
		{
			try
			{
				object @lock = this.m_lock;
				bool flag2;
				lock (@lock)
				{
					flag2 = this.m_rooms.Remove(room.ID);
					this.m_roomRefToRoom.Remove(room.Reference);
					string key = this.m_server_to_room.FirstOrDefault((KeyValuePair<string, IGameRoom> x) => x.Value.ID == room.ID).Key;
					if (!string.IsNullOrEmpty(key))
					{
						this.m_server_to_room.Remove(key);
					}
				}
				if (flag2)
				{
					Log.Info<ulong, GameRoomType>("Game room {0} {1} closed", room.ID, room.Type);
					this.CleanupCacheMapping(room.ID);
					if (this.RoomClosed != null)
					{
						this.RoomClosed(room);
					}
				}
			}
			finally
			{
				TimeSpan roomLifeTime = DateTime.UtcNow - room.CreationTime;
				this.m_logService.Event.RoomCloseLog(room.ID, room.RoomName, roomLifeTime);
				room.Dispose();
			}
		}

		// Token: 0x06002AB8 RID: 10936 RVA: 0x000B8074 File Offset: 0x000B6474
		private void CleanupCacheMapping(ulong room_id)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				List<ulong> list = (from e in this.m_profile_to_room
				where e.Value.ID == room_id
				select e.Key).ToList<ulong>();
				List<string> list2 = (from e in this.m_server_to_room
				where e.Value.ID == room_id
				select e.Key).ToList<string>();
				List<string> list3 = (from e in this.m_session_to_room
				where e.Value.ID == room_id
				select e.Key).ToList<string>();
				if (list.Any<ulong>())
				{
					Log.Warning<int>("[GameRoomManager]: Cache inconsistency detected for profile to room mapping, missed {0}", list.Count);
					foreach (ulong key in list)
					{
						this.m_profile_to_room.Remove(key);
					}
				}
				if (list2.Any<string>())
				{
					Log.Warning<int>("[GameRoomManager]: Cache inconsistency detected for server to room mapping, missed {0}", list2.Count);
					foreach (string key2 in list2)
					{
						this.m_server_to_room.Remove(key2);
					}
				}
				if (list3.Any<string>())
				{
					Log.Warning<int>("[GameRoomManager]: Cache inconsistency detected for session to room mapping, missed {0}", list3.Count);
					foreach (string key3 in list3)
					{
						this.m_session_to_room.Remove(key3);
					}
				}
			}
		}

		// Token: 0x06002AB9 RID: 10937 RVA: 0x000B82F4 File Offset: 0x000B66F4
		private void RoomServerBound(IGameRoom room, ServerEntity server)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				string serverID = server.ServerID;
				if (this.m_server_to_room.ContainsKey(serverID))
				{
					IGameRoom gameRoom = this.m_server_to_room[serverID];
					bool flag2 = this.m_rooms.ContainsKey(gameRoom.ID);
					throw new ArgumentException(string.Format("[GameRoomManager]: Server {0} is already bound to {1} room (ID: {2}, Type: {3}, CreationDate: {4}). Unable to bind it to room {5}", new object[]
					{
						serverID,
						(!flag2) ? "unregistered" : "registered",
						gameRoom.ID,
						gameRoom.Type,
						gameRoom.CreationTime.ToLocalTime(),
						room.ID
					}));
				}
				this.m_server_to_room.Add(serverID, room);
			}
		}

		// Token: 0x06002ABA RID: 10938 RVA: 0x000B83EC File Offset: 0x000B67EC
		private void RoomServerUnbound(IGameRoom room, ServerEntity server)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_server_to_room.Remove(server.ServerID);
			}
		}

		// Token: 0x06002ABB RID: 10939 RVA: 0x000B843C File Offset: 0x000B683C
		private void OnSessionStarted(IGameRoom room, string session_id)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_session_to_room.Add(session_id, room);
			}
			if (this.SessionStarted != null)
			{
				this.SessionStarted(room, session_id);
			}
		}

		// Token: 0x06002ABC RID: 10940 RVA: 0x000B84A0 File Offset: 0x000B68A0
		private void OnSessionEnded(IGameRoom room, string sessionId, bool abnormal)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_session_to_room.Remove(sessionId);
			}
			if (this.SessionEnded != null)
			{
				this.SessionEnded(room, sessionId, abnormal);
			}
		}

		// Token: 0x06002ABD RID: 10941 RVA: 0x000B8504 File Offset: 0x000B6904
		private void OnConfigChanged(ConfigEventArgs args)
		{
			if (args.Name.Equals("EmptyRoomLifetimeSec", StringComparison.InvariantCultureIgnoreCase))
			{
				this.m_emptyRoomLifetime = TimeSpan.FromSeconds((double)args.iValue);
			}
		}

		// Token: 0x06002ABE RID: 10942 RVA: 0x000B8530 File Offset: 0x000B6930
		private void OnSkillChanged(ulong profileId, Skill updatedSkill)
		{
			IGameRoom roomByPlayer = this.GetRoomByPlayer(profileId);
			if (roomByPlayer != null)
			{
				SkillType skillTypeByRoomType = SkillTypeHelper.GetSkillTypeByRoomType(roomByPlayer.Type);
				if (updatedSkill.Type == skillTypeByRoomType)
				{
					roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						RoomPlayer player = r.GetPlayer(profileId);
						if (player != null)
						{
							player.Skill = updatedSkill;
						}
					});
				}
			}
		}

		// Token: 0x06002ABF RID: 10943 RVA: 0x000B8594 File Offset: 0x000B6994
		private void OnProfileRankChanged(SProfileInfo profile, SRankInfo newRank, SRankInfo oldRank, ILogGroup logGroup)
		{
			IGameRoom roomByPlayer = this.GetRoomByPlayer(profile.Id);
			if (roomByPlayer != null)
			{
				roomByPlayer.CheckPlayerRank(profile, newRank);
			}
		}

		// Token: 0x06002AC0 RID: 10944 RVA: 0x000B85C0 File Offset: 0x000B69C0
		private void OnClassPresenceReceived(ClassPresenceData data)
		{
			IGameRoom gameRoom;
			if (!this.m_session_to_room.TryGetValue(data.sessionId, out gameRoom))
			{
				return;
			}
			PlayTimeExtension playTimeExtension;
			if (!gameRoom.TryGetExtension<PlayTimeExtension>(out playTimeExtension))
			{
				return;
			}
			playTimeExtension.UpdateClassPresence(data);
		}

		// Token: 0x06002AC1 RID: 10945 RVA: 0x000B85FC File Offset: 0x000B69FC
		public bool SetObserver(ulong profileId, bool enable)
		{
			IGameRoom room = this.GetRoomByPlayer(profileId);
			if (room != null && room.IsPvpMode())
			{
				room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					room.SetObserver(profileId, enable);
					room.SignalPlayersChanged();
				});
				return true;
			}
			return false;
		}

		// Token: 0x06002AC2 RID: 10946 RVA: 0x000B8668 File Offset: 0x000B6A68
		private void OnUserLoggedOut(UserInfo.User user, ELogoutType logoutType)
		{
			IGameRoom roomByPlayer = this.GetRoomByPlayer(user.ProfileID);
			if (roomByPlayer != null)
			{
				try
				{
					GameRoomPlayerRemoveReason reason = (logoutType != ELogoutType.LostConnection) ? GameRoomPlayerRemoveReason.Left : GameRoomPlayerRemoveReason.KickLostConnection;
					roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						r.RemovePlayer(user.ProfileID, reason);
					});
				}
				catch (RoomClosedException)
				{
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		// Token: 0x06002AC3 RID: 10947 RVA: 0x000B8704 File Offset: 0x000B6B04
		private void OnUserStatus(UserStatus prev_status, UserStatus new_status, string onlineId)
		{
			UserInfo.User user = this.m_userRepository.GetUserByOnlineId(onlineId);
			if (user == null)
			{
				return;
			}
			IGameRoom roomByPlayer = this.GetRoomByPlayer(user.ProfileID);
			if (roomByPlayer != null)
			{
				try
				{
					roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						r.UpdatePlayerStatus(user.ProfileID, new_status);
					});
				}
				catch (RoomClosedException)
				{
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		// Token: 0x06002AC4 RID: 10948 RVA: 0x000B879C File Offset: 0x000B6B9C
		public void SetPlayerTeam(ulong roomId, ulong profileId, int team)
		{
			IGameRoom room = this.GetRoom(roomId);
			if (room == null)
			{
				throw new NullReferenceException(string.Format("Can't find room with id {0}", roomId));
			}
			room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				RoomPlayer player = room.GetPlayer(profileId);
				if (player == null)
				{
					throw new NullReferenceException(string.Format("Can't find player {0} in room {1}", profileId, roomId));
				}
				player.TeamID = team;
				r.SignalPlayersChanged();
			});
		}

		// Token: 0x06002AC5 RID: 10949 RVA: 0x000B8814 File Offset: 0x000B6C14
		public void SetTeamForAllPalyers(ulong roomId, int team)
		{
			IGameRoom room = this.GetRoom(roomId);
			if (room == null)
			{
				throw new NullReferenceException(string.Format("Can't find room with id {0}", roomId));
			}
			room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				foreach (RoomPlayer roomPlayer in r.Players)
				{
					roomPlayer.TeamID = team;
				}
				r.SignalPlayersChanged();
			});
		}

		// Token: 0x040016B9 RID: 5817
		public const int INVALID_ROOM_ID = 0;

		// Token: 0x040016BA RID: 5818
		public const int MAX_ROOM_PLAYERS = 16;

		// Token: 0x040016BB RID: 5819
		public const int MAX_ROOM_PLAYERS_PVE = 5;

		// Token: 0x040016BC RID: 5820
		private TimeSpan m_emptyRoomLifetime;

		// Token: 0x040016BD RID: 5821
		private readonly IUserRepository m_userRepository;

		// Token: 0x040016BE RID: 5822
		private readonly IUserStatusProxy m_userStatusProxy;

		// Token: 0x040016BF RID: 5823
		private readonly IJobSchedulerService m_jobSchedulerService;

		// Token: 0x040016C0 RID: 5824
		private readonly ILogService m_logService;

		// Token: 0x040016C1 RID: 5825
		private readonly ISkillService m_skillService;

		// Token: 0x040016C2 RID: 5826
		private readonly IRankSystem m_rankSystem;

		// Token: 0x040016C3 RID: 5827
		private readonly IClassPresenceService m_classPresenceService;

		// Token: 0x040016C4 RID: 5828
		private readonly object m_lock = new object();

		// Token: 0x040016C5 RID: 5829
		private readonly Dictionary<ulong, IGameRoom> m_rooms = new Dictionary<ulong, IGameRoom>();

		// Token: 0x040016C6 RID: 5830
		private readonly Dictionary<ulong, IGameRoom> m_profile_to_room = new Dictionary<ulong, IGameRoom>();

		// Token: 0x040016C7 RID: 5831
		private readonly Dictionary<string, IGameRoom> m_server_to_room = new Dictionary<string, IGameRoom>();

		// Token: 0x040016C8 RID: 5832
		private readonly Dictionary<string, IGameRoom> m_session_to_room = new Dictionary<string, IGameRoom>();

		// Token: 0x040016C9 RID: 5833
		private readonly Dictionary<RoomReference, IGameRoom> m_roomRefToRoom = new Dictionary<RoomReference, IGameRoom>();
	}
}
