using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Common;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameRoomSystem;

namespace MasterServer.Matchmaking.Data
{
	// Token: 0x02000516 RID: 1302
	[Serializable]
	internal class MMRoomDTO
	{
		// Token: 0x06001C43 RID: 7235 RVA: 0x00071B80 File Offset: 0x0006FF80
		public MMRoomDTO(IGameRoom room)
		{
			MMRoomDTO $this = this;
			room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				$this.RoomId = room.ID;
				$this.RoomName = room.RoomName;
				$this.RoomType = room.Type;
				$this.MissionId = room.MissionKey;
				$this.MMGeneration = room.MMGeneration;
				$this.RegionId = room.RegionId;
				$this.Players = (from p in room.Players.Concat(room.ReservedPlayers)
				select new MMPlayerDTO(p.ProfileID, p.UserID, p.Nickname, p.OnlineID, p.Skill.Value, p.GroupID, ProfileProgressionInfo.ClassToClassChar[ProfileProgressionInfo.PlayerClassFromClassId(p.ClassID)])).ToList<MMPlayerDTO>();
			});
		}

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06001C44 RID: 7236 RVA: 0x00071BBF File Offset: 0x0006FFBF
		// (set) Token: 0x06001C45 RID: 7237 RVA: 0x00071BC7 File Offset: 0x0006FFC7
		public ulong RoomId { get; private set; }

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x06001C46 RID: 7238 RVA: 0x00071BD0 File Offset: 0x0006FFD0
		// (set) Token: 0x06001C47 RID: 7239 RVA: 0x00071BD8 File Offset: 0x0006FFD8
		public string RoomName { get; private set; }

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x06001C48 RID: 7240 RVA: 0x00071BE1 File Offset: 0x0006FFE1
		// (set) Token: 0x06001C49 RID: 7241 RVA: 0x00071BE9 File Offset: 0x0006FFE9
		public GameRoomType RoomType { get; private set; }

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x06001C4A RID: 7242 RVA: 0x00071BF2 File Offset: 0x0006FFF2
		// (set) Token: 0x06001C4B RID: 7243 RVA: 0x00071BFA File Offset: 0x0006FFFA
		public string MissionId { get; private set; }

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x06001C4C RID: 7244 RVA: 0x00071C03 File Offset: 0x00070003
		// (set) Token: 0x06001C4D RID: 7245 RVA: 0x00071C0B File Offset: 0x0007000B
		public ulong MMGeneration { get; private set; }

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x06001C4E RID: 7246 RVA: 0x00071C14 File Offset: 0x00070014
		// (set) Token: 0x06001C4F RID: 7247 RVA: 0x00071C1C File Offset: 0x0007001C
		public string RegionId { get; private set; }

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x06001C50 RID: 7248 RVA: 0x00071C25 File Offset: 0x00070025
		// (set) Token: 0x06001C51 RID: 7249 RVA: 0x00071C2D File Offset: 0x0007002D
		public List<MMPlayerDTO> Players { get; private set; }
	}
}
