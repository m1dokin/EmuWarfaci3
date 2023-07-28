using System;
using System.Collections.Generic;

namespace MasterServer.Users
{
	// Token: 0x02000742 RID: 1858
	internal class ClassPresenceData
	{
		// Token: 0x06002654 RID: 9812 RVA: 0x000A2ACC File Offset: 0x000A0ECC
		public ClassPresenceData()
		{
			this.presence = new Dictionary<ulong, List<ClassPresenceData.PresenceData>>();
		}

		// Token: 0x040013C2 RID: 5058
		public Dictionary<ulong, List<ClassPresenceData.PresenceData>> presence;

		// Token: 0x040013C3 RID: 5059
		public string sessionId;

		// Token: 0x02000743 RID: 1859
		public struct PresenceData
		{
			// Token: 0x06002655 RID: 9813 RVA: 0x000A2ADF File Offset: 0x000A0EDF
			public PresenceData(int cid, int v)
			{
				this.ClassId = cid;
				this.PlayedTimeSec = v;
			}

			// Token: 0x040013C4 RID: 5060
			public int ClassId;

			// Token: 0x040013C5 RID: 5061
			public int PlayedTimeSec;
		}
	}
}
