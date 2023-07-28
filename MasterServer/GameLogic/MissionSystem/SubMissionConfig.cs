using System;
using System.Collections.Generic;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003C8 RID: 968
	internal class SubMissionConfig
	{
		// Token: 0x04000A2D RID: 2605
		public string LevelName;

		// Token: 0x04000A2E RID: 2606
		public string Setting;

		// Token: 0x04000A2F RID: 2607
		public SubMissionConfig BaseLevel;

		// Token: 0x04000A30 RID: 2608
		public SubMissionConfig.ParameterSet[] ParameterSets;

		// Token: 0x020003C9 RID: 969
		public class ParameterSet
		{
			// Token: 0x06001556 RID: 5462 RVA: 0x000596A2 File Offset: 0x00057AA2
			public string GetKey()
			{
				return this.SubMission.LevelName + ' ' + this.MissionFlow;
			}

			// Token: 0x06001557 RID: 5463 RVA: 0x000596C4 File Offset: 0x00057AC4
			public override string ToString()
			{
				return string.Format("{0} {1} {2} {3} {4}", new object[]
				{
					this.SubMission.LevelName,
					this.Difficulty,
					this.Kind,
					this.MissionType,
					this.MissionFlow
				});
			}

			// Token: 0x04000A31 RID: 2609
			public SubMissionConfig SubMission;

			// Token: 0x04000A32 RID: 2610
			public string Difficulty;

			// Token: 0x04000A33 RID: 2611
			public string DifficultyCfg;

			// Token: 0x04000A34 RID: 2612
			public string Kind;

			// Token: 0x04000A35 RID: 2613
			public string MissionType;

			// Token: 0x04000A36 RID: 2614
			public string MissionFlow;

			// Token: 0x04000A37 RID: 2615
			public int TimeLimit;

			// Token: 0x04000A38 RID: 2616
			public int Score;

			// Token: 0x04000A39 RID: 2617
			public string TeleportStart;

			// Token: 0x04000A3A RID: 2618
			public string TeleportFinish;

			// Token: 0x04000A3B RID: 2619
			public int ScorePool;

			// Token: 0x04000A3C RID: 2620
			public int WinPool;

			// Token: 0x04000A3D RID: 2621
			public int LosePool;

			// Token: 0x04000A3E RID: 2622
			public int DrawPool;

			// Token: 0x04000A3F RID: 2623
			public Dictionary<string, SubMissionConfig.ParameterSet.Threshold> CrownThresholds;

			// Token: 0x04000A40 RID: 2624
			public Dictionary<string, string> PoolRewards;

			// Token: 0x020003CA RID: 970
			public struct Threshold
			{
				// Token: 0x06001558 RID: 5464 RVA: 0x00059713 File Offset: 0x00057B13
				public Threshold(uint b, uint s, uint g)
				{
					this.Bronze = b;
					this.Silver = s;
					this.Gold = g;
				}

				// Token: 0x04000A41 RID: 2625
				public uint Bronze;

				// Token: 0x04000A42 RID: 2626
				public uint Silver;

				// Token: 0x04000A43 RID: 2627
				public uint Gold;
			}
		}
	}
}
