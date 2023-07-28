using System;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200000B RID: 11
	internal class ProfileValidationServiceConfig
	{
		// Token: 0x06000023 RID: 35 RVA: 0x00004CC9 File Offset: 0x000030C9
		public ProfileValidationServiceConfig(bool isCheckAchievementsEnabled, bool isCheckProfileEnabled, bool isCheckClanEnabled, bool isCheckHeadEnabled)
		{
			this.IsCheckAchievementsEnabled = isCheckAchievementsEnabled;
			this.IsCheckProfileEnabled = isCheckProfileEnabled;
			this.IsCheckClanEnabled = isCheckClanEnabled;
			this.IsCheckHeadEnabled = isCheckHeadEnabled;
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00004CEE File Offset: 0x000030EE
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00004CF6 File Offset: 0x000030F6
		public bool IsCheckAchievementsEnabled { get; private set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00004CFF File Offset: 0x000030FF
		// (set) Token: 0x06000027 RID: 39 RVA: 0x00004D07 File Offset: 0x00003107
		public bool IsCheckProfileEnabled { get; private set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00004D10 File Offset: 0x00003110
		// (set) Token: 0x06000029 RID: 41 RVA: 0x00004D18 File Offset: 0x00003118
		public bool IsCheckClanEnabled { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00004D21 File Offset: 0x00003121
		// (set) Token: 0x0600002B RID: 43 RVA: 0x00004D29 File Offset: 0x00003129
		public bool IsCheckHeadEnabled { get; private set; }
	}
}
