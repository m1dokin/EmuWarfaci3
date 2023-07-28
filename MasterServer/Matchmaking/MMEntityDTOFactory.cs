using System;
using HK2Net;
using MasterServer.Matchmaking.Data;

namespace MasterServer.Matchmaking
{
	// Token: 0x02000511 RID: 1297
	[Service]
	[Singleton]
	internal class MMEntityDTOFactory : IMMEntityDTOFactory
	{
		// Token: 0x06001C21 RID: 7201 RVA: 0x000718DF File Offset: 0x0006FCDF
		public MMEntityDTOFactory(IMatchmakingSystem matchmakingSystem)
		{
			this.m_matchmakingSystem = matchmakingSystem;
		}

		// Token: 0x06001C22 RID: 7202 RVA: 0x000718EE File Offset: 0x0006FCEE
		public MMEntityDTO Create(MMEntityInfo mmEntityInfo)
		{
			return new MMEntityDTO(mmEntityInfo, this.m_matchmakingSystem.GetAcceptedMissions(mmEntityInfo));
		}

		// Token: 0x04000D74 RID: 3444
		private readonly IMatchmakingSystem m_matchmakingSystem;
	}
}
