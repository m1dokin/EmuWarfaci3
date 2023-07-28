using System;
using HK2Net;

namespace MasterServer.GameRoom.RoomExtensions.Vote
{
	// Token: 0x020004E6 RID: 1254
	[Contract]
	internal interface IVoteConfigContainer
	{
		// Token: 0x06001AFF RID: 6911
		VoteConfig GetConfig(VoteType voteType);
	}
}
