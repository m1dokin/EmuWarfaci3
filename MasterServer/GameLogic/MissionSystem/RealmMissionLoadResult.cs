using System;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003A8 RID: 936
	internal class RealmMissionLoadResult
	{
		// Token: 0x060014B6 RID: 5302 RVA: 0x00055039 File Offset: 0x00053439
		public RealmMissionLoadResult(LoadResult code) : this(code, null)
		{
		}

		// Token: 0x060014B7 RID: 5303 RVA: 0x00055043 File Offset: 0x00053443
		public RealmMissionLoadResult(LoadResult code, MissionSet mset)
		{
			this.Code = code;
			this.MissionSet = mset;
		}

		// Token: 0x040009C6 RID: 2502
		public LoadResult Code;

		// Token: 0x040009C7 RID: 2503
		public MissionSet MissionSet;
	}
}
