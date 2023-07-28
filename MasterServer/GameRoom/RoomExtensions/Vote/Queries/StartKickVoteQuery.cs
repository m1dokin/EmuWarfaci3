using System;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.RoomExtensions.Vote.Queries
{
	// Token: 0x020004DD RID: 1245
	[QueryAttributes(TagName = "kick_voting_start")]
	internal class StartKickVoteQuery : StartVoteQuery<KickVoteExtension>
	{
		// Token: 0x06001AE5 RID: 6885 RVA: 0x0006E605 File Offset: 0x0006CA05
		public StartKickVoteQuery(IGameRoomManager gameRoomManager) : base(gameRoomManager, VoteType.KickVote)
		{
		}
	}
}
