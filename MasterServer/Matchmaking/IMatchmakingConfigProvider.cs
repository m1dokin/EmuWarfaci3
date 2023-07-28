using System;
using HK2Net;
using MasterServer.Matchmaking.Data;

namespace MasterServer.Matchmaking
{
	// Token: 0x02000504 RID: 1284
	[Contract]
	internal interface IMatchmakingConfigProvider
	{
		// Token: 0x1400006B RID: 107
		// (add) Token: 0x06001BBF RID: 7103
		// (remove) Token: 0x06001BC0 RID: 7104
		event Action<MatchmakingConfig> OnConfigChanged;

		// Token: 0x06001BC1 RID: 7105
		MatchmakingConfig Get();
	}
}
