using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x02000602 RID: 1538
	[RoomExtension]
	internal class ReservationsExtension : RoomExtensionBase
	{
		// Token: 0x060020D6 RID: 8406 RVA: 0x00086DC0 File Offset: 0x000851C0
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			ConfigSection section = Resources.ModuleSettings.GetSection("GameRoom");
			int cacheTimeoutSec = int.Parse(section.Get("ReservationTimeoutSec"));
			section.OnConfigChanged += this.OnConfigChanged;
			this.m_reservations = new CacheDictionary<ulong, object>(cacheTimeoutSec, CacheDictionaryMode.Expiration);
			this.m_reservations.ItemExpired += this.ReservationsOnItemExpired;
			room.tr_player_add_check += this.OnTrPlayerAddCheck;
		}

		// Token: 0x060020D7 RID: 8407 RVA: 0x00086E40 File Offset: 0x00085240
		protected override void OnDisposing()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("GameRoom");
			section.OnConfigChanged -= this.OnConfigChanged;
			this.m_reservations.Dispose();
			base.Room.tr_player_add_check -= this.OnTrPlayerAddCheck;
			base.OnDisposing();
		}

		// Token: 0x060020D8 RID: 8408 RVA: 0x00086E98 File Offset: 0x00085298
		public bool TryAddReservations(IEnumerable<ulong> profileIds)
		{
			if (base.Room.PlayerCountWithReserved + profileIds.Count<ulong>() > base.Room.MaxPlayers)
			{
				return false;
			}
			if (profileIds.All((ulong profileId) => this.m_reservations.Add(profileId, null)))
			{
				return true;
			}
			this.RemoveReservations(profileIds);
			return false;
		}

		// Token: 0x060020D9 RID: 8409 RVA: 0x00086EEA File Offset: 0x000852EA
		public bool TryAddReservation(ulong profileId)
		{
			return base.Room.PlayerCountWithReserved < base.Room.MaxPlayers && this.m_reservations.Add(profileId, null);
		}

		// Token: 0x060020DA RID: 8410 RVA: 0x00086F16 File Offset: 0x00085316
		public void RemoveReservation(ulong profileId)
		{
			this.m_reservations.Remove(profileId);
		}

		// Token: 0x060020DB RID: 8411 RVA: 0x00086F28 File Offset: 0x00085328
		public void RemoveReservations(IEnumerable<ulong> profileIds)
		{
			foreach (ulong profileId in profileIds)
			{
				this.RemoveReservation(profileId);
			}
		}

		// Token: 0x060020DC RID: 8412 RVA: 0x00086F7C File Offset: 0x0008537C
		public bool HasReservationFor(ulong profileId)
		{
			return this.m_reservations.ContainsKey(profileId);
		}

		// Token: 0x060020DD RID: 8413 RVA: 0x00086F8C File Offset: 0x0008538C
		private void ReservationsOnItemExpired(ulong key, object data)
		{
			base.Room.transaction(AccessMode.ReadWrite, delegate(IGameRoom room)
			{
				room.RemoveReservation(key, ReservationRemovedReason.Expired);
			});
		}

		// Token: 0x060020DE RID: 8414 RVA: 0x00086FC0 File Offset: 0x000853C0
		private void OnConfigChanged(ConfigEventArgs args)
		{
			if (string.Equals(args.Name, "ReservationTimeoutSec", StringComparison.CurrentCultureIgnoreCase))
			{
				int iValue = args.iValue;
				this.m_reservations.ChangeTimeout(iValue);
			}
		}

		// Token: 0x060020DF RID: 8415 RVA: 0x00086FF6 File Offset: 0x000853F6
		private GameRoomRetCode OnTrPlayerAddCheck(RoomPlayer user)
		{
			return (!this.HasReservationFor(user.ProfileID) && base.Room.PlayerCountWithReserved >= base.Room.MaxPlayers) ? GameRoomRetCode.FULL : GameRoomRetCode.OK;
		}

		// Token: 0x04001010 RID: 4112
		private CacheDictionary<ulong, object> m_reservations;
	}
}
