using System;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.RoomExtensions.Vote.Queries
{
	// Token: 0x020004DE RID: 1246
	[QueryAttributes(TagName = "surrender_voting_start")]
	internal class StartSurrenderVoteQuery : StartVoteQuery<SurrenderVoteExtension>
	{
		// Token: 0x06001AE6 RID: 6886 RVA: 0x0006E60F File Offset: 0x0006CA0F
		public StartSurrenderVoteQuery(IGameRoomManager gameRoomManager) : base(gameRoomManager, VoteType.SurrenderVote)
		{
		}
	}
}
