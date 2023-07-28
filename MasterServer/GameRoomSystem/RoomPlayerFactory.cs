using System;
using HK2Net;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.GameLogic.SkillSystem;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004EF RID: 1263
	[Service]
	[Singleton]
	internal class RoomPlayerFactory : IRoomPlayerFactory
	{
		// Token: 0x06001B27 RID: 6951 RVA: 0x0006EE10 File Offset: 0x0006D210
		public RoomPlayerFactory(IClanService clanService, ISkillService skillService)
		{
			this.m_clanService = clanService;
			this.m_skillService = skillService;
		}

		// Token: 0x06001B28 RID: 6952 RVA: 0x0006EE28 File Offset: 0x0006D228
		public RoomPlayer GetRoomPlayer(UserInfo.User user, GameRoomType roomType)
		{
			SkillType skillTypeByRoomType = SkillTypeHelper.GetSkillTypeByRoomType(roomType);
			Skill skill = this.m_skillService.GetSkill(user.ProfileID, skillTypeByRoomType);
			return new RoomPlayer(user, this.m_clanService)
			{
				Skill = skill
			};
		}

		// Token: 0x04000CFD RID: 3325
		private readonly IClanService m_clanService;

		// Token: 0x04000CFE RID: 3326
		private readonly ISkillService m_skillService;
	}
}
