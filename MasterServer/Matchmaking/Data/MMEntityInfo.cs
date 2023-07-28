using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Users;

namespace MasterServer.Matchmaking.Data
{
	// Token: 0x020004F4 RID: 1268
	internal class MMEntityInfo
	{
		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06001B3E RID: 6974 RVA: 0x0006F08F File Offset: 0x0006D48F
		// (set) Token: 0x06001B3F RID: 6975 RVA: 0x0006F097 File Offset: 0x0006D497
		public string Id { get; set; }

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x06001B40 RID: 6976 RVA: 0x0006F0A0 File Offset: 0x0006D4A0
		// (set) Token: 0x06001B41 RID: 6977 RVA: 0x0006F0A8 File Offset: 0x0006D4A8
		public UserInfo.User Initiator { get; set; }

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06001B42 RID: 6978 RVA: 0x0006F0B1 File Offset: 0x0006D4B1
		// (set) Token: 0x06001B43 RID: 6979 RVA: 0x0006F0B9 File Offset: 0x0006D4B9
		public MMSettings Settings { get; set; }

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x06001B44 RID: 6980 RVA: 0x0006F0C2 File Offset: 0x0006D4C2
		// (set) Token: 0x06001B45 RID: 6981 RVA: 0x0006F0CA File Offset: 0x0006D4CA
		public IEnumerable<MMPlayerInfo> Players { get; set; }

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x06001B46 RID: 6982 RVA: 0x0006F0D3 File Offset: 0x0006D4D3
		public MMPlayerInfo InitiatorPlayer
		{
			get
			{
				return this.Players.Single((MMPlayerInfo p) => p.User.ProfileID == this.Initiator.ProfileID);
			}
		}

		// Token: 0x06001B47 RID: 6983 RVA: 0x0006F0EC File Offset: 0x0006D4EC
		public double GetSkill()
		{
			return (from p in this.Players
			select p.Skill.Value).Max();
		}
	}
}
