using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x02000604 RID: 1540
	[RoomExtension]
	internal class RoomMasterExtension : RoomExtensionBase
	{
		// Token: 0x14000083 RID: 131
		// (add) Token: 0x060020E3 RID: 8419 RVA: 0x00087064 File Offset: 0x00085464
		// (remove) Token: 0x060020E4 RID: 8420 RVA: 0x0008709C File Offset: 0x0008549C
		internal event RoomMasterExtension.TrOnMasterChangedDeleg tr_master_changed;

		// Token: 0x060020E5 RID: 8421 RVA: 0x000870D2 File Offset: 0x000854D2
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			base.Room.tr_player_added += this.trOnPlayerAdded;
			base.Room.tr_player_removed += this.trOnPlayerRemoved;
		}

		// Token: 0x060020E6 RID: 8422 RVA: 0x00087109 File Offset: 0x00085509
		public override void Close()
		{
			base.Close();
			base.Room.tr_player_added -= this.trOnPlayerAdded;
			base.Room.tr_player_removed -= this.trOnPlayerRemoved;
		}

		// Token: 0x060020E7 RID: 8423 RVA: 0x00087140 File Offset: 0x00085540
		public bool IsRoomMaster(ulong profile_id)
		{
			RoomMasterState state = base.Room.GetState<RoomMasterState>(AccessMode.ReadOnly);
			return state.RoomMaster == profile_id;
		}

		// Token: 0x060020E8 RID: 8424 RVA: 0x00087164 File Offset: 0x00085564
		public void PromoteToMaster(ulong initiator_pid, ulong target_pid)
		{
			if (!this.IsRoomMaster(initiator_pid))
			{
				throw new ApplicationException("Only master can promote others");
			}
			CoreState state = base.Room.GetState<CoreState>(AccessMode.ReadOnly);
			if (!state.Players.ContainsKey(initiator_pid))
			{
				throw new ApplicationException("No such player");
			}
			this.PromoteToMaster(target_pid);
		}

		// Token: 0x060020E9 RID: 8425 RVA: 0x000871B8 File Offset: 0x000855B8
		private void PromoteToMaster(ulong target_pid)
		{
			CoreState state = base.Room.GetState<CoreState>(AccessMode.ReadOnly);
			RoomMasterState state2 = base.Room.GetState<RoomMasterState>(AccessMode.ReadWrite);
			ulong roomMaster = state2.RoomMaster;
			Log.Info<ulong, ulong, ulong>("Player {0} gives up master privileges of room {1} to player {2}", roomMaster, base.Room.ID, target_pid);
			if (!Resources.DebugGameModeSettingsEnabled && roomMaster == target_pid)
			{
				throw new ApplicationException("Self-promoting");
			}
			RoomPlayer roomPlayer;
			if (!state.Players.TryGetValue(target_pid, out roomPlayer))
			{
				throw new ApplicationException("No such player");
			}
			state2.RoomMaster = target_pid;
			roomPlayer.RoomStatus = ((roomPlayer.RoomStatus != RoomPlayer.EStatus.CantBeReady) ? RoomPlayer.EStatus.NotReady : RoomPlayer.EStatus.CantBeReady);
			RoomPlayer roomPlayer2;
			if (state.Players.TryGetValue(roomMaster, out roomPlayer2))
			{
				roomPlayer2.RoomStatus = ((roomPlayer2.RoomStatus != RoomPlayer.EStatus.CantBeReady) ? RoomPlayer.EStatus.NotReady : RoomPlayer.EStatus.CantBeReady);
			}
			if (this.tr_master_changed != null)
			{
				this.tr_master_changed(roomMaster, target_pid);
			}
			Log.Verbose(Log.Group.GameRoom, "Player {0} is now master of room {1}", new object[]
			{
				target_pid,
				base.Room.ID
			});
		}

		// Token: 0x060020EA RID: 8426 RVA: 0x000872CC File Offset: 0x000856CC
		public void ChooseNewMaster()
		{
			Log.Verbose(Log.Group.GameRoom, "Choosing new master for room {0}", new object[]
			{
				base.Room.ID
			});
			CoreState state = base.Room.GetState<CoreState>(AccessMode.ReadOnly);
			RoomMasterState state2 = base.Room.GetState<RoomMasterState>(AccessMode.ReadWrite);
			if (state.Players.Values.Count == 0)
			{
				state2.RoomMaster = 0UL;
				return;
			}
			ulong num = 0UL;
			ulong roomMaster = state2.RoomMaster;
			ulong num2 = roomMaster;
			foreach (RoomPlayer roomPlayer in state.Players.Values)
			{
				if (roomPlayer.Experience + 1UL > num && !roomPlayer.WasMaster && roomPlayer.ProfileID != roomMaster)
				{
					num = roomPlayer.Experience + 1UL;
					num2 = roomPlayer.ProfileID;
				}
			}
			if (num2 != roomMaster)
			{
				this.PromoteToMaster(num2);
			}
			else
			{
				int num3 = 0;
				foreach (RoomPlayer roomPlayer2 in state.Players.Values)
				{
					roomPlayer2.WasMaster = false;
					if (roomPlayer2.ProfileID != roomMaster)
					{
						num3++;
					}
				}
				if (num3 != 0)
				{
					this.ChooseNewMaster();
				}
			}
		}

		// Token: 0x060020EB RID: 8427 RVA: 0x0008745C File Offset: 0x0008585C
		public override XmlElement SerializeStateChanges(RoomUpdate.Context ctx)
		{
			RoomMasterState state = (RoomMasterState)ctx.new_state;
			XmlElement xmlElement = ctx.factory.CreateElement("room_master");
			RoomMasterExtension.SerializeRoomMaster(xmlElement, state);
			return xmlElement;
		}

		// Token: 0x060020EC RID: 8428 RVA: 0x0008748E File Offset: 0x0008588E
		public static void SerializeRoomMaster(XmlElement elem, RoomMasterState state)
		{
			elem.SetAttribute("master", state.RoomMaster.ToString());
		}

		// Token: 0x060020ED RID: 8429 RVA: 0x000874AC File Offset: 0x000858AC
		public void trOnPlayerAdded(ulong profileId, GameRoomPlayerAddReason reason)
		{
			CoreState state = base.Room.GetState<CoreState>(AccessMode.ReadOnly);
			if (state.Players.Values.Count == 1)
			{
				this.PromoteToMaster(profileId);
			}
		}

		// Token: 0x060020EE RID: 8430 RVA: 0x000874E3 File Offset: 0x000858E3
		public void trOnPlayerRemoved(RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			if (this.IsRoomMaster(player.ProfileID))
			{
				this.ChooseNewMaster();
			}
		}

		// Token: 0x02000605 RID: 1541
		// (Invoke) Token: 0x060020F0 RID: 8432
		internal delegate void TrOnMasterChangedDeleg(ulong old_master, ulong new_master);
	}
}
