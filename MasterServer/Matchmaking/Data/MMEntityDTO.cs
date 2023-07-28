using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameRoomSystem;
using Util.Common;

namespace MasterServer.Matchmaking.Data
{
	// Token: 0x0200050F RID: 1295
	[Serializable]
	internal class MMEntityDTO
	{
		// Token: 0x06001C0E RID: 7182 RVA: 0x00071754 File Offset: 0x0006FB54
		public MMEntityDTO(MMEntityInfo mmEntityInfo, IEnumerable<string> missionIds)
		{
			this.EntityId = mmEntityInfo.Id;
			this.RoomType = mmEntityInfo.Settings.RoomType;
			this.RoomName = mmEntityInfo.Settings.RoomName;
			this.Initiator = mmEntityInfo.Initiator.Nickname;
			this.Players = (from p in mmEntityInfo.Players
			select new MMPlayerDTO(p.User.ProfileID, p.User.UserID, p.User.Nickname, p.User.OnlineID, p.Skill.Value, this.EntityId, ProfileProgressionInfo.ClassToClassChar[p.PlayerCurrentClass])).ToList<MMPlayerDTO>();
			this.StartTime = TimeUtils.LocalTimeToUTCTimestamp(mmEntityInfo.Settings.StartTimeUtc);
			this.MissionIds = missionIds;
			this.RegionId = mmEntityInfo.Initiator.RegionId;
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06001C0F RID: 7183 RVA: 0x000717F6 File Offset: 0x0006FBF6
		// (set) Token: 0x06001C10 RID: 7184 RVA: 0x000717FE File Offset: 0x0006FBFE
		public string EntityId { get; private set; }

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06001C11 RID: 7185 RVA: 0x00071807 File Offset: 0x0006FC07
		// (set) Token: 0x06001C12 RID: 7186 RVA: 0x0007180F File Offset: 0x0006FC0F
		public GameRoomType RoomType { get; private set; }

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x06001C13 RID: 7187 RVA: 0x00071818 File Offset: 0x0006FC18
		// (set) Token: 0x06001C14 RID: 7188 RVA: 0x00071820 File Offset: 0x0006FC20
		public string RoomName { get; private set; }

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x06001C15 RID: 7189 RVA: 0x00071829 File Offset: 0x0006FC29
		// (set) Token: 0x06001C16 RID: 7190 RVA: 0x00071831 File Offset: 0x0006FC31
		public string Initiator { get; private set; }

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x06001C17 RID: 7191 RVA: 0x0007183A File Offset: 0x0006FC3A
		// (set) Token: 0x06001C18 RID: 7192 RVA: 0x00071842 File Offset: 0x0006FC42
		public List<MMPlayerDTO> Players { get; private set; }

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x06001C19 RID: 7193 RVA: 0x0007184B File Offset: 0x0006FC4B
		// (set) Token: 0x06001C1A RID: 7194 RVA: 0x00071853 File Offset: 0x0006FC53
		public IEnumerable<string> MissionIds { get; private set; }

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x06001C1B RID: 7195 RVA: 0x0007185C File Offset: 0x0006FC5C
		// (set) Token: 0x06001C1C RID: 7196 RVA: 0x00071864 File Offset: 0x0006FC64
		public ulong StartTime { get; private set; }

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x06001C1D RID: 7197 RVA: 0x0007186D File Offset: 0x0006FC6D
		// (set) Token: 0x06001C1E RID: 7198 RVA: 0x00071875 File Offset: 0x0006FC75
		public string RegionId { get; private set; }
	}
}
