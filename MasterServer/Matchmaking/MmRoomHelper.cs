using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.CryOnlineNET;
using MasterServer.Matchmaking.Data;

namespace MasterServer.Matchmaking
{
	// Token: 0x02000672 RID: 1650
	internal static class MmRoomHelper
	{
		// Token: 0x060022B2 RID: 8882 RVA: 0x00090E54 File Offset: 0x0008F254
		public static void SendMatchmakingSuccessQuery(IEnumerable<MMResultEntity> mmEntities)
		{
			foreach (MMResultEntity mmEntity in mmEntities)
			{
				MmRoomHelper.SendMatchmakingSuccessQuery(mmEntity);
			}
		}

		// Token: 0x060022B3 RID: 8883 RVA: 0x00090EA8 File Offset: 0x0008F2A8
		public static void SendMatchmakingFailedQuery(IEnumerable<MMResultEntity> mmEntities)
		{
			foreach (MMResultEntity mmEntity in mmEntities)
			{
				MmRoomHelper.SendMatchmakingFailedQuery(mmEntity);
			}
		}

		// Token: 0x060022B4 RID: 8884 RVA: 0x00090EFC File Offset: 0x0008F2FC
		public static void SendMatchmakingStartedQuery(MMEntityInfo mmEntity)
		{
			if (mmEntity.Players.Any<MMPlayerInfo>())
			{
				QueryManager.BroadcastRequestSt("gameroom_quickplay_started", (from p in mmEntity.Players
				select p.User.OnlineID).ToList<string>(), new object[]
				{
					mmEntity
				});
			}
		}

		// Token: 0x060022B5 RID: 8885 RVA: 0x00090F5C File Offset: 0x0008F35C
		public static void SendMatchmakingCanceledQuery(MMEntityInfo mmEntity)
		{
			if (mmEntity.Players.Any<MMPlayerInfo>())
			{
				QueryManager.BroadcastRequestSt("gameroom_quickplay_canceled", (from p in mmEntity.Players
				select p.User.OnlineID).ToList<string>(), new object[]
				{
					mmEntity
				});
			}
		}

		// Token: 0x060022B6 RID: 8886 RVA: 0x00090FBC File Offset: 0x0008F3BC
		private static void SendMatchmakingFailedQuery(MMResultEntity mmEntity)
		{
			if (mmEntity.Players.Any<MMResultPlayerInfo>())
			{
				QueryManager.BroadcastRequestSt("gameroom_quickplay_failed", (from p in mmEntity.Players
				select p.OnlineId).ToList<string>(), new object[]
				{
					mmEntity
				});
			}
		}

		// Token: 0x060022B7 RID: 8887 RVA: 0x0009101C File Offset: 0x0008F41C
		private static void SendMatchmakingSuccessQuery(MMResultEntity mmEntity)
		{
			if (mmEntity.Players.Any<MMResultPlayerInfo>())
			{
				QueryManager.BroadcastRequestSt("gameroom_quickplay_succeeded", (from p in mmEntity.Players
				select p.OnlineId).ToList<string>(), new object[]
				{
					mmEntity
				});
			}
		}
	}
}
