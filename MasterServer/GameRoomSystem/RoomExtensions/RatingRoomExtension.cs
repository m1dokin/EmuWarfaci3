using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Timers;
using MasterServer.GameLogic.RatingSystem;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020004C2 RID: 1218
	[RoomExtension]
	internal class RatingRoomExtension : RoomExtensionBase
	{
		// Token: 0x06001A57 RID: 6743 RVA: 0x0006C584 File Offset: 0x0006A984
		public RatingRoomExtension(ITimerFactory timerFactory, IConfigProvider<RatingRoomConfig> config, IRatingGameBanService ratingGameBanService)
		{
			this.m_timerFactory = timerFactory;
			this.m_config = config;
			this.m_ratingGameBanService = ratingGameBanService;
		}

		// Token: 0x06001A58 RID: 6744 RVA: 0x0006C5AC File Offset: 0x0006A9AC
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			base.Room.tr_player_add_check += this.OnTrPlayerAddCheck;
			base.Room.tr_player_added += this.OnTrPlayerAdded;
			base.Room.tr_player_removed += this.OnTrPlayerRemoved;
			base.Room.tr_player_reservation_removed += this.OnReservationRemoved;
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_start_failed += this.OnTrSessionStartFailed;
			extension.tr_session_ended += this.OnSessionEnded;
			base.Room.tr_player_joined_session += this.OnTrPlayerJoinedSession;
		}

		// Token: 0x06001A59 RID: 6745 RVA: 0x0006C664 File Offset: 0x0006AA64
		protected override void OnDisposing()
		{
			base.Room.tr_player_joined_session -= this.OnTrPlayerJoinedSession;
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_ended -= this.OnSessionEnded;
			extension.tr_session_start_failed -= this.OnTrSessionStartFailed;
			base.Room.tr_player_reservation_removed -= this.OnReservationRemoved;
			base.Room.tr_player_add_check -= this.OnTrPlayerAddCheck;
			base.Room.tr_player_added -= this.OnTrPlayerAdded;
			base.Room.tr_player_removed -= this.OnTrPlayerRemoved;
			this.StopTimer();
			base.OnDisposing();
		}

		// Token: 0x06001A5A RID: 6746 RVA: 0x0006C720 File Offset: 0x0006AB20
		private void StartTimer()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_timer == null)
				{
					this.m_timer = this.m_timerFactory.CreateTimer(new TimerCallback(this.OnKickTimer), null, this.m_config.Get().PlayersCheckTimeout);
				}
				else
				{
					this.m_timer.Change(this.m_config.Get().PlayersCheckTimeout, this.m_config.Get().PlayersCheckTimeout);
				}
			}
		}

		// Token: 0x06001A5B RID: 6747 RVA: 0x0006C7C8 File Offset: 0x0006ABC8
		private void StopTimer()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_timer != null)
				{
					this.m_timer.Dispose();
					this.m_timer = null;
				}
			}
		}

		// Token: 0x06001A5C RID: 6748 RVA: 0x0006C824 File Offset: 0x0006AC24
		private void OnTrPlayerJoinedSession(ulong profileId)
		{
			if (this.m_ratingGameBanService.IsPlayerBanned(profileId))
			{
				this.m_ratingGameBanService.UnbanRatingGameForPlayers(new ulong[]
				{
					profileId
				});
			}
		}

		// Token: 0x06001A5D RID: 6749 RVA: 0x0006C84C File Offset: 0x0006AC4C
		private void OnTrPlayerAdded(ulong profileId, GameRoomPlayerAddReason reason)
		{
			this.StartTimer();
		}

		// Token: 0x06001A5E RID: 6750 RVA: 0x0006C854 File Offset: 0x0006AC54
		private void OnTrPlayerRemoved(RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			AutoStart state = base.Room.GetState<AutoStart>(AccessMode.ReadOnly);
			if (state.IsIntermission)
			{
				this.KickAllPlayers(GameRoomPlayerRemoveReason.KickRatingGameCouldnotStart);
			}
		}

		// Token: 0x06001A5F RID: 6751 RVA: 0x0006C881 File Offset: 0x0006AC81
		private void OnReservationRemoved(ReservationRemovedReason reason)
		{
			if (reason.IsOneOf(new ReservationRemovedReason[]
			{
				ReservationRemovedReason.Expired,
				ReservationRemovedReason.CleanUp
			}))
			{
				this.StartTimer();
			}
		}

		// Token: 0x06001A60 RID: 6752 RVA: 0x0006C8A4 File Offset: 0x0006ACA4
		private void OnKickTimer(object state)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_timer == null)
				{
					return;
				}
			}
			try
			{
				base.Room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					SessionExtension extension = r.GetExtension<SessionExtension>();
					if (!extension.Started && r.PlayerCountWithReserved < r.MinReadyPlayers)
					{
						this.KickAllPlayers(GameRoomPlayerRemoveReason.KickRatingGameCouldnotStart);
					}
				});
			}
			catch (RoomClosedException p)
			{
				Log.Info<RoomClosedException>("{0}", p);
			}
			this.StopTimer();
		}

		// Token: 0x06001A61 RID: 6753 RVA: 0x0006C934 File Offset: 0x0006AD34
		private GameRoomRetCode OnTrPlayerAddCheck(RoomPlayer player)
		{
			return (!base.Room.HasReservation(player.ProfileID)) ? GameRoomRetCode.NOT_PARTICIPATE_IN_RATING_GAME : GameRoomRetCode.OK;
		}

		// Token: 0x06001A62 RID: 6754 RVA: 0x0006C954 File Offset: 0x0006AD54
		private void OnTrSessionStartFailed()
		{
			this.KickAllPlayers(GameRoomPlayerRemoveReason.KickRatingGameCouldnotStart);
		}

		// Token: 0x06001A63 RID: 6755 RVA: 0x0006C95E File Offset: 0x0006AD5E
		private void OnSessionEnded(string sessionId, bool abnormal)
		{
			this.KickAllPlayers(GameRoomPlayerRemoveReason.KickRatingSessionEnded);
			base.Room.RemoveAllReservations();
		}

		// Token: 0x06001A64 RID: 6756 RVA: 0x0006C974 File Offset: 0x0006AD74
		private void KickAllPlayers(GameRoomPlayerRemoveReason reason)
		{
			List<ulong> arr = base.Room.GetState<CoreState>(AccessMode.ReadOnly).Players.SafeSelect((KeyValuePair<ulong, RoomPlayer> p) => p.Key).ToList<ulong>();
			KickExtension kickExtension = base.Room.GetExtension<KickExtension>();
			arr.SafeForEach(delegate(ulong profileId)
			{
				kickExtension.KickPlayer(profileId, reason);
			});
		}

		// Token: 0x04000C9C RID: 3228
		private readonly IRatingGameBanService m_ratingGameBanService;

		// Token: 0x04000C9D RID: 3229
		private readonly ITimerFactory m_timerFactory;

		// Token: 0x04000C9E RID: 3230
		private readonly IConfigProvider<RatingRoomConfig> m_config;

		// Token: 0x04000C9F RID: 3231
		private readonly object m_lock = new object();

		// Token: 0x04000CA0 RID: 3232
		private ITimer m_timer;
	}
}
