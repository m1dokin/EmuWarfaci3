using System;
using MasterServer.Core;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.SkillSystem
{
	// Token: 0x0200043F RID: 1087
	internal static class SkillTypeHelper
	{
		// Token: 0x0600173E RID: 5950 RVA: 0x00060C46 File Offset: 0x0005F046
		public static SkillType GetSkillTypeByChannelType(Resources.ChannelType type)
		{
			switch (type)
			{
			case Resources.ChannelType.PVE:
				return SkillType.Pve;
			case Resources.ChannelType.PVP_Newbie:
			case Resources.ChannelType.PVP_Skilled:
			case Resources.ChannelType.PVP_Pro:
				return SkillType.Pvp;
			default:
				return SkillType.None;
			}
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x00060C68 File Offset: 0x0005F068
		public static SkillType GetSkillTypeByRoomType(GameRoomType roomType)
		{
			if (roomType.IsPveMode())
			{
				return SkillType.Pve;
			}
			if (roomType == GameRoomType.PvP_Rating)
			{
				return SkillType.Rating;
			}
			return SkillType.Pvp;
		}
	}
}
