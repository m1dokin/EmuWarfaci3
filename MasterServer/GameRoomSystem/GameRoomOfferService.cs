using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Timers;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000529 RID: 1321
	[Service]
	[Singleton]
	internal class GameRoomOfferService : ServiceModule, IGameRoomOfferService
	{
		// Token: 0x06001CAA RID: 7338 RVA: 0x00072CA8 File Offset: 0x000710A8
		public GameRoomOfferService(IQueryManager queryManager, ITimerFactory timerFactory, IGameRoomManager gameRoomManager, ISessionInfoService sessionInfoService, IUserProxyRepository userProxyRepository, IRoomPlayerFactory roomPlayerFactory)
		{
			this.m_queryManager = queryManager;
			this.m_timerFactory = timerFactory;
			this.m_gameRoomManager = gameRoomManager;
			this.m_sessionInfoService = sessionInfoService;
			this.m_userProxyRepository = userProxyRepository;
			this.m_roomPlayerFactory = roomPlayerFactory;
		}

		// Token: 0x06001CAB RID: 7339 RVA: 0x00072CE8 File Offset: 0x000710E8
		public override void Init()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("GameRoom");
			this.m_expirationTimeout = TimeSpan.FromSeconds((double)int.Parse(section.Get("RoomOfferTTLSec")));
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)int.Parse(section.Get("RoomOfferExpirationCheckPeriodSec")));
			this.m_expirationTimer = this.m_timerFactory.CreateTimer(new TimerCallback(this.OnExpirationTick), null, timeSpan, timeSpan);
			section.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x06001CAC RID: 7340 RVA: 0x00072D6C File Offset: 0x0007116C
		public override void Stop()
		{
			if (this.m_expirationTimer != null)
			{
				this.m_expirationTimer.Dispose();
				this.m_expirationTimer = null;
			}
			ConfigSection section = Resources.ModuleSettings.GetSection("GameRoom");
			section.OnConfigChanged -= this.OnConfigChanged;
		}

		// Token: 0x06001CAD RID: 7341 RVA: 0x00072DB8 File Offset: 0x000711B8
		public void OfferRoomByRoomRef(RoomReference roomRef, IEnumerable<PlayerInfoForRoomOffer> players)
		{
			IGameRoom roomByRoomRef = this.m_gameRoomManager.GetRoomByRoomRef(roomRef);
			if (roomByRoomRef == null)
			{
				throw new GameRoomManagerException(string.Format("There was no room found with roomRef: {0}", roomRef));
			}
			this.OfferRoom(roomByRoomRef, players);
		}

		// Token: 0x06001CAE RID: 7342 RVA: 0x00072DF4 File Offset: 0x000711F4
		public void OfferRoomForReconnect(IGameRoom room, ulong profileId)
		{
			UserInfo.User userOrProxyByProfileId = this.m_userProxyRepository.GetUserOrProxyByProfileId(profileId, true);
			RoomPlayer player = this.m_roomPlayerFactory.GetRoomPlayer(userOrProxyByProfileId, room.Type);
			room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				r.ReservePlaceForPlayers(new RoomPlayer[]
				{
					player
				});
			});
			this.OfferRoom("@ui_game_room_offer_system", userOrProxyByProfileId.OnlineID, userOrProxyByProfileId.ProfileID, room.ID.ToString(), false, room);
		}

		// Token: 0x06001CAF RID: 7343 RVA: 0x00072E70 File Offset: 0x00071270
		private void OfferRoom(IGameRoom room, IEnumerable<PlayerInfoForRoomOffer> players)
		{
			List<string> list = new List<string>();
			List<ulong> list2 = new List<ulong>();
			List<PlayerInfoForRoomOffer> playersList = players.ToList<PlayerInfoForRoomOffer>();
			foreach (PlayerInfoForRoomOffer playerInfoForRoomOffer in playersList)
			{
				if (playerInfoForRoomOffer.IsNicknameUsed)
				{
					list.Add(playerInfoForRoomOffer.Nickname);
				}
				else
				{
					list2.Add(playerInfoForRoomOffer.ProfileId);
				}
			}
			Task<List<ProfileInfo>> profileInfoAsync = this.m_sessionInfoService.GetProfileInfoAsync(list, list2);
			profileInfoAsync.ContinueWith(delegate(Task<List<ProfileInfo>> t)
			{
				List<ProfileInfo> result = t.Result;
				List<RoomPlayer> list3 = new List<RoomPlayer>();
				using (List<ProfileInfo>.Enumerator enumerator2 = result.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ProfileInfo info = enumerator2.Current;
						if (UserStatuses.IsPreGame(info.Status))
						{
							UserInfo.User userOrProxyByNickname = this.m_userProxyRepository.GetUserOrProxyByNickname(info.Nickname);
							PlayerInfoForRoomOffer playerInfoForRoomOffer2 = playersList.First((PlayerInfoForRoomOffer x) => x.ProfileId == info.ProfileID || x.Nickname == info.Nickname);
							GameRoomType type = room.Type;
							RoomPlayer roomPlayer = this.m_roomPlayerFactory.GetRoomPlayer(userOrProxyByNickname, type);
							roomPlayer.OnlineID = info.OnlineID;
							roomPlayer.GroupID = playerInfoForRoomOffer2.GroupId;
							list3.Add(roomPlayer);
						}
						else
						{
							Log.Warning<ulong, string, UserStatus>("Player with profile_id: {0}, nickname: {1} is {2}", info.ProfileID, info.Nickname, info.Status);
						}
					}
				}
				this.OfferRoom(room, list3);
			});
		}

		// Token: 0x06001CB0 RID: 7344 RVA: 0x00072F40 File Offset: 0x00071340
		private void OfferRoom(IGameRoom room, IEnumerable<RoomPlayer> players)
		{
			foreach (RoomPlayer roomPlayer in players)
			{
				bool hasPlayer = false;
				RoomPlayer player = roomPlayer;
				room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					hasPlayer = r.HasPlayer(player.ProfileID);
					if (!hasPlayer)
					{
						r.ReservePlaceForPlayers(new RoomPlayer[]
						{
							player
						});
					}
				});
				if (!hasPlayer)
				{
					this.OfferRoom("@ui_game_room_offer_system", roomPlayer.OnlineID, roomPlayer.ProfileID, roomPlayer.GroupID, false, room);
				}
				else
				{
					Log.Warning<ulong, string, RoomReference>("Player with profile_id: {0}, nickname: {1} already in room with roomRef {2}", roomPlayer.ProfileID, roomPlayer.Nickname, room.Reference);
				}
			}
		}

		// Token: 0x06001CB1 RID: 7345 RVA: 0x00073000 File Offset: 0x00071400
		public bool OfferRoom(string fromNickname, UserInfo.User toUser, string token, IGameRoom room)
		{
			return this.OfferRoom(fromNickname, new UserInfo.User[]
			{
				toUser
			}, token, room);
		}

		// Token: 0x06001CB2 RID: 7346 RVA: 0x00073018 File Offset: 0x00071418
		public bool OfferRoom(string fromNickname, IEnumerable<UserInfo.User> toUsers, string token, IGameRoom room)
		{
			bool res = false;
			RoomPlayer[] players = toUsers.Select(delegate(UserInfo.User user)
			{
				RoomPlayer roomPlayer = this.m_roomPlayerFactory.GetRoomPlayer(user, room.Type);
				roomPlayer.GroupID = token;
				return roomPlayer;
			}).ToArray<RoomPlayer>();
			room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				res = r.ReservePlaceForPlayers(players);
			});
			if (!res)
			{
				return false;
			}
			foreach (UserInfo.User user2 in toUsers)
			{
				this.OfferRoom(fromNickname, user2.OnlineID, user2.ProfileID, token, true, room);
			}
			return true;
		}

		// Token: 0x06001CB3 RID: 7347 RVA: 0x000730EC File Offset: 0x000714EC
		private void OfferRoom(string fromNickname, string onlineId, ulong profileId, string token, bool silent, IGameRoom room)
		{
			GameRoomOfferService.GameRoomOffer offer = new GameRoomOfferService.GameRoomOffer
			{
				Guid = Guid.NewGuid(),
				RoomId = room.ID,
				ProfileId = profileId
			};
			object offers = this.m_offers;
			lock (offers)
			{
				this.m_offers.Add(offer.Guid, offer);
			}
			Task<object> task = this.m_queryManager.RequestAsync("gameroom_offer", onlineId, new object[]
			{
				fromNickname,
				room,
				offer.Guid,
				token,
				silent
			});
			task.ContinueWith(delegate(Task<object> t)
			{
				if (t.IsFaulted)
				{
					Log.Error(t.Exception);
					this.OnResponse(offer.Guid, false);
				}
			});
		}

		// Token: 0x06001CB4 RID: 7348 RVA: 0x000731D8 File Offset: 0x000715D8
		public void OnResponse(Guid guid, bool accepted)
		{
			object offers = this.m_offers;
			GameRoomOfferService.GameRoomOffer gameRoomOffer;
			lock (offers)
			{
				if (this.m_offers.TryGetValue(guid, out gameRoomOffer))
				{
					this.m_offers.Remove(guid);
				}
				else
				{
					Log.Error("[GameRoomOfferService.OnResponse] unexpected response");
				}
			}
			if (!accepted && gameRoomOffer != null)
			{
				this.CleanUp(gameRoomOffer);
			}
		}

		// Token: 0x06001CB5 RID: 7349 RVA: 0x00073258 File Offset: 0x00071658
		private void OnExpirationTick(object dummy)
		{
			object offers = this.m_offers;
			List<GameRoomOfferService.GameRoomOffer> list;
			lock (offers)
			{
				list = (from el in this.m_offers.Values
				where DateTime.UtcNow - el.CreatedAt > this.m_expirationTimeout
				select el).ToList<GameRoomOfferService.GameRoomOffer>();
				foreach (GameRoomOfferService.GameRoomOffer gameRoomOffer in list)
				{
					this.m_offers.Remove(gameRoomOffer.Guid);
				}
			}
			foreach (GameRoomOfferService.GameRoomOffer offer in list)
			{
				this.CleanUp(offer);
			}
		}

		// Token: 0x06001CB6 RID: 7350 RVA: 0x00073354 File Offset: 0x00071754
		private void CleanUp(GameRoomOfferService.GameRoomOffer offer)
		{
			IGameRoom room = this.m_gameRoomManager.GetRoom(offer.RoomId);
			if (room != null)
			{
				room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					r.RemoveReservation(offer.ProfileId, ReservationRemovedReason.CleanUp);
				});
			}
		}

		// Token: 0x06001CB7 RID: 7351 RVA: 0x000733A0 File Offset: 0x000717A0
		private void OnConfigChanged(ConfigEventArgs args)
		{
			if (args.Name.Equals("RoomOfferTTLSec", StringComparison.InvariantCultureIgnoreCase))
			{
				this.m_expirationTimeout = TimeSpan.FromSeconds((double)args.iValue);
			}
			else if (args.Name.Equals("RoomOfferExpirationCheckPeriodSec", StringComparison.InvariantCultureIgnoreCase))
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds((double)args.iValue);
				this.m_expirationTimer.Change(timeSpan, timeSpan);
			}
		}

		// Token: 0x04000DA3 RID: 3491
		private const string SystemOffer = "@ui_game_room_offer_system";

		// Token: 0x04000DA4 RID: 3492
		private static readonly string DefaultToken = string.Empty;

		// Token: 0x04000DA5 RID: 3493
		private readonly IQueryManager m_queryManager;

		// Token: 0x04000DA6 RID: 3494
		private readonly ITimerFactory m_timerFactory;

		// Token: 0x04000DA7 RID: 3495
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000DA8 RID: 3496
		private readonly ISessionInfoService m_sessionInfoService;

		// Token: 0x04000DA9 RID: 3497
		private readonly IUserProxyRepository m_userProxyRepository;

		// Token: 0x04000DAA RID: 3498
		private readonly IRoomPlayerFactory m_roomPlayerFactory;

		// Token: 0x04000DAB RID: 3499
		private readonly Dictionary<Guid, GameRoomOfferService.GameRoomOffer> m_offers = new Dictionary<Guid, GameRoomOfferService.GameRoomOffer>();

		// Token: 0x04000DAC RID: 3500
		private ITimer m_expirationTimer;

		// Token: 0x04000DAD RID: 3501
		private TimeSpan m_expirationTimeout;

		// Token: 0x0200052A RID: 1322
		private class GameRoomOffer
		{
			// Token: 0x04000DAE RID: 3502
			public Guid Guid;

			// Token: 0x04000DAF RID: 3503
			public readonly DateTime CreatedAt = DateTime.UtcNow;

			// Token: 0x04000DB0 RID: 3504
			public ulong RoomId;

			// Token: 0x04000DB1 RID: 3505
			public ulong ProfileId;
		}
	}
}
