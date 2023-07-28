using System;
using HK2Net;

namespace MasterServer.GameLogic.InGameEventSystem
{
	// Token: 0x0200030C RID: 780
	[Contract]
	public interface IInGameEventsService
	{
		// Token: 0x14000035 RID: 53
		// (add) Token: 0x060011F6 RID: 4598
		// (remove) Token: 0x060011F7 RID: 4599
		event OnInGameRewardDelegate OnInGameReward;

		// Token: 0x060011F8 RID: 4600
		void FireInGameEvent(string fromJid, string eventName, string sessionId, ulong profileId, InGameEventData data);
	}
}
