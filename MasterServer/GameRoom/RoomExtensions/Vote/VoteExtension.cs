using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

namespace MasterServer.GameRoom.RoomExtensions.Vote
{
	// Token: 0x020004EB RID: 1259
	internal abstract class VoteExtension : RoomExtensionBase
	{
		// Token: 0x06001B10 RID: 6928 RVA: 0x0006DC53 File Offset: 0x0006C053
		protected VoteExtension(IQueryManager queryManager, ILogService logService, IVoteConfigContainer voteConfigContainer, VoteType voteType)
		{
			this.m_queryManager = queryManager;
			this.m_logService = logService;
			this.m_voteConfigContainer = voteConfigContainer;
			this.m_voteType = voteType;
		}

		// Token: 0x06001B11 RID: 6929 RVA: 0x0006DC78 File Offset: 0x0006C078
		public override void Close()
		{
			VoteState state = base.Room.GetState<VoteState>(AccessMode.ReadWrite);
			state.Dispose();
		}

		// Token: 0x06001B12 RID: 6930 RVA: 0x0006DC98 File Offset: 0x0006C098
		public GameVote GetVote(int teamId)
		{
			VoteState state = base.Room.GetState<VoteState>(AccessMode.ReadOnly);
			GameVote result;
			state.VotesInProgress.TryGetValue(teamId, out result);
			return result;
		}

		// Token: 0x06001B13 RID: 6931 RVA: 0x0006DCC4 File Offset: 0x0006C0C4
		public VoteError StartVote(RoomPlayer initiator, RoomPlayer target, IEnumerable<ulong> votersList, int initiatorTeamId)
		{
			if (initiator == null)
			{
				return VoteError.InvalidInitiator;
			}
			if (!base.Room.Players.Contains(initiator) || !UserStatuses.IsInGame(initiator.UserStatus))
			{
				return VoteError.InvalidInitiator;
			}
			VoteConfig config = this.m_voteConfigContainer.GetConfig(this.m_voteType);
			if (!this.CheckCooldown(initiator, config.Cooldown))
			{
				return VoteError.VoteTimeout;
			}
			if (this.IsVoteInProgress(initiatorTeamId))
			{
				return VoteError.VoteInProgress;
			}
			List<RoomPlayer> list = (from p in base.Room.Players
			where votersList.Contains(p.ProfileID) && UserStatuses.IsInGame(p.UserStatus)
			select p).ToList<RoomPlayer>();
			if (target != null)
			{
				list.Remove(target);
			}
			VoteError voteError = this.CheckVote(initiator, target, list);
			if (voteError != VoteError.None)
			{
				return voteError;
			}
			GameVote vote = new GameVote(config, initiator, initiatorTeamId, target, list);
			this.AddVote(vote);
			this.OnVoteStarted(vote);
			return VoteError.None;
		}

		// Token: 0x06001B14 RID: 6932
		protected abstract VoteError CheckVote(RoomPlayer initiator, RoomPlayer target, IEnumerable<RoomPlayer> voters);

		// Token: 0x06001B15 RID: 6933
		protected abstract void OnVoteStarted(GameVote vote);

		// Token: 0x06001B16 RID: 6934
		protected abstract void OnVoteSuccess(GameVote vote);

		// Token: 0x06001B17 RID: 6935
		protected abstract void OnLogVoteResults(GameVote vote, VoteResult result, int yesVotes, int noVotes);

		// Token: 0x06001B18 RID: 6936 RVA: 0x0006DDA4 File Offset: 0x0006C1A4
		protected void NotifyPlayers(GameVote vote, string query, params object[] args)
		{
			foreach (RoomPlayer roomPlayer in vote.Voters)
			{
				this.m_queryManager.Request(query, roomPlayer.OnlineID, args);
			}
		}

		// Token: 0x06001B19 RID: 6937 RVA: 0x0006DE0C File Offset: 0x0006C20C
		private void AddVote(GameVote vote)
		{
			VoteState state = base.Room.GetState<VoteState>(AccessMode.ReadWrite);
			state.VotesInProgress[vote.InitiatorTeamId] = vote;
			vote.Updated += this.OnVoteUpdated;
			vote.Copmleted += this.OnVoteCompleted;
			state.VotesCooldowns[vote.Initiator.ProfileID] = DateTime.Now;
			vote.Vote(vote.Initiator.ProfileID, true);
		}

		// Token: 0x06001B1A RID: 6938 RVA: 0x0006DE8C File Offset: 0x0006C28C
		private bool IsVoteInProgress(int teamId)
		{
			VoteState state = base.Room.GetState<VoteState>(AccessMode.ReadOnly);
			return state.VotesInProgress.ContainsKey(teamId);
		}

		// Token: 0x06001B1B RID: 6939 RVA: 0x0006DEB4 File Offset: 0x0006C2B4
		private bool CheckCooldown(RoomPlayer initiator, TimeSpan cooldown)
		{
			VoteState state = base.Room.GetState<VoteState>(AccessMode.ReadOnly);
			DateTime d;
			return !state.VotesCooldowns.TryGetValue(initiator.ProfileID, out d) || DateTime.Now - d > cooldown;
		}

		// Token: 0x06001B1C RID: 6940 RVA: 0x0006DEFC File Offset: 0x0006C2FC
		private void OnVoteUpdated(GameVote vote, int yesVotes, int noVotes)
		{
			Log.Verbose(Log.Group.GameRoom, "Room {0} {1} voting progress: yes({2}) / no({3})", new object[]
			{
				base.Room.ID,
				this.m_voteType,
				yesVotes,
				noVotes
			});
			this.NotifyPlayers(vote, "on_voting_vote", new object[]
			{
				yesVotes,
				noVotes
			});
		}

		// Token: 0x06001B1D RID: 6941 RVA: 0x0006DF74 File Offset: 0x0006C374
		private void OnVoteCompleted(GameVote vote, VoteResult result, int yesVotes, int noVotes)
		{
			vote.Updated -= this.OnVoteUpdated;
			vote.Copmleted -= this.OnVoteCompleted;
			base.Room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				VoteState state = this.Room.GetState<VoteState>(AccessMode.ReadWrite);
				state.VotesInProgress.Remove(vote.InitiatorTeamId);
			});
			if (result == VoteResult.Success)
			{
				this.OnVoteSuccess(vote);
			}
			this.OnLogVoteResults(vote, result, yesVotes, noVotes);
			Log.Verbose(Log.Group.GameRoom, "Room {0} {1} voting finished: {3}", new object[]
			{
				base.Room.ID,
				this.m_voteType,
				result
			});
			this.NotifyPlayers(vote, "on_voting_finished", new object[]
			{
				result,
				yesVotes,
				noVotes
			});
		}

		// Token: 0x04000CF7 RID: 3319
		protected readonly IQueryManager m_queryManager;

		// Token: 0x04000CF8 RID: 3320
		protected readonly ILogService m_logService;

		// Token: 0x04000CF9 RID: 3321
		private readonly IVoteConfigContainer m_voteConfigContainer;

		// Token: 0x04000CFA RID: 3322
		private readonly VoteType m_voteType;
	}
}
