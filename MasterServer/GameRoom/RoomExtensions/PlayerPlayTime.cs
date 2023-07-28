using System;

namespace MasterServer.GameRoom.RoomExtensions
{
	// Token: 0x020004BD RID: 1213
	internal class PlayerPlayTime
	{
		// Token: 0x06001A40 RID: 6720 RVA: 0x0006C10A File Offset: 0x0006A50A
		public PlayerPlayTime(ulong profileId, double skill)
		{
			this.ProfileId = profileId;
			this.Skill = skill;
			this.m_playTime = TimeSpan.Zero;
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x06001A41 RID: 6721 RVA: 0x0006C12B File Offset: 0x0006A52B
		// (set) Token: 0x06001A42 RID: 6722 RVA: 0x0006C133 File Offset: 0x0006A533
		public ulong ProfileId { get; private set; }

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x06001A43 RID: 6723 RVA: 0x0006C13C File Offset: 0x0006A53C
		// (set) Token: 0x06001A44 RID: 6724 RVA: 0x0006C144 File Offset: 0x0006A544
		public double Skill { get; private set; }

		// Token: 0x06001A45 RID: 6725 RVA: 0x0006C14D File Offset: 0x0006A54D
		public void UpdatePlayTime(TimeSpan playTime)
		{
			this.m_playTime += playTime;
		}

		// Token: 0x06001A46 RID: 6726 RVA: 0x0006C161 File Offset: 0x0006A561
		public double GetPlayTime()
		{
			return this.m_playTime.TotalSeconds;
		}

		// Token: 0x04000C93 RID: 3219
		private TimeSpan m_playTime;
	}
}
