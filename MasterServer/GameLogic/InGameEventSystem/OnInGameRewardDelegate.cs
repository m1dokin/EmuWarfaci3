using System;
using MasterServer.DAL;

namespace MasterServer.GameLogic.InGameEventSystem
{
	// Token: 0x0200030A RID: 778
	// (Invoke) Token: 0x060011F2 RID: 4594
	public delegate void OnInGameRewardDelegate(string sessionId, string missionType, SProfileInfo profile, string rewardSet);
}
