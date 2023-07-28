using System;
using System.Collections.Generic;

namespace MasterServer.DAL
{
	// Token: 0x02000038 RID: 56
	[Serializable]
	public class SoftShufflePoolData
	{
		// Token: 0x04000086 RID: 134
		public string m_key;

		// Token: 0x04000087 RID: 135
		public int m_softShuffleIdx;

		// Token: 0x04000088 RID: 136
		public int m_marker;

		// Token: 0x04000089 RID: 137
		public List<SoftShufflePoolElement> m_elements = new List<SoftShufflePoolElement>();
	}
}
