using System;
using HK2Net;
using MasterServer.Core.Configuration;

namespace MasterServer.GameRoom.RoomExtensions.Vote
{
	// Token: 0x020004E4 RID: 1252
	[Contract]
	internal interface IVoteConfigParser
	{
		// Token: 0x06001AFC RID: 6908
		VoteConfig Parse(ConfigSection configSection);
	}
}
