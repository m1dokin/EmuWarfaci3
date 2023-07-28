using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MasterServer.Core.Timers;
using MasterServer.GameRoom.RoomExtensions.Vote.Exceptions;
using MasterServer.GameRoomSystem;
using Util.Common;

namespace MasterServer.GameRoom.RoomExtensions.Vote
{
	// Token: 0x020004D6 RID: 1238
	internal class GameVote : IDisposable
	{
		// Token: 0x06001ABD RID: 6845 RVA: 0x0006D87C File Offset: 0x0006BC7C
		public GameVote(VoteConfig config, RoomPlayer initiator, int initiatorTeamId, RoomPlayer target, IEnumerable<RoomPlayer> voters)
		{
			this.Initiator = initiator;
			this.InitiatorTeamId = initiatorTeamId;
			this.Target = target;
			this.Voters = voters.ToList<RoomPlayer>();
			this.m_votes = new Dictionary<ulong, bool>();
			this.m_voteTimer = new SafeTimer(new TimerCallback(this.VoteExpired), null, config.Timeout, config.Timeout);
			this.m_isCompleted = false;
			this.CalculateRequiredVotes(this.Voters.Count, (double)config.Threshold);
		}

		// Token: 0x14000065 RID: 101
		// (add) Token: 0x06001ABE RID: 6846 RVA: 0x0006D90C File Offset: 0x0006BD0C
		// (remove) Token: 0x06001ABF RID: 6847 RVA: 0x0006D944 File Offset: 0x0006BD44
		public event Action<GameVote, int, int> Updated;

		// Token: 0x14000066 RID: 102
		// (add) Token: 0x06001AC0 RID: 6848 RVA: 0x0006D97C File Offset: 0x0006BD7C
		// (remove) Token: 0x06001AC1 RID: 6849 RVA: 0x0006D9B4 File Offset: 0x0006BDB4
		public event Action<GameVote, VoteResult, int, int> Copmleted;

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x06001AC2 RID: 6850 RVA: 0x0006D9EA File Offset: 0x0006BDEA
		// (set) Token: 0x06001AC3 RID: 6851 RVA: 0x0006D9F2 File Offset: 0x0006BDF2
		public RoomPlayer Initiator { get; private set; }

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x06001AC4 RID: 6852 RVA: 0x0006D9FB File Offset: 0x0006BDFB
		// (set) Token: 0x06001AC5 RID: 6853 RVA: 0x0006DA03 File Offset: 0x0006BE03
		public int InitiatorTeamId { get; private set; }

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x06001AC6 RID: 6854 RVA: 0x0006DA0C File Offset: 0x0006BE0C
		// (set) Token: 0x06001AC7 RID: 6855 RVA: 0x0006DA14 File Offset: 0x0006BE14
		public RoomPlayer Target { get; private set; }

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x06001AC8 RID: 6856 RVA: 0x0006DA1D File Offset: 0x0006BE1D
		// (set) Token: 0x06001AC9 RID: 6857 RVA: 0x0006DA25 File Offset: 0x0006BE25
		public List<RoomPlayer> Voters { get; private set; }

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x06001ACA RID: 6858 RVA: 0x0006DA2E File Offset: 0x0006BE2E
		// (set) Token: 0x06001ACB RID: 6859 RVA: 0x0006DA36 File Offset: 0x0006BE36
		public int YesVotesRequired { get; private set; }

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x06001ACC RID: 6860 RVA: 0x0006DA3F File Offset: 0x0006BE3F
		// (set) Token: 0x06001ACD RID: 6861 RVA: 0x0006DA47 File Offset: 0x0006BE47
		public int NoVotesRequired { get; private set; }

		// Token: 0x06001ACE RID: 6862 RVA: 0x0006DA50 File Offset: 0x0006BE50
		public void Dispose()
		{
			this.StopTimer();
		}

		// Token: 0x06001ACF RID: 6863 RVA: 0x0006DA58 File Offset: 0x0006BE58
		public void Vote(ulong profileId, bool vote)
		{
			if (this.m_isCompleted)
			{
				return;
			}
			this.AddVote(profileId, vote);
			int num = this.m_votes.Values.Count((bool v) => v);
			int num2 = this.m_votes.Values.Count - num;
			if (num >= this.YesVotesRequired)
			{
				this.EndVote(VoteResult.Success, num, num2);
			}
			else if (num2 >= this.NoVotesRequired)
			{
				this.EndVote(VoteResult.Fail, num, num2);
			}
			this.Updated.SafeInvoke(this, num, num2);
		}

		// Token: 0x06001AD0 RID: 6864 RVA: 0x0006DAF7 File Offset: 0x0006BEF7
		private void CalculateRequiredVotes(int votersCount, double threshold)
		{
			this.YesVotesRequired = (int)Math.Ceiling((double)votersCount * threshold);
			this.NoVotesRequired = votersCount - this.YesVotesRequired + 1;
		}

		// Token: 0x06001AD1 RID: 6865 RVA: 0x0006DB1C File Offset: 0x0006BF1C
		private void AddVote(ulong profileId, bool vote)
		{
			if (!this.Voters.Exists((RoomPlayer p) => p.ProfileID == profileId))
			{
				throw new VoteException(string.Format("Player {0} is not listed on this vote", profileId));
			}
			if (this.m_votes.ContainsKey(profileId))
			{
				throw new VoteException(string.Format("Player {0} has already voted", profileId));
			}
			this.m_votes.Add(profileId, vote);
		}

		// Token: 0x06001AD2 RID: 6866 RVA: 0x0006DBB0 File Offset: 0x0006BFB0
		private void VoteExpired(object dummy)
		{
			this.EndVote(VoteResult.Timeout, -1, -1);
		}

		// Token: 0x06001AD3 RID: 6867 RVA: 0x0006DBBB File Offset: 0x0006BFBB
		private void EndVote(VoteResult result, int yes, int no)
		{
			this.StopTimer();
			this.m_isCompleted = true;
			this.Copmleted.SafeInvoke(this, result, yes, no);
		}

		// Token: 0x06001AD4 RID: 6868 RVA: 0x0006DBDC File Offset: 0x0006BFDC
		private void StopTimer()
		{
			object timerLock = this.m_timerLock;
			lock (timerLock)
			{
				if (this.m_voteTimer != null)
				{
					this.m_voteTimer.Dispose();
					this.m_voteTimer = null;
				}
			}
		}

		// Token: 0x04000CC8 RID: 3272
		private readonly object m_timerLock = new object();

		// Token: 0x04000CC9 RID: 3273
		private readonly Dictionary<ulong, bool> m_votes;

		// Token: 0x04000CCA RID: 3274
		private SafeTimer m_voteTimer;

		// Token: 0x04000CCB RID: 3275
		private bool m_isCompleted;
	}
}
