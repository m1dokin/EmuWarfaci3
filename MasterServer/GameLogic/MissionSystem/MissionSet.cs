using System;
using System.Collections.Generic;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003AF RID: 943
	internal class MissionSet
	{
		// Token: 0x040009D4 RID: 2516
		public string Hash;

		// Token: 0x040009D5 RID: 2517
		public int Generation;

		// Token: 0x040009D6 RID: 2518
		public Dictionary<Guid, MissionContext> Missions = new Dictionary<Guid, MissionContext>();
	}
}
