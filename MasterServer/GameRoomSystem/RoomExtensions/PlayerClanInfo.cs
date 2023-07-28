using System;
using System.Linq;
using System.Threading;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameLogic.ClanSystem;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x02000601 RID: 1537
	[RoomExtension]
	internal class PlayerClanInfo : RoomExtensionBase
	{
		// Token: 0x060020CC RID: 8396 RVA: 0x00086BA8 File Offset: 0x00084FA8
		public PlayerClanInfo(IClanService clanService)
		{
			this.m_clanService = clanService;
		}

		// Token: 0x060020CD RID: 8397 RVA: 0x00086BB7 File Offset: 0x00084FB7
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			base.Room.tr_player_added += this.trOnPlayerAdded;
		}

		// Token: 0x060020CE RID: 8398 RVA: 0x00086BD7 File Offset: 0x00084FD7
		protected override void OnDisposing()
		{
			base.Room.tr_player_added -= this.trOnPlayerAdded;
			base.OnDisposing();
		}

		// Token: 0x060020CF RID: 8399 RVA: 0x00086BF8 File Offset: 0x00084FF8
		public void UpdateClanName(ulong profileId)
		{
			RoomPlayer player = base.Room.GetPlayer(profileId);
			if (player != null)
			{
				this.UpdateClanName(player);
				base.Room.SignalPlayersChanged();
			}
		}

		// Token: 0x060020D0 RID: 8400 RVA: 0x00086C2C File Offset: 0x0008502C
		private void trOnPlayerAdded(ulong profileId, GameRoomPlayerAddReason reason)
		{
			if (reason == GameRoomPlayerAddReason.Matchmaking)
			{
				if (!this.m_resolveScheduled)
				{
					this.m_resolveScheduled = true;
					ThreadPoolProxy.QueueUserWorkItem(new WaitCallback(this.ResolveClanNames), null, true);
				}
			}
			else
			{
				RoomPlayer player = base.Room.GetPlayer(profileId);
				this.UpdateClanName(player);
				base.Room.SignalPlayersChanged();
			}
		}

		// Token: 0x060020D1 RID: 8401 RVA: 0x00086C8C File Offset: 0x0008508C
		private void ResolveClanNames(object dummy)
		{
			try
			{
				base.Room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					this.m_resolveScheduled = false;
					CoreState state = r.GetState<CoreState>(AccessMode.ReadWrite);
					foreach (RoomPlayer player in from p in state.Players.Values
					where !p.IsInClan()
					select p)
					{
						this.UpdateClanName(player);
					}
					base.Room.SignalPlayersChanged();
				});
			}
			catch (Exception)
			{
				Log.Error("Can't resolve clan names");
				throw;
			}
		}

		// Token: 0x060020D2 RID: 8402 RVA: 0x00086CD4 File Offset: 0x000850D4
		private void UpdateClanName(RoomPlayer player)
		{
			player.UpdateClanInfo(this.m_clanService);
			if (!player.IsInClan() && base.Room.IsClanWarMode())
			{
				base.Room.RemovePlayer(player.ProfileID, GameRoomPlayerRemoveReason.KickClan);
			}
		}

		// Token: 0x0400100D RID: 4109
		private bool m_resolveScheduled;

		// Token: 0x0400100E RID: 4110
		private IClanService m_clanService;
	}
}
