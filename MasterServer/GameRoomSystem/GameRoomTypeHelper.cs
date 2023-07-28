using System;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005E9 RID: 1513
	internal static class GameRoomTypeHelper
	{
		// Token: 0x0600200B RID: 8203 RVA: 0x000821B1 File Offset: 0x000805B1
		public static bool IsPveMode(this GameRoomType mode)
		{
			return (mode & GameRoomType.PvE) != (GameRoomType)0;
		}

		// Token: 0x0600200C RID: 8204 RVA: 0x000821BD File Offset: 0x000805BD
		public static bool IsPvpMode(this GameRoomType mode)
		{
			return (mode & GameRoomType.PvP) != (GameRoomType)0;
		}

		// Token: 0x0600200D RID: 8205 RVA: 0x000821C9 File Offset: 0x000805C9
		public static bool IsClanWarMode(this GameRoomType mode)
		{
			return (mode & GameRoomType.PvP_ClanWar) != (GameRoomType)0;
		}

		// Token: 0x0600200E RID: 8206 RVA: 0x000821D4 File Offset: 0x000805D4
		public static bool IsAutoStartMode(this GameRoomType mode)
		{
			return ((mode & GameRoomType.PvP_AutoStart) | (mode & GameRoomType.PvE_AutoStart) | (mode & GameRoomType.PvP_Rating)) != (GameRoomType)0;
		}

		// Token: 0x0600200F RID: 8207 RVA: 0x000821E9 File Offset: 0x000805E9
		public static bool IsPublicPvPMode(this GameRoomType mode)
		{
			return (mode & GameRoomType.PvP_Public) != (GameRoomType)0;
		}

		// Token: 0x06002010 RID: 8208 RVA: 0x000821F4 File Offset: 0x000805F4
		public static bool IsPveAutoStartMode(this GameRoomType mode)
		{
			return mode.IsPveMode() & mode.IsAutoStartMode();
		}

		// Token: 0x06002011 RID: 8209 RVA: 0x00082203 File Offset: 0x00080603
		public static bool IsPvpAutoStartMode(this GameRoomType mode)
		{
			return mode.IsPvpMode() & mode.IsAutoStartMode();
		}

		// Token: 0x06002012 RID: 8210 RVA: 0x00082212 File Offset: 0x00080612
		public static bool IsPvpRatingMode(this GameRoomType mode)
		{
			return (mode & GameRoomType.PvP_Rating) != (GameRoomType)0;
		}
	}
}
