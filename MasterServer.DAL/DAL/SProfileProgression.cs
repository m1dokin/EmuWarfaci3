using System;

namespace MasterServer.DAL
{
	// Token: 0x0200007E RID: 126
	[Serializable]
	public struct SProfileProgression
	{
		// Token: 0x06000184 RID: 388 RVA: 0x00004CAC File Offset: 0x000030AC
		public bool IsEmpty()
		{
			return this.ProfileId == 0UL && this.MissionPassCounter == 0 && this.ZombieMissionPassCounter == 0 && this.CampaignPassCounter == 0 && this.VolcanoCampaignPasCounter == 0 && this.AnubisCampaignPassCounter == 0 && this.ZombieTowerCampaignPassCounter == 0 && this.IceBreakerCampaignPassCounter == 0 && this.TutorialUnlocked == 0 && this.TutorialPassed == 0 && this.ClassUnlocked == 0;
		}

		// Token: 0x04000147 RID: 327
		public ulong ProfileId;

		// Token: 0x04000148 RID: 328
		public int MissionPassCounter;

		// Token: 0x04000149 RID: 329
		public int ZombieMissionPassCounter;

		// Token: 0x0400014A RID: 330
		public int CampaignPassCounter;

		// Token: 0x0400014B RID: 331
		public int VolcanoCampaignPasCounter;

		// Token: 0x0400014C RID: 332
		public int AnubisCampaignPassCounter;

		// Token: 0x0400014D RID: 333
		public int ZombieTowerCampaignPassCounter;

		// Token: 0x0400014E RID: 334
		public int IceBreakerCampaignPassCounter;

		// Token: 0x0400014F RID: 335
		public int TutorialUnlocked;

		// Token: 0x04000150 RID: 336
		public int TutorialPassed;

		// Token: 0x04000151 RID: 337
		public int ClassUnlocked;

		// Token: 0x04000152 RID: 338
		public int MissionUnlocked;

		// Token: 0x04000153 RID: 339
		public static readonly SProfileProgression Empty = default(SProfileProgression);
	}
}
