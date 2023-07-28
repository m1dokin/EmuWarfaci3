using System;
using System.Collections.Generic;
using MasterServer.DAL.CustomRules;
using MasterServer.DAL.PlayerStats;
using MasterServer.DAL.RatingSystem;

namespace MasterServer.DAL
{
	// Token: 0x0200003D RID: 61
	[Serializable]
	public class ColdProfileData
	{
		// Token: 0x0400008C RID: 140
		public List<SEquipItem> EquipItems = new List<SEquipItem>();

		// Token: 0x0400008D RID: 141
		public List<ulong> UnlockItems = new List<ulong>();

		// Token: 0x0400008E RID: 142
		public List<AchievementInfo> Achievements = new List<AchievementInfo>();

		// Token: 0x0400008F RID: 143
		public List<SSponsorPoints> SponsorPoints = new List<SSponsorPoints>();

		// Token: 0x04000090 RID: 144
		public List<SPersistentSettings> PersistentSettings = new List<SPersistentSettings>();

		// Token: 0x04000091 RID: 145
		public PlayerStatistics PlayerStatistics;

		// Token: 0x04000092 RID: 146
		public ProfileContract ProfileContract;

		// Token: 0x04000093 RID: 147
		public SProfileProgression ProfileProgression = SProfileProgression.Empty;

		// Token: 0x04000094 RID: 148
		public List<CustomRuleRawState> CustomRuleStates = new List<CustomRuleRawState>();

		// Token: 0x04000095 RID: 149
		public IList<SkillInfo> SkillInfos = new List<SkillInfo>();

		// Token: 0x04000096 RID: 150
		public RatingInfo RatingInfo;

		// Token: 0x04000097 RID: 151
		public RatingGamePlayerBanInfo RatingGamePlayerBan = new RatingGamePlayerBanInfo();
	}
}
