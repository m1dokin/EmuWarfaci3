using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using MasterServer.Common;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.RoomExtensions
{
	// Token: 0x020004C4 RID: 1220
	[RoomExtension]
	internal class RegionAutostartExtension : RoomExtensionBase
	{
		// Token: 0x06001A6C RID: 6764 RVA: 0x0006CB48 File Offset: 0x0006AF48
		public static void SerializeTo(XmlElement element, RegionState state)
		{
			element.SetAttribute("region_id", state.RegionId);
		}

		// Token: 0x06001A6D RID: 6765 RVA: 0x0006CB5B File Offset: 0x0006AF5B
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			base.Room.tr_player_added += this.TrOnPlayerAdded;
			base.Room.tr_player_removed += this.TrOnPlayerRemoved;
		}

		// Token: 0x06001A6E RID: 6766 RVA: 0x0006CB94 File Offset: 0x0006AF94
		public override XmlElement SerializeStateChanges(RoomUpdate.Context ctx)
		{
			RegionState state = (RegionState)ctx.new_state;
			XmlElement xmlElement = ctx.factory.CreateElement("regions");
			RegionAutostartExtension.SerializeTo(xmlElement, state);
			return xmlElement;
		}

		// Token: 0x06001A6F RID: 6767 RVA: 0x0006CBC6 File Offset: 0x0006AFC6
		protected override void OnDisposing()
		{
			base.OnDisposing();
			base.Room.tr_player_added -= this.TrOnPlayerAdded;
			base.Room.tr_player_removed -= this.TrOnPlayerRemoved;
		}

		// Token: 0x06001A70 RID: 6768 RVA: 0x0006CBFC File Offset: 0x0006AFFC
		private void TrOnPlayerAdded(ulong profileId, GameRoomPlayerAddReason reason)
		{
			this.UpdateRegionIdInAutostartRoom();
		}

		// Token: 0x06001A71 RID: 6769 RVA: 0x0006CC04 File Offset: 0x0006B004
		private void TrOnPlayerRemoved(RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			this.UpdateRegionIdInAutostartRoom();
		}

		// Token: 0x06001A72 RID: 6770 RVA: 0x0006CC0C File Offset: 0x0006B00C
		private void UpdateRegionIdInAutostartRoom()
		{
			IEnumerable<IGrouping<string, RoomPlayer>> source = from player in base.Room.Players
			group player by player.RegionId;
			if (RegionAutostartExtension.<>f__mg$cache0 == null)
			{
				RegionAutostartExtension.<>f__mg$cache0 = new Func<IGrouping<string, RoomPlayer>, int>(Enumerable.Count<RoomPlayer>);
			}
			IEnumerable<string> source2 = from @group in source.OrderByDescending(RegionAutostartExtension.<>f__mg$cache0)
			select @group.Key;
			base.Room.GetState<RegionState>(AccessMode.ReadWrite).RegionId = (source2.FirstOrDefault<string>() ?? "global");
		}

		// Token: 0x04000CA3 RID: 3235
		[CompilerGenerated]
		private static Func<IGrouping<string, RoomPlayer>, int> <>f__mg$cache0;
	}
}
