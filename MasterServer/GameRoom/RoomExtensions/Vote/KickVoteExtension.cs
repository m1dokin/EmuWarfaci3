using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.PunishmentSystem;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.RoomExtensions.Vote
{
	// Token: 0x020004D7 RID: 1239
	[RoomExtension]
	internal class KickVoteExtension : VoteExtension
	{
		// Token: 0x06001AD6 RID: 6870 RVA: 0x0006E0DA File Offset: 0x0006C4DA
		public KickVoteExtension(IQueryManager queryManager, ILogService logService, IPunishmentService punishmentService, IVoteConfigContainer voteConfigContainer) : base(queryManager, logService, voteConfigContainer, VoteType.KickVote)
		{
			this.m_punishmentService = punishmentService;
		}

		// Token: 0x06001AD7 RID: 6871 RVA: 0x0006E0EE File Offset: 0x0006C4EE
		protected override VoteError CheckVote(RoomPlayer initiator, RoomPlayer target, IEnumerable<RoomPlayer> voters)
		{
			if (target == null)
			{
				return VoteError.InvalidTarget;
			}
			if (initiator.Equals(target))
			{
				return VoteError.InvalidTarget;
			}
			if (voters.Count<RoomPlayer>() <= 1)
			{
				return VoteError.NotEnoughPlayers;
			}
			if (!this.m_punishmentService.CanKick(target.ProfileID))
			{
				return VoteError.InvalidTarget;
			}
			return VoteError.None;
		}

		// Token: 0x06001AD8 RID: 6872 RVA: 0x0006E130 File Offset: 0x0006C530
		protected override void OnVoteStarted(GameVote vote)
		{
			string nickname = vote.Initiator.Nickname;
			string nickname2 = vote.Target.Nickname;
			Log.Verbose(Log.Group.GameRoom, "Room {0} player {1} has started kick vote against {2}", new object[]
			{
				base.Room.ID,
				nickname,
				nickname2
			});
			base.NotifyPlayers(vote, "on_kick_voting_started", new object[]
			{
				nickname,
				nickname2,
				vote.YesVotesRequired,
				vote.NoVotesRequired
			});
		}

		// Token: 0x06001AD9 RID: 6873 RVA: 0x0006E1B7 File Offset: 0x0006C5B7
		protected override void OnVoteSuccess(GameVote vote)
		{
			this.m_punishmentService.KickPlayerLocal(vote.Target.ProfileID, GameRoomPlayerRemoveReason.KickVote);
		}

		// Token: 0x06001ADA RID: 6874 RVA: 0x0006E1D4 File Offset: 0x0006C5D4
		protected override void OnLogVoteResults(GameVote vote, VoteResult result, int yesVotes, int noVotes)
		{
			string sessionId = string.Empty;
			base.Room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				sessionId = r.SessionID;
			});
			this.m_logService.Event.KickVotingResult(sessionId, vote.Initiator.ProfileID, vote.Target.ProfileID, (int)result, yesVotes, noVotes);
		}

		// Token: 0x04000CD5 RID: 3285
		private readonly IPunishmentService m_punishmentService;
	}
}
