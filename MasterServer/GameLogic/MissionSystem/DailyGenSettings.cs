using System;
using System.Collections.Generic;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x0200039E RID: 926
	internal class DailyGenSettings
	{
		// Token: 0x040009A8 RID: 2472
		public string MissionType;

		// Token: 0x040009A9 RID: 2473
		public string Difficulty;

		// Token: 0x040009AA RID: 2474
		public string PropagateOnExpire;

		// Token: 0x040009AB RID: 2475
		public List<DailyGenSettings.SettingCfg> Settings;

		// Token: 0x040009AC RID: 2476
		public List<string> LevelGraphs;

		// Token: 0x040009AD RID: 2477
		public List<string> TimesOfDay;

		// Token: 0x040009AE RID: 2478
		public int GenerateCount;

		// Token: 0x040009AF RID: 2479
		public int GenerateStep;

		// Token: 0x040009B0 RID: 2480
		public int ExpireCount;

		// Token: 0x040009B1 RID: 2481
		public int SoftShuffleGenerate;

		// Token: 0x040009B2 RID: 2482
		public int SecondaryObjMin;

		// Token: 0x040009B3 RID: 2483
		public int SecondaryObjMax;

		// Token: 0x0200039F RID: 927
		public class SettingCfg
		{
			// Token: 0x0600148C RID: 5260 RVA: 0x000536FC File Offset: 0x00051AFC
			public SettingCfg(string name, List<string> ss)
			{
				this.Setting = name;
				this.SublevelRestrictions.Clear();
				foreach (string name2 in ss)
				{
					this.SublevelRestrictions.Add(new MissionGenerator.GenerateParams.SubLevelRestriction(name2));
				}
			}

			// Token: 0x040009B4 RID: 2484
			public string Setting;

			// Token: 0x040009B5 RID: 2485
			public List<MissionGenerator.GenerateParams.SubLevelRestriction> SublevelRestrictions = new List<MissionGenerator.GenerateParams.SubLevelRestriction>();
		}
	}
}
