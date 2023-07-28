using System;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Services;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.GameLogic.InvitationSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Matchmaking;

namespace MasterServer.Users
{
	// Token: 0x020007FC RID: 2044
	[Service]
	[Singleton]
	internal class UserInvitation : ServiceModule, IUserInvitation
	{
		// Token: 0x060029ED RID: 10733 RVA: 0x000B4B4C File Offset: 0x000B2F4C
		public UserInvitation(ISessionInfoService sessionInfoService, IQueryManager queryManager, IGameRoomManager gameRoomManager, IUserRepository userRepository, IUserProxyRepository userProxyRepository, IMatchmakingSystem matchmakingSystem, ILogService logService, IUserInvitationTicketRepository ticketRepository, IClanService clanService, IConfigProvider<UserInvitationConfig> configProvider)
		{
			this.m_sessionInfoService = sessionInfoService;
			this.m_queryManager = queryManager;
			this.m_gameRoomManager = gameRoomManager;
			this.m_userRepository = userRepository;
			this.m_userProxyRepository = userProxyRepository;
			this.m_matchmakingSystem = matchmakingSystem;
			this.m_logService = logService;
			this.m_ticketRepository = ticketRepository;
			this.m_clanService = clanService;
			this.m_configProvider = configProvider;
		}

		// Token: 0x060029EE RID: 10734 RVA: 0x000B4BB7 File Offset: 0x000B2FB7
		public override void Init()
		{
			base.Init();
			this.m_ticketRepository.TicketExpired += this.OnTicketExpired;
		}

		// Token: 0x060029EF RID: 10735 RVA: 0x000B4BD6 File Offset: 0x000B2FD6
		public override void Stop()
		{
			this.m_ticketRepository.TicketExpired -= this.OnTicketExpired;
			this.m_ticketRepository.Dispose();
			base.Stop();
		}

		// Token: 0x060029F0 RID: 10736 RVA: 0x000B4C00 File Offset: 0x000B3000
		private string GetInviteId(ulong roomId, ulong profileId, string nickname)
		{
			return string.Format("{0}_{1}_{2}", roomId, profileId, nickname);
		}

		// Token: 0x060029F1 RID: 10737 RVA: 0x000B4C19 File Offset: 0x000B3019
		public EInvitationStatus SendInvitation(UserInfo.User fromUser, UserInfo.User toUser, string groupId, string isFollow)
		{
			return this.SendInvitation(fromUser, toUser, groupId, isFollow, false);
		}

		// Token: 0x060029F2 RID: 10738 RVA: 0x000B4C28 File Offset: 0x000B3028
		public EInvitationStatus SendInvitation(UserInfo.User fromUser, UserInfo.User toUser, string groupId, string isFollow, bool isSilent)
		{
			if (toUser == null)
			{
				return EInvitationStatus.TargetInvalid;
			}
			if (Resources.BootstrapMode && !this.m_userRepository.IsSameBootstrap(fromUser.UserID, toUser.UserID))
			{
				return EInvitationStatus.TargetInvalid;
			}
			IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(fromUser.ProfileID);
			EInvitationStatus status = EInvitationStatus.InvalidState;
			if (roomByPlayer != null)
			{
				roomByPlayer.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
				{
					status = this.CanJoinRoom(r, toUser, isFollow == "1");
				});
			}
			UserInvitationConfig config = this.m_configProvider.Get();
			object @lock = this.m_lock;
			UserInvitation.Ticket ticket;
			lock (@lock)
			{
				string inviteId = this.GetInviteId((roomByPlayer == null) ? 0UL : roomByPlayer.ID, fromUser.ProfileID, toUser.Nickname);
				if (this.m_ticketRepository.TryGetValue(inviteId, out ticket))
				{
					status = ((!ticket.IsFollow) ? EInvitationStatus.Duplicate : EInvitationStatus.DuplicateFollow);
				}
				ticket = new UserInvitation.Ticket(inviteId, fromUser, toUser.Nickname, (roomByPlayer == null) ? 0UL : roomByPlayer.ID, isFollow, isSilent);
				ticket.OptionalInviteeUserID = toUser.UserID;
				if (config.UseGroups)
				{
					ticket.GroupId = groupId;
				}
				if (status == EInvitationStatus.Pending)
				{
					this.m_ticketRepository.Add(inviteId, ticket);
				}
			}
			if (roomByPlayer != null && status == EInvitationStatus.Pending)
			{
				roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					RoomPlayer player = r.GetPlayer(fromUser.ProfileID);
					if (player != null && !player.HasGroup && config.UseGroups)
					{
						player.GroupID = groupId;
					}
				});
			}
			if (!string.IsNullOrEmpty(toUser.OnlineID))
			{
				ticket.To.OnlineID = toUser.OnlineID;
				ticket.To.ProfileID = toUser.ProfileID;
				this.OnTargetResolved(ticket, status);
			}
			else
			{
				this.m_sessionInfoService.GetProfileInfo(toUser.Nickname, delegate(ProfileInfo pi)
				{
					ticket.To.OnlineID = pi.OnlineID;
					ticket.To.ProfileID = pi.ProfileID;
					this.OnTargetResolved(ticket, status);
				});
			}
			if (status != EInvitationStatus.Pending)
			{
				this.m_logService.Event.PlayerInviteFailedLog(fromUser.ProfileID, toUser.ProfileID, status);
			}
			return status;
		}

		// Token: 0x060029F3 RID: 10739 RVA: 0x000B4F04 File Offset: 0x000B3304
		private void OnTargetResolved(UserInvitation.Ticket ticket, EInvitationStatus status)
		{
			if (string.IsNullOrEmpty(ticket.To.OnlineID))
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					this.m_ticketRepository.Remove(ticket.ID);
				}
				if (!ticket.isSilent)
				{
					this.m_queryManager.Request("invitation_result", ticket.From.OnlineID, new object[]
					{
						EInvitationStatus.UserOffline,
						ticket.To.Nickname,
						ticket.isFollow,
						ticket.OptionalInviteeUserID
					});
				}
				return;
			}
			if (status == EInvitationStatus.Pending)
			{
				CommonInitiatorData commonInitiatorData = CommonInitiatorData.CreateInitiatorData(this.m_clanService, this.m_userRepository, ticket.From.ProfileID);
				Task<object> task = this.m_queryManager.RequestAsync("invitation_request", ticket.To.OnlineID, new object[]
				{
					ticket.From.Nickname,
					ticket.ID,
					ticket.RoomID,
					ticket.isFollow,
					ticket.GroupId,
					commonInitiatorData
				});
				task.ContinueWith(delegate(Task<object> t)
				{
					if (t.IsFaulted)
					{
						Log.Error(t.Exception);
						if (!ticket.isSilent)
						{
							this.InvitationResponse(ticket.ID, EInvitationStatus.ServiceError);
						}
					}
				});
			}
			else if (!ticket.isSilent && ticket.IsFollow)
			{
				this.m_queryManager.Request("invitation_result", ticket.To.OnlineID, new object[]
				{
					status,
					ticket.To.Nickname,
					ticket.isFollow,
					ticket.OptionalInviteeUserID
				});
			}
		}

		// Token: 0x060029F4 RID: 10740 RVA: 0x000B5140 File Offset: 0x000B3540
		public EInvitationStatus InvitationResponse(string ticketID, EInvitationStatus status)
		{
			object @lock = this.m_lock;
			UserInvitation.Ticket ticket;
			lock (@lock)
			{
				if (!this.m_ticketRepository.TryGetValue(ticketID, out ticket))
				{
					return EInvitationStatus.Expired;
				}
				this.m_ticketRepository.Remove(ticket.ID);
			}
			IGameRoom room = this.m_gameRoomManager.GetRoom(ticket.RoomID);
			if (status == EInvitationStatus.Accepted)
			{
				this.m_matchmakingSystem.UnQueueEntity(ticket.To.ProfileID, EUnQueueReason.GetInvited);
				try
				{
					UserInfo.User user = this.m_userProxyRepository.GetUserOrProxyByProfileId(ticket.To.ProfileID);
					EInvitationStatus result = EInvitationStatus.InvalidState;
					if (room != null)
					{
						room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
						{
							result = this.CanJoinRoom(r, user, ticket.IsFollow);
						});
						if (result == EInvitationStatus.Pending)
						{
							IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(ticket.To.ProfileID);
							if (roomByPlayer != null && roomByPlayer.ID != room.ID)
							{
								try
								{
									roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
									{
										r.RemovePlayer(ticket.To.ProfileID, GameRoomPlayerRemoveReason.Left);
									});
								}
								catch (RoomClosedException)
								{
									Log.Verbose(Log.Group.GameRoom, "Failed to remove user '{0}' from already closed room '{1}'", new object[]
									{
										ticket.To.ProfileID,
										roomByPlayer.ID
									});
								}
							}
						}
						room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
						{
							result = this.CanJoinRoom(r, user, ticket.IsFollow);
							if (result == EInvitationStatus.Pending)
							{
								r.ReservePlaceForPlayer(user, ticket.GroupId);
							}
						});
					}
					if (result != EInvitationStatus.Pending)
					{
						status = result;
						this.m_logService.Event.PlayerInviteFailedLog(ticket.From.ProfileID, ticket.To.ProfileID, status);
					}
				}
				catch (RoomClosedException)
				{
				}
			}
			if (room != null && status != EInvitationStatus.Accepted)
			{
				room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					r.RemoveReservation(ticket.To.ProfileID, ReservationRemovedReason.CleanUp);
				});
			}
			if (!ticket.isSilent)
			{
				this.m_queryManager.Request("invitation_result", ticket.From.OnlineID, new object[]
				{
					status,
					ticket.To.Nickname,
					ticket.isFollow,
					ticket.OptionalInviteeUserID
				});
			}
			return status;
		}

		// Token: 0x060029F5 RID: 10741 RVA: 0x000B5424 File Offset: 0x000B3824
		private EInvitationStatus CanJoinRoom(IGameRoom room, UserInfo.User user, bool isFollow)
		{
			EInvitationStatus result = EInvitationStatus.Pending;
			RoomPlayer player = room.GetPlayer(user.ProfileID);
			GameRoomRetCode p;
			if (player != null)
			{
				p = GameRoomRetCode.ERROR;
			}
			else if (!room.AllowManualJoin || (isFollow && (room.Private || room.Locked)))
			{
				p = GameRoomRetCode.PRIVATE;
			}
			else
			{
				p = room.CanJoin(user);
			}
			switch (p)
			{
			case GameRoomRetCode.OK:
				return EInvitationStatus.Pending;
			case GameRoomRetCode.BANNED:
				return EInvitationStatus.Banned;
			case GameRoomRetCode.FULL:
				return EInvitationStatus.FullRoom;
			case GameRoomRetCode.ERROR:
				return (!isFollow) ? EInvitationStatus.Duplicate : EInvitationStatus.DuplicateFollow;
			case GameRoomRetCode.PRIVATE:
				return EInvitationStatus.PrivateRoom;
			case GameRoomRetCode.BUILD_TYPE_MISMATCH:
				return EInvitationStatus.BuilTypeMismatch;
			case GameRoomRetCode.RANK_RESTRICTED:
				return EInvitationStatus.RankRestricted;
			case GameRoomRetCode.MISSION_RESTRICTED:
				return EInvitationStatus.MissionRestricted;
			case GameRoomRetCode.NOT_IN_CLAN:
				return EInvitationStatus.NotInClan;
			case GameRoomRetCode.NOT_PARTICIPATE_IN_CLAN_WAR:
				return EInvitationStatus.NotParticipateToClanWar;
			case GameRoomRetCode.CLASS_RESTRICTED:
				return EInvitationStatus.ClassRestricted;
			case GameRoomRetCode.BUILD_VERSION_MISMATCH:
				return EInvitationStatus.BuildVersionMismatch;
			case GameRoomRetCode.ITEM_NOT_AVAILABLE:
				return EInvitationStatus.ItemNotAvailable;
			case GameRoomRetCode.NOT_PARTICIPATE_IN_RATING_GAME:
				return EInvitationStatus.NotParticipateInRatingGame;
			}
			Log.Warning<GameRoomRetCode>("No handler for invite error: {0}", p);
			return result;
		}

		// Token: 0x060029F6 RID: 10742 RVA: 0x000B5584 File Offset: 0x000B3984
		private void OnTicketExpired(UserInvitation.Ticket expiredTicket)
		{
			if (!expiredTicket.isSilent)
			{
				this.m_queryManager.Request("invitation_result", expiredTicket.From.OnlineID, new object[]
				{
					EInvitationStatus.Expired,
					expiredTicket.To.Nickname,
					expiredTicket.isFollow,
					expiredTicket.OptionalInviteeUserID
				});
			}
		}

		// Token: 0x04001644 RID: 5700
		private readonly ISessionInfoService m_sessionInfoService;

		// Token: 0x04001645 RID: 5701
		private readonly IQueryManager m_queryManager;

		// Token: 0x04001646 RID: 5702
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04001647 RID: 5703
		private readonly IUserRepository m_userRepository;

		// Token: 0x04001648 RID: 5704
		private readonly IUserProxyRepository m_userProxyRepository;

		// Token: 0x04001649 RID: 5705
		private readonly IMatchmakingSystem m_matchmakingSystem;

		// Token: 0x0400164A RID: 5706
		private readonly ILogService m_logService;

		// Token: 0x0400164B RID: 5707
		private readonly IUserInvitationTicketRepository m_ticketRepository;

		// Token: 0x0400164C RID: 5708
		private readonly IClanService m_clanService;

		// Token: 0x0400164D RID: 5709
		private readonly IConfigProvider<UserInvitationConfig> m_configProvider;

		// Token: 0x0400164E RID: 5710
		private readonly object m_lock = new object();

		// Token: 0x020007FD RID: 2045
		internal struct Credentials
		{
			// Token: 0x0400164F RID: 5711
			public string OnlineID;

			// Token: 0x04001650 RID: 5712
			public ulong ProfileID;

			// Token: 0x04001651 RID: 5713
			public string Nickname;
		}

		// Token: 0x020007FE RID: 2046
		internal class Ticket
		{
			// Token: 0x060029F7 RID: 10743 RVA: 0x000B55EC File Offset: 0x000B39EC
			public Ticket(string id, UserInfo.User from, string to_nickname, ulong room_id, string is_follow, bool is_silent)
			{
				this.ID = id;
				this.RoomID = room_id;
				this.From = new UserInvitation.Credentials
				{
					OnlineID = from.OnlineID,
					ProfileID = from.ProfileID,
					Nickname = from.Nickname
				};
				this.To = new UserInvitation.Credentials
				{
					Nickname = to_nickname
				};
				this.CreatedAt = DateTime.Now;
				this.isFollow = is_follow;
				this.isSilent = is_silent;
				this.OptionalInviteeUserID = 0UL;
				this.GroupId = string.Empty;
			}

			// Token: 0x170003DD RID: 989
			// (get) Token: 0x060029F8 RID: 10744 RVA: 0x000B5689 File Offset: 0x000B3A89
			public bool IsFollow
			{
				get
				{
					return this.isFollow == "1";
				}
			}

			// Token: 0x04001652 RID: 5714
			public string ID;

			// Token: 0x04001653 RID: 5715
			public UserInvitation.Credentials From;

			// Token: 0x04001654 RID: 5716
			public UserInvitation.Credentials To;

			// Token: 0x04001655 RID: 5717
			public ulong RoomID;

			// Token: 0x04001656 RID: 5718
			public DateTime CreatedAt;

			// Token: 0x04001657 RID: 5719
			public string isFollow;

			// Token: 0x04001658 RID: 5720
			public bool isSilent;

			// Token: 0x04001659 RID: 5721
			public ulong OptionalInviteeUserID;

			// Token: 0x0400165A RID: 5722
			public string GroupId;
		}
	}
}
