using System;
using MasterServer.GameLogic.PersistentSettingsSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.SkillSystem;
using MasterServer.Users;

namespace MasterServer.Matchmaking.Data
{
	// Token: 0x02000595 RID: 1429
	internal class MMPlayerInfo
	{
		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06001EC1 RID: 7873 RVA: 0x0007D1AD File Offset: 0x0007B5AD
		// (set) Token: 0x06001EC2 RID: 7874 RVA: 0x0007D1B5 File Offset: 0x0007B5B5
		public UserInfo.User User { get; set; }

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06001EC3 RID: 7875 RVA: 0x0007D1BE File Offset: 0x0007B5BE
		// (set) Token: 0x06001EC4 RID: 7876 RVA: 0x0007D1C6 File Offset: 0x0007B5C6
		public PersistentSettings PersistentSettings { get; set; }

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06001EC5 RID: 7877 RVA: 0x0007D1CF File Offset: 0x0007B5CF
		// (set) Token: 0x06001EC6 RID: 7878 RVA: 0x0007D1D7 File Offset: 0x0007B5D7
		public ProfileProgressionInfo.PlayerClass PlayerCurrentClass { get; set; }

		// Token: 0x04000EFD RID: 3837
		public Skill Skill;
	}
}
