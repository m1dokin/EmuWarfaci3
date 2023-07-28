using System;

namespace MasterServer.Matchmaking.Data
{
	// Token: 0x02000515 RID: 1301
	[Serializable]
	public class MMPlayerDTO
	{
		// Token: 0x06001C34 RID: 7220 RVA: 0x00071ACC File Offset: 0x0006FECC
		public MMPlayerDTO(ulong profileId, ulong userId, string nickname, string onlineId, double skill, string groupId, char classId)
		{
			this.ProfileId = profileId;
			this.UserId = userId;
			this.Nickname = nickname;
			this.OnlineId = onlineId;
			this.Skill = skill;
			this.GroupId = groupId;
			this.ClassId = classId;
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06001C35 RID: 7221 RVA: 0x00071B09 File Offset: 0x0006FF09
		// (set) Token: 0x06001C36 RID: 7222 RVA: 0x00071B11 File Offset: 0x0006FF11
		public ulong ProfileId { get; private set; }

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06001C37 RID: 7223 RVA: 0x00071B1A File Offset: 0x0006FF1A
		// (set) Token: 0x06001C38 RID: 7224 RVA: 0x00071B22 File Offset: 0x0006FF22
		public ulong UserId { get; private set; }

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06001C39 RID: 7225 RVA: 0x00071B2B File Offset: 0x0006FF2B
		// (set) Token: 0x06001C3A RID: 7226 RVA: 0x00071B33 File Offset: 0x0006FF33
		public string Nickname { get; private set; }

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06001C3B RID: 7227 RVA: 0x00071B3C File Offset: 0x0006FF3C
		// (set) Token: 0x06001C3C RID: 7228 RVA: 0x00071B44 File Offset: 0x0006FF44
		public string OnlineId { get; private set; }

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06001C3D RID: 7229 RVA: 0x00071B4D File Offset: 0x0006FF4D
		// (set) Token: 0x06001C3E RID: 7230 RVA: 0x00071B55 File Offset: 0x0006FF55
		public double Skill { get; private set; }

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x06001C3F RID: 7231 RVA: 0x00071B5E File Offset: 0x0006FF5E
		// (set) Token: 0x06001C40 RID: 7232 RVA: 0x00071B66 File Offset: 0x0006FF66
		public string GroupId { get; private set; }

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x06001C41 RID: 7233 RVA: 0x00071B6F File Offset: 0x0006FF6F
		// (set) Token: 0x06001C42 RID: 7234 RVA: 0x00071B77 File Offset: 0x0006FF77
		public char ClassId { get; private set; }
	}
}
