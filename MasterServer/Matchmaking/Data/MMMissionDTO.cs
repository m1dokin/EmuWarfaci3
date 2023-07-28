using System;

namespace MasterServer.Matchmaking.Data
{
	// Token: 0x02000512 RID: 1298
	[Serializable]
	internal class MMMissionDTO
	{
		// Token: 0x06001C23 RID: 7203 RVA: 0x00071902 File Offset: 0x0006FD02
		public MMMissionDTO(string missionId, bool isTeamMode, int minPlayersToJoin, int minPlayersToCreate, int maxPlayers, char[] classPattern)
		{
			this.MissionId = missionId;
			this.Teams = isTeamMode;
			this.MinPlayersToJoin = minPlayersToJoin;
			this.MinPlayersToCreate = minPlayersToCreate;
			this.MaxPlayers = maxPlayers;
			this.ClassPattern = classPattern;
		}

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x06001C24 RID: 7204 RVA: 0x00071937 File Offset: 0x0006FD37
		// (set) Token: 0x06001C25 RID: 7205 RVA: 0x0007193F File Offset: 0x0006FD3F
		public string MissionId { get; private set; }

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06001C26 RID: 7206 RVA: 0x00071948 File Offset: 0x0006FD48
		// (set) Token: 0x06001C27 RID: 7207 RVA: 0x00071950 File Offset: 0x0006FD50
		public bool Teams { get; private set; }

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x06001C28 RID: 7208 RVA: 0x00071959 File Offset: 0x0006FD59
		// (set) Token: 0x06001C29 RID: 7209 RVA: 0x00071961 File Offset: 0x0006FD61
		public int MinPlayersToJoin { get; private set; }

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06001C2A RID: 7210 RVA: 0x0007196A File Offset: 0x0006FD6A
		// (set) Token: 0x06001C2B RID: 7211 RVA: 0x00071972 File Offset: 0x0006FD72
		public int MinPlayersToCreate { get; private set; }

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06001C2C RID: 7212 RVA: 0x0007197B File Offset: 0x0006FD7B
		// (set) Token: 0x06001C2D RID: 7213 RVA: 0x00071983 File Offset: 0x0006FD83
		public int MaxPlayers { get; private set; }

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06001C2E RID: 7214 RVA: 0x0007198C File Offset: 0x0006FD8C
		// (set) Token: 0x06001C2F RID: 7215 RVA: 0x00071994 File Offset: 0x0006FD94
		public char[] ClassPattern { get; private set; }

		// Token: 0x06001C30 RID: 7216 RVA: 0x000719A0 File Offset: 0x0006FDA0
		public override string ToString()
		{
			return string.Format("mission_id: {0}, teams: {1}, min_players_to_join: {2}, min_players_to_create: {3}, max_players: {4}, class_pattern: {5}", new object[]
			{
				this.MissionId,
				this.Teams,
				this.MinPlayersToJoin,
				this.MinPlayersToCreate,
				this.MaxPlayers,
				string.Join<char>(string.Empty, this.ClassPattern)
			});
		}
	}
}
