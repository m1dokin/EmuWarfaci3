using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200077D RID: 1917
	public struct SlotInfo
	{
		// Token: 0x060027B9 RID: 10169 RVA: 0x000A9C3E File Offset: 0x000A803E
		public SlotInfo(string _name, ulong _id, string _classId, int _minCount, int _maxCount)
		{
			this.name = _name;
			this.minCount = _minCount;
			this.maxCount = _maxCount;
			this.classId = _classId;
			this.classIndex = -1;
			this.id = _id;
		}

		// Token: 0x060027BA RID: 10170 RVA: 0x000A9C6C File Offset: 0x000A806C
		public void Dump()
		{
			Log.Info<string>("Name: {0}", this.name);
			Log.Info<ulong>("Id: {0}", this.id);
			Log.Info<int>("Min: {0}", this.minCount);
			Log.Info<int>("Max: {0}", this.maxCount);
			Log.Info<string>("Class: {0}", this.classId);
		}

		// Token: 0x040014C1 RID: 5313
		public string name;

		// Token: 0x040014C2 RID: 5314
		public ulong id;

		// Token: 0x040014C3 RID: 5315
		public int minCount;

		// Token: 0x040014C4 RID: 5316
		public int maxCount;

		// Token: 0x040014C5 RID: 5317
		public string classId;

		// Token: 0x040014C6 RID: 5318
		public int classIndex;
	}
}
