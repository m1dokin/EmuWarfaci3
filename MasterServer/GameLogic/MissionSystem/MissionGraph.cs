using System;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003C0 RID: 960
	internal class MissionGraph
	{
		// Token: 0x04000A14 RID: 2580
		public string Name;

		// Token: 0x04000A15 RID: 2581
		public string SettingRestriction;

		// Token: 0x04000A16 RID: 2582
		public string GameMode;

		// Token: 0x04000A17 RID: 2583
		public string Difficulty;

		// Token: 0x04000A18 RID: 2584
		public int SecondaryObjectives;

		// Token: 0x04000A19 RID: 2585
		public MissionGraph.SubMissionPattern BaseMissionPattern;

		// Token: 0x04000A1A RID: 2586
		public MissionGraph.SubMissionPattern[] SubMissionPatterns;

		// Token: 0x04000A1B RID: 2587
		public MissionGraph.UIInfo UI_Info;

		// Token: 0x020003C1 RID: 961
		public struct UIInfo
		{
			// Token: 0x04000A1C RID: 2588
			public string DisplayName;

			// Token: 0x04000A1D RID: 2589
			public string DescriptionText;

			// Token: 0x04000A1E RID: 2590
			public string DescriptionIcon;

			// Token: 0x04000A1F RID: 2591
			public string GameModeText;

			// Token: 0x04000A20 RID: 2592
			public string GameModeIcon;
		}

		// Token: 0x020003C2 RID: 962
		public class SubMissionPattern
		{
			// Token: 0x0600153F RID: 5439 RVA: 0x00058717 File Offset: 0x00056B17
			public SubMissionPattern(string kind, string difficulty, string name, string missionFlow)
			{
				this.Kind = kind;
				this.Difficulty = difficulty;
				this.Name = name;
				this.MissionFlow = missionFlow;
			}

			// Token: 0x04000A21 RID: 2593
			public string Kind;

			// Token: 0x04000A22 RID: 2594
			public string Difficulty;

			// Token: 0x04000A23 RID: 2595
			public string Name;

			// Token: 0x04000A24 RID: 2596
			public string MissionFlow;
		}
	}
}
