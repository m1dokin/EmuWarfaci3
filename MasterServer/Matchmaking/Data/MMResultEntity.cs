using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using MasterServer.GameRoomSystem;
using Util.Common;

namespace MasterServer.Matchmaking.Data
{
	// Token: 0x020004F6 RID: 1270
	internal class MMResultEntity
	{
		// Token: 0x06001B51 RID: 6993 RVA: 0x0006F1A1 File Offset: 0x0006D5A1
		public MMResultEntity()
		{
		}

		// Token: 0x06001B52 RID: 6994 RVA: 0x0006F1B4 File Offset: 0x0006D5B4
		public MMResultEntity(MMEntityInfo mmEntityInfo)
		{
			this.EntityId = mmEntityInfo.Id;
			this.Initiator = mmEntityInfo.Initiator.Nickname;
			this.Players.AddRange(from p in mmEntityInfo.Players
			select new MMResultPlayerInfo
			{
				ProfileId = p.User.ProfileID,
				UserId = p.User.UserID,
				Nickname = p.User.Nickname,
				OnlineId = p.User.OnlineID
			});
			this.RoomType = mmEntityInfo.Settings.RoomType;
			this.StartTime = TimeUtils.LocalTimeToUTCTimestamp(mmEntityInfo.Settings.StartTimeUtc);
			this.RegionId = mmEntityInfo.Initiator.RegionId;
		}

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06001B53 RID: 6995 RVA: 0x0006F25A File Offset: 0x0006D65A
		// (set) Token: 0x06001B54 RID: 6996 RVA: 0x0006F262 File Offset: 0x0006D662
		public string EntityId { get; set; }

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06001B55 RID: 6997 RVA: 0x0006F26B File Offset: 0x0006D66B
		// (set) Token: 0x06001B56 RID: 6998 RVA: 0x0006F273 File Offset: 0x0006D673
		public string Initiator { get; set; }

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x06001B57 RID: 6999 RVA: 0x0006F27C File Offset: 0x0006D67C
		// (set) Token: 0x06001B58 RID: 7000 RVA: 0x0006F284 File Offset: 0x0006D684
		public GameRoomType RoomType { get; set; }

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06001B59 RID: 7001 RVA: 0x0006F28D File Offset: 0x0006D68D
		// (set) Token: 0x06001B5A RID: 7002 RVA: 0x0006F295 File Offset: 0x0006D695
		public ulong StartTime { get; set; }

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06001B5B RID: 7003 RVA: 0x0006F29E File Offset: 0x0006D69E
		// (set) Token: 0x06001B5C RID: 7004 RVA: 0x0006F2A6 File Offset: 0x0006D6A6
		public int ChannelSwitches { get; set; }

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06001B5D RID: 7005 RVA: 0x0006F2AF File Offset: 0x0006D6AF
		// (set) Token: 0x06001B5E RID: 7006 RVA: 0x0006F2B7 File Offset: 0x0006D6B7
		public string RegionId { get; set; }

		// Token: 0x06001B5F RID: 7007 RVA: 0x0006F2C0 File Offset: 0x0006D6C0
		public override string ToString()
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			return javaScriptSerializer.Serialize(this);
		}

		// Token: 0x04000D13 RID: 3347
		public List<MMResultPlayerInfo> Players = new List<MMResultPlayerInfo>();
	}
}
