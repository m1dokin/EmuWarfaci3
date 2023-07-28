using System;
using System.Collections.Generic;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoom.RoomExtensions.Vote
{
	// Token: 0x020004E2 RID: 1250
	[RoomExtension]
	internal class SurrenderVoteExtension : VoteExtension
	{
		// Token: 0x06001AEE RID: 6894 RVA: 0x0006E62A File Offset: 0x0006CA2A
		public SurrenderVoteExtension(IQueryManager queryManager, ILogService logService, IVoteConfigContainer voteConfigContainer) : base(queryManager, logService, voteConfigContainer, VoteType.SurrenderVote)
		{
		}

		// Token: 0x06001AEF RID: 6895 RVA: 0x0006E636 File Offset: 0x0006CA36
		protected override VoteError CheckVote(RoomPlayer initiator, RoomPlayer target, IEnumerable<RoomPlayer> voters)
		{
			return VoteError.None;
		}

		// Token: 0x06001AF0 RID: 6896 RVA: 0x0006E63C File Offset: 0x0006CA3C
		protected override void OnVoteStarted(GameVote vote)
		{
			string nickname = vote.Initiator.Nickname;
			Log.Verbose(Log.Group.GameRoom, "Room {0} player {1} has started surrender vote", new object[]
			{
				base.Room.ID,
				nickname
			});
			base.NotifyPlayers(vote, "on_surrender_voting_started", new object[]
			{
				nickname,
				vote.YesVotesRequired,
				vote.NoVotesRequired
			});
		}

		// Token: 0x06001AF1 RID: 6897 RVA: 0x0006E6B0 File Offset: 0x0006CAB0
		protected override void OnVoteSuccess(GameVote vote)
		{
			string serverOnlineId = string.Empty;
			base.Room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				ServerExtension extension = this.Room.GetExtension<ServerExtension>();
				serverOnlineId = extension.ServerOnlineID;
			});
			this.m_queryManager.Request("on_surrender_requested", serverOnlineId, new object[]
			{
				vote.InitiatorTeamId
			});
		}

		// Token: 0x06001AF2 RID: 6898 RVA: 0x0006E718 File Offset: 0x0006CB18
		protected override void OnLogVoteResults(GameVote vote, VoteResult result, int yesVotes, int noVotes)
		{
			string sessionId = string.Empty;
			base.Room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				sessionId = r.SessionID;
			});
			this.m_logService.Event.SurrenderVotingResult(sessionId, (int)result, yesVotes, noVotes);
		}
	}
}
