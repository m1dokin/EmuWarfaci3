using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Common;
using MasterServer.GameLogic.MissionAccessLimitation;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020004AE RID: 1198
	[RoomExtension]
	internal class AutoStartKickExtension : RoomExtensionBase
	{
		// Token: 0x0600196C RID: 6508 RVA: 0x000673B4 File Offset: 0x000657B4
		public AutoStartKickExtension(IMissionAccessLimitationService limitationService)
		{
			this.m_limitationService = limitationService;
		}

		// Token: 0x0600196D RID: 6509 RVA: 0x000673C3 File Offset: 0x000657C3
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			base.Room.GetExtension<MissionExtension>().TrSetMissionInfoEnded += this.OnSetMissionEnded;
		}

		// Token: 0x0600196E RID: 6510 RVA: 0x000673E8 File Offset: 0x000657E8
		public override void Close()
		{
			base.Room.GetExtension<MissionExtension>().TrSetMissionInfoEnded -= this.OnSetMissionEnded;
			base.Close();
		}

		// Token: 0x0600196F RID: 6511 RVA: 0x0006740C File Offset: 0x0006580C
		private void OnSetMissionEnded(MissionContext _)
		{
			if (!base.Room.IsPveAutoStartMode())
			{
				return;
			}
			Dictionary<ulong, RoomPlayer> players = base.Room.GetState<CoreState>(AccessMode.ReadWrite).Players;
			Dictionary<ulong, GameRoomPlayerRemoveReason> profilesToKick = new Dictionary<ulong, GameRoomPlayerRemoveReason>();
			foreach (RoomPlayer roomPlayer in players.Values)
			{
				bool flag = roomPlayer.ProfileProgression.IsMissionTypeUnlocked(base.Room.MissionType.Name);
				bool flag2 = roomPlayer.CanJoinMission(this.m_limitationService, base.Room.MissionType.Name);
				if (!flag || !flag2)
				{
					profilesToKick.Add(roomPlayer.ProfileID, flag ? GameRoomPlayerRemoveReason.KickItemNotAvalaible : GameRoomPlayerRemoveReason.KickRankRestricted);
				}
			}
			if (profilesToKick.Keys.Any<ulong>())
			{
				base.Room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					KickExtension k = r.GetExtension<KickExtension>();
					profilesToKick.SafeForEach(delegate(KeyValuePair<ulong, GameRoomPlayerRemoveReason> kv)
					{
						k.KickPlayer(kv.Key, kv.Value);
					});
				});
				base.Room.SignalPlayersChanged();
			}
		}

		// Token: 0x04000C2C RID: 3116
		private readonly IMissionAccessLimitationService m_limitationService;
	}
}
