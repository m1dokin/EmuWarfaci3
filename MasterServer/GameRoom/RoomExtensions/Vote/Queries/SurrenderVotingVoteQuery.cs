using System;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.RoomExtensions.Vote.Queries
{
	// Token: 0x020004E0 RID: 1248
	[QueryAttributes(TagName = "surrender_voting_vote")]
	internal class SurrenderVotingVoteQuery : VotingVoteQuery
	{
		// Token: 0x06001AE9 RID: 6889 RVA: 0x0006E619 File Offset: 0x0006CA19
		public SurrenderVotingVoteQuery(IGameRoomManager gameRoomManager) : base(gameRoomManager)
		{
		}

		// Token: 0x06001AEA RID: 6890 RVA: 0x0006E622 File Offset: 0x0006CA22
		protected override VoteExtension GetVoteExtension(IGameRoom room)
		{
			return room.GetExtension<SurrenderVoteExtension>();
		}
	}
}
