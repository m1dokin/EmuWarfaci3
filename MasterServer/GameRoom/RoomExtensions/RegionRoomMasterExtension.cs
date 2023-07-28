using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoom.RoomExtensions
{
	// Token: 0x020004C5 RID: 1221
	[RoomExtension]
	internal class RegionRoomMasterExtension : RoomExtensionBase
	{
		// Token: 0x06001A77 RID: 6775 RVA: 0x0006CCE6 File Offset: 0x0006B0E6
		public static void SerializeTo(XmlElement element, RegionState state)
		{
			element.SetAttribute("region_id", state.RegionId);
		}

		// Token: 0x06001A78 RID: 6776 RVA: 0x0006CCF9 File Offset: 0x0006B0F9
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			base.Room.GetExtension<RoomMasterExtension>().tr_master_changed += this.TrOnMasterChanged;
		}

		// Token: 0x06001A79 RID: 6777 RVA: 0x0006CD20 File Offset: 0x0006B120
		public override XmlElement SerializeStateChanges(RoomUpdate.Context ctx)
		{
			RegionState state = (RegionState)ctx.new_state;
			XmlElement xmlElement = ctx.factory.CreateElement("regions");
			RegionRoomMasterExtension.SerializeTo(xmlElement, state);
			return xmlElement;
		}

		// Token: 0x06001A7A RID: 6778 RVA: 0x0006CD52 File Offset: 0x0006B152
		protected override void OnDisposing()
		{
			base.OnDisposing();
			base.Room.GetExtension<RoomMasterExtension>().tr_master_changed -= this.TrOnMasterChanged;
		}

		// Token: 0x06001A7B RID: 6779 RVA: 0x0006CD78 File Offset: 0x0006B178
		private void TrOnMasterChanged(ulong oldMaster, ulong newMaster)
		{
			RegionState state = base.Room.GetState<RegionState>(AccessMode.ReadWrite);
			RoomPlayer player = base.Room.GetPlayer(newMaster);
			state.RegionId = player.RegionId;
		}
	}
}
