using System;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.RoomExtensions.Vote.Queries
{
	// Token: 0x020004D8 RID: 1240
	[QueryAttributes(TagName = "kick_voting_vote")]
	internal class KickVotingVoteQuery : VotingVoteQuery
	{
		// Token: 0x06001ADB RID: 6875 RVA: 0x0006E348 File Offset: 0x0006C748
		public KickVotingVoteQuery(IGameRoomManager gameRoomManager) : base(gameRoomManager)
		{
		}

		// Token: 0x06001ADC RID: 6876 RVA: 0x0006E351 File Offset: 0x0006C751
		protected override VoteExtension GetVoteExtension(IGameRoom room)
		{
			return room.GetExtension<KickVoteExtension>();
		}
	}
}
