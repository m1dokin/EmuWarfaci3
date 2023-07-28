using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004A2 RID: 1186
	internal class GroupInfo
	{
		// Token: 0x06001940 RID: 6464 RVA: 0x00066DD4 File Offset: 0x000651D4
		public GroupInfo(RoomPlayer roomPlayer)
		{
			this.Players = new List<RoomPlayer>
			{
				roomPlayer
			};
		}

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06001941 RID: 6465 RVA: 0x00066DFB File Offset: 0x000651FB
		public int Count
		{
			get
			{
				return this.Players.Count;
			}
		}

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06001942 RID: 6466 RVA: 0x00066E08 File Offset: 0x00065208
		public double Skill
		{
			get
			{
				return this.Players.Max((RoomPlayer x) => x.Skill.Value) * (double)this.Players.Count;
			}
		}

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06001943 RID: 6467 RVA: 0x00066E3F File Offset: 0x0006523F
		public bool HasTeam
		{
			get
			{
				return this.Players.Any((RoomPlayer x) => x.TeamID != 0);
			}
		}

		// Token: 0x06001944 RID: 6468 RVA: 0x00066E69 File Offset: 0x00065269
		public void Add(RoomPlayer roomPlayer)
		{
			this.Players.Add(roomPlayer);
		}

		// Token: 0x04000C10 RID: 3088
		public readonly List<RoomPlayer> Players;
	}
}
