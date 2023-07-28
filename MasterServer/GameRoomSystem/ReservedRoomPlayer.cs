using System;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004AB RID: 1195
	internal class ReservedRoomPlayer : RoomPlayer
	{
		// Token: 0x06001968 RID: 6504 RVA: 0x00067316 File Offset: 0x00065716
		public ReservedRoomPlayer(UserInfo.User user, IClanService clanService) : base(user, clanService)
		{
		}
	}
}
