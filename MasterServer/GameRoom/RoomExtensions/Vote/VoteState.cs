using System;
using System.Collections.Generic;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.RoomExtensions.Vote
{
	// Token: 0x020004ED RID: 1261
	[RoomState(new Type[]
	{
		typeof(KickVoteExtension),
		typeof(SurrenderVoteExtension)
	})]
	internal class VoteState : RoomStateBase, IDisposable
	{
		// Token: 0x06001B1F RID: 6943 RVA: 0x0006EC80 File Offset: 0x0006D080
		public VoteState()
		{
			this.VotesInProgress = new Dictionary<int, GameVote>();
			this.VotesCooldowns = new Dictionary<ulong, DateTime>();
		}

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06001B20 RID: 6944 RVA: 0x0006EC9E File Offset: 0x0006D09E
		// (set) Token: 0x06001B21 RID: 6945 RVA: 0x0006ECA6 File Offset: 0x0006D0A6
		public Dictionary<int, GameVote> VotesInProgress { get; private set; }

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06001B22 RID: 6946 RVA: 0x0006ECAF File Offset: 0x0006D0AF
		// (set) Token: 0x06001B23 RID: 6947 RVA: 0x0006ECB7 File Offset: 0x0006D0B7
		public Dictionary<ulong, DateTime> VotesCooldowns { get; private set; }

		// Token: 0x06001B24 RID: 6948 RVA: 0x0006ECC0 File Offset: 0x0006D0C0
		public override object Clone()
		{
			VoteState voteState = (VoteState)base.Clone();
			voteState.VotesInProgress = new Dictionary<int, GameVote>();
			foreach (KeyValuePair<int, GameVote> keyValuePair in this.VotesInProgress)
			{
				voteState.VotesInProgress[keyValuePair.Key] = keyValuePair.Value;
			}
			voteState.VotesCooldowns = new Dictionary<ulong, DateTime>();
			foreach (KeyValuePair<ulong, DateTime> keyValuePair2 in this.VotesCooldowns)
			{
				voteState.VotesCooldowns[keyValuePair2.Key] = keyValuePair2.Value;
			}
			return voteState;
		}

		// Token: 0x06001B25 RID: 6949 RVA: 0x0006EDB0 File Offset: 0x0006D1B0
		public void Dispose()
		{
			foreach (GameVote gameVote in this.VotesInProgress.Values)
			{
				gameVote.Dispose();
			}
		}
	}
}
