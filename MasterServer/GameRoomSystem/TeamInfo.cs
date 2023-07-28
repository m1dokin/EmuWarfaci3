using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004A3 RID: 1187
	internal class TeamInfo : IEnumerable<RoomPlayer>, IEnumerable
	{
		// Token: 0x06001947 RID: 6471 RVA: 0x00066E92 File Offset: 0x00065292
		public TeamInfo(int teamMaxSize, int teamId)
		{
			this.TeamId = teamId;
			this.m_teamMaxSize = teamMaxSize;
			this.m_players = new List<RoomPlayer>();
		}

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06001948 RID: 6472 RVA: 0x00066EB3 File Offset: 0x000652B3
		public int PlayersCount
		{
			get
			{
				return this.m_players.Count;
			}
		}

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06001949 RID: 6473 RVA: 0x00066EC0 File Offset: 0x000652C0
		// (set) Token: 0x0600194A RID: 6474 RVA: 0x00066EC8 File Offset: 0x000652C8
		public double TeamSkill { get; private set; }

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x0600194B RID: 6475 RVA: 0x00066ED1 File Offset: 0x000652D1
		// (set) Token: 0x0600194C RID: 6476 RVA: 0x00066ED9 File Offset: 0x000652D9
		public int TeamId { get; private set; }

		// Token: 0x0600194D RID: 6477 RVA: 0x00066EE2 File Offset: 0x000652E2
		public void AddPlayer(RoomPlayer roomPlayer)
		{
			this.m_players.Add(roomPlayer);
			this.TeamSkill += roomPlayer.Skill.Value;
		}

		// Token: 0x0600194E RID: 6478 RVA: 0x00066F08 File Offset: 0x00065308
		public void AddGroup(GroupInfo groupInfo)
		{
			this.m_players.AddRange(groupInfo.Players);
			this.TeamSkill += groupInfo.Skill;
		}

		// Token: 0x0600194F RID: 6479 RVA: 0x00066F2E File Offset: 0x0006532E
		public bool HasPlayer(RoomPlayer roomPlayer)
		{
			return this.m_players.Contains(roomPlayer);
		}

		// Token: 0x06001950 RID: 6480 RVA: 0x00066F3C File Offset: 0x0006533C
		public bool HasGroup(string groupId)
		{
			return this.m_players.Any((RoomPlayer x) => x.GroupID == groupId);
		}

		// Token: 0x06001951 RID: 6481 RVA: 0x00066F6D File Offset: 0x0006536D
		public bool HasFreeSlots()
		{
			return this.PlayersCount < this.m_teamMaxSize;
		}

		// Token: 0x06001952 RID: 6482 RVA: 0x00066F7D File Offset: 0x0006537D
		public bool HasSpaceForGroup(GroupInfo groupInfo)
		{
			return this.PlayersCount + groupInfo.Count <= this.m_teamMaxSize;
		}

		// Token: 0x06001953 RID: 6483 RVA: 0x00066F97 File Offset: 0x00065397
		public IEnumerator<RoomPlayer> GetEnumerator()
		{
			return this.m_players.GetEnumerator();
		}

		// Token: 0x06001954 RID: 6484 RVA: 0x00066FAC File Offset: 0x000653AC
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(string.Format("Team: {0}, Count:{1}, Skill:{2}\nPlayers:\n", this.TeamId, this.PlayersCount, this.TeamSkill));
			foreach (RoomPlayer roomPlayer in this.m_players)
			{
				stringBuilder.AppendLine(roomPlayer.ToString());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001955 RID: 6485 RVA: 0x00067048 File Offset: 0x00065448
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x04000C13 RID: 3091
		public const int InvalidTeam = -1;

		// Token: 0x04000C14 RID: 3092
		public const int NoTeam = 0;

		// Token: 0x04000C15 RID: 3093
		public const int FirstTeam = 1;

		// Token: 0x04000C16 RID: 3094
		public const int SecondTeam = 2;

		// Token: 0x04000C17 RID: 3095
		private readonly List<RoomPlayer> m_players;

		// Token: 0x04000C18 RID: 3096
		private readonly int m_teamMaxSize;
	}
}
