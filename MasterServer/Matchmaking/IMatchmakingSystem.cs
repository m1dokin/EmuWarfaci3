using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.GameRoomSystem;
using MasterServer.Matchmaking.Data;

namespace MasterServer.Matchmaking
{
	// Token: 0x02000675 RID: 1653
	[Contract]
	internal interface IMatchmakingSystem
	{
		// Token: 0x14000093 RID: 147
		// (add) Token: 0x060022C0 RID: 8896
		// (remove) Token: 0x060022C1 RID: 8897
		event UnQueueEntityDeleg OnUnQueueEntity;

		// Token: 0x14000094 RID: 148
		// (add) Token: 0x060022C2 RID: 8898
		// (remove) Token: 0x060022C3 RID: 8899
		event Action<IEnumerable<MMResultEntity>> OnEntitiesFailed;

		// Token: 0x14000095 RID: 149
		// (add) Token: 0x060022C4 RID: 8900
		// (remove) Token: 0x060022C5 RID: 8901
		event Action<IEnumerable<MMResultEntity>, IGameRoom> OnEntitiesSucceded;

		// Token: 0x060022C6 RID: 8902
		MatchmakingConfig GetConfig();

		// Token: 0x060022C7 RID: 8903
		void QueueEntity(MMEntityInfo entityInfo);

		// Token: 0x060022C8 RID: 8904
		void UnQueueEntity(ulong initiatorProfileId, EUnQueueReason reason);

		// Token: 0x060022C9 RID: 8905
		void ResetMapsSettings(ulong initiatorProfileId);

		// Token: 0x060022CA RID: 8906
		void OnMatchmakingResult(MatchmakingResult result);

		// Token: 0x060022CB RID: 8907
		IDictionary<GameRoomType, MMEntityPool> GetQueue();

		// Token: 0x060022CC RID: 8908
		IEnumerable<string> GetAcceptedMissions(MMEntityInfo mmEntity);
	}
}
