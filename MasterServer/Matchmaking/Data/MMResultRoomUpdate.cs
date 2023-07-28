using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using MasterServer.GameRoomSystem;

namespace MasterServer.Matchmaking.Data
{
	// Token: 0x020004F8 RID: 1272
	internal class MMResultRoomUpdate
	{
		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x06001B6B RID: 7019 RVA: 0x0006F393 File Offset: 0x0006D793
		// (set) Token: 0x06001B6C RID: 7020 RVA: 0x0006F39B File Offset: 0x0006D79B
		public ulong RoomId { get; set; }

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06001B6D RID: 7021 RVA: 0x0006F3A4 File Offset: 0x0006D7A4
		// (set) Token: 0x06001B6E RID: 7022 RVA: 0x0006F3AC File Offset: 0x0006D7AC
		public GameRoomType RoomType { get; set; }

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06001B6F RID: 7023 RVA: 0x0006F3B5 File Offset: 0x0006D7B5
		// (set) Token: 0x06001B70 RID: 7024 RVA: 0x0006F3BD File Offset: 0x0006D7BD
		public string RoomName { get; set; }

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06001B71 RID: 7025 RVA: 0x0006F3C6 File Offset: 0x0006D7C6
		// (set) Token: 0x06001B72 RID: 7026 RVA: 0x0006F3CE File Offset: 0x0006D7CE
		public string MissionId { get; set; }

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06001B73 RID: 7027 RVA: 0x0006F3D7 File Offset: 0x0006D7D7
		// (set) Token: 0x06001B74 RID: 7028 RVA: 0x0006F3DF File Offset: 0x0006D7DF
		public int PlayersCount { get; set; }

		// Token: 0x06001B75 RID: 7029 RVA: 0x0006F3E8 File Offset: 0x0006D7E8
		public override string ToString()
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			return javaScriptSerializer.Serialize(this);
		}

		// Token: 0x04000D1F RID: 3359
		public List<MMResultEntity> Entities = new List<MMResultEntity>();
	}
}
