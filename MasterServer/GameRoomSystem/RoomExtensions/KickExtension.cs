using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020005FB RID: 1531
	[RoomExtension]
	internal class KickExtension : RoomExtensionBase
	{
		// Token: 0x06002095 RID: 8341 RVA: 0x00085436 File Offset: 0x00083836
		public KickExtension(IClientVersionsManagementService clientVersionsManagementService, IUserRepository userRepository)
		{
			this.m_clientVersionsManagementService = clientVersionsManagementService;
			this.m_userRepository = userRepository;
		}

		// Token: 0x14000081 RID: 129
		// (add) Token: 0x06002096 RID: 8342 RVA: 0x00085470 File Offset: 0x00083870
		// (remove) Token: 0x06002097 RID: 8343 RVA: 0x000854A8 File Offset: 0x000838A8
		internal event KickExtension.TrOnPlayerKickDeleg tr_player_kick;

		// Token: 0x06002098 RID: 8344 RVA: 0x000854E0 File Offset: 0x000838E0
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			base.Room.tr_player_add_check += this.PlayerAddCheck;
			base.Room.tr_player_removed += this.OnPlayerRemoved;
			base.Room.tr_player_status += this.OnPlayerStatus;
			base.Room.PlayerRemoved += this.Room_PlayerRemoved;
			this.m_clientVersionsManagementService.ClientVersionsChanged += this.OnClientVersionsChanged;
		}

		// Token: 0x06002099 RID: 8345 RVA: 0x00085568 File Offset: 0x00083968
		public override void Close()
		{
			base.Close();
			base.Room.tr_player_add_check -= this.PlayerAddCheck;
			base.Room.tr_player_removed -= this.OnPlayerRemoved;
			base.Room.tr_player_status -= this.OnPlayerStatus;
			base.Room.PlayerRemoved -= this.Room_PlayerRemoved;
			this.m_clientVersionsManagementService.ClientVersionsChanged -= this.OnClientVersionsChanged;
			this.tr_player_kick = null;
		}

		// Token: 0x0600209A RID: 8346 RVA: 0x000855F8 File Offset: 0x000839F8
		private void OnPlayerRemoved(RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_defferedkicks.Remove(player.ProfileID);
			}
		}

		// Token: 0x0600209B RID: 8347 RVA: 0x00085648 File Offset: 0x00083A48
		private void Room_PlayerRemoved(IGameRoom room, RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			if (!reason.IsOneOf(new GameRoomPlayerRemoveReason[]
			{
				GameRoomPlayerRemoveReason.Left,
				GameRoomPlayerRemoveReason.KickLostConnection
			}))
			{
				QueryManager.RequestSt("gameroom_on_kicked", player.OnlineID, new object[]
				{
					reason
				});
			}
		}

		// Token: 0x0600209C RID: 8348 RVA: 0x00085680 File Offset: 0x00083A80
		private GameRoomRetCode PlayerAddCheck(RoomPlayer player)
		{
			base.Room.CheckAccessMode(AccessMode.ReadOnly);
			object @lock = this.m_lock;
			bool flag2;
			lock (@lock)
			{
				flag2 = this.m_banlist.Contains(player.ProfileID);
			}
			return (!flag2) ? GameRoomRetCode.OK : GameRoomRetCode.BANNED;
		}

		// Token: 0x0600209D RID: 8349 RVA: 0x000856E8 File Offset: 0x00083AE8
		public void BanPlayer(ulong profile_id)
		{
			base.Room.CheckAccessMode(AccessMode.ReadWrite);
			if (base.Room.HasPlayer(profile_id))
			{
				throw new ApplicationException("Can't ban player while he is in room");
			}
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_banlist.Add(profile_id);
			}
			Log.Verbose(Log.Group.GameRoom, "Player {0} was banned in room {1}", new object[]
			{
				profile_id,
				base.Room.ID
			});
		}

		// Token: 0x0600209E RID: 8350 RVA: 0x00085788 File Offset: 0x00083B88
		public void UnbanPlayer(ulong profile_id)
		{
			base.Room.CheckAccessMode(AccessMode.ReadWrite);
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_banlist.Remove(profile_id);
			}
			Log.Verbose(Log.Group.GameRoom, "Player {0} was unbanned in room {1}", new object[]
			{
				profile_id,
				base.Room.ID
			});
		}

		// Token: 0x0600209F RID: 8351 RVA: 0x0008580C File Offset: 0x00083C0C
		public void UnbanPlayers()
		{
			base.Room.CheckAccessMode(AccessMode.ReadWrite);
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_banlist.Clear();
			}
		}

		// Token: 0x060020A0 RID: 8352 RVA: 0x00085860 File Offset: 0x00083C60
		public void KickPlayer(ulong target_pid, GameRoomPlayerRemoveReason reason)
		{
			this.KickPlayer(target_pid, reason, false);
		}

		// Token: 0x060020A1 RID: 8353 RVA: 0x0008586C File Offset: 0x00083C6C
		public void KickPlayer(ulong target_pid, GameRoomPlayerRemoveReason reason, bool ignoreDeffered)
		{
			if (!ignoreDeffered && this.IsDefferedKick(target_pid, reason))
			{
				return;
			}
			if (base.Room.HasPlayer(target_pid))
			{
				RoomPlayer player = base.Room.GetPlayer(target_pid);
				if (this.tr_player_kick != null)
				{
					this.tr_player_kick(player, reason);
				}
				base.Room.RemovePlayer(target_pid, reason);
			}
			if (reason != GameRoomPlayerRemoveReason.KickVersionMismatch && reason != GameRoomPlayerRemoveReason.KickRankRestricted && reason != GameRoomPlayerRemoveReason.KickItemNotAvalaible && reason != GameRoomPlayerRemoveReason.KickHighLatency)
			{
				this.BanPlayer(target_pid);
			}
			ServerExtension extension = base.Room.GetExtension<ServerExtension>();
			if (extension.GameRunning)
			{
				QueryManager.RequestSt("srv_player_kicked", extension.ServerOnlineID, new object[]
				{
					target_pid,
					reason
				});
			}
			Log.Verbose(Log.Group.GameRoom, "Player {0} was kicked from room {1}: {2}", new object[]
			{
				target_pid,
				base.Room.ID,
				reason
			});
		}

		// Token: 0x060020A2 RID: 8354 RVA: 0x0008596C File Offset: 0x00083D6C
		private bool IsDefferedKick(ulong target_pid, GameRoomPlayerRemoveReason reason)
		{
			if (reason.IsOneOf(new GameRoomPlayerRemoveReason[]
			{
				GameRoomPlayerRemoveReason.KickRankRestricted,
				GameRoomPlayerRemoveReason.KickVersionMismatch
			}))
			{
				RoomPlayer player = base.Room.GetPlayer(target_pid);
				if (player != null && UserStatuses.IsInGame(player.UserStatus))
				{
					object @lock = this.m_lock;
					lock (@lock)
					{
						if (!this.m_defferedkicks.ContainsKey(target_pid))
						{
							this.m_defferedkicks.Add(target_pid, reason);
						}
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x060020A3 RID: 8355 RVA: 0x00085A08 File Offset: 0x00083E08
		private void HandleDefferedKick(ulong profileId, UserStatus status)
		{
			object @lock = this.m_lock;
			GameRoomPlayerRemoveReason reason;
			bool flag2;
			lock (@lock)
			{
				flag2 = this.m_defferedkicks.TryGetValue(profileId, out reason);
			}
			if (flag2 && UserStatuses.IsPreGame(status))
			{
				this.KickPlayer(profileId, reason, true);
			}
		}

		// Token: 0x060020A4 RID: 8356 RVA: 0x00085A70 File Offset: 0x00083E70
		private void OnPlayerStatus(ulong profileId, UserStatus old_status, UserStatus new_status)
		{
			base.Room.CheckAccessMode(AccessMode.ReadWrite);
			this.HandleDefferedKick(profileId, new_status);
		}

		// Token: 0x060020A5 RID: 8357 RVA: 0x00085A86 File Offset: 0x00083E86
		private void OnClientVersionsChanged()
		{
			base.Room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				base.Room.Players.ToList<RoomPlayer>().SafeForEach(delegate(RoomPlayer p)
				{
					UserInfo.User userByUserId = this.m_userRepository.GetUserByUserId(p.UserID);
					if (userByUserId == null)
					{
						return;
					}
					if (!this.m_clientVersionsManagementService.Validate(userByUserId.Version))
					{
						this.KickPlayer(p.ProfileID, GameRoomPlayerRemoveReason.KickVersionMismatch, false);
					}
				});
			});
		}

		// Token: 0x04000FFF RID: 4095
		private readonly object m_lock = new object();

		// Token: 0x04001000 RID: 4096
		private readonly List<ulong> m_banlist = new List<ulong>();

		// Token: 0x04001001 RID: 4097
		private readonly Dictionary<ulong, GameRoomPlayerRemoveReason> m_defferedkicks = new Dictionary<ulong, GameRoomPlayerRemoveReason>();

		// Token: 0x04001002 RID: 4098
		private readonly IClientVersionsManagementService m_clientVersionsManagementService;

		// Token: 0x04001003 RID: 4099
		private readonly IUserRepository m_userRepository;

		// Token: 0x020005FC RID: 1532
		// (Invoke) Token: 0x060020A9 RID: 8361
		internal delegate void TrOnPlayerKickDeleg(RoomPlayer player, GameRoomPlayerRemoveReason reason);
	}
}
