using System;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005AC RID: 1452
	// (Invoke) Token: 0x06001F22 RID: 7970
	internal delegate void OnProfileRankChangedDeleg(SProfileInfo profile, SRankInfo newRank, SRankInfo oldRank, ILogGroup logGroup);
}
