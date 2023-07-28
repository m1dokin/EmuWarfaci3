using System;

namespace MasterServer.GFaceAPI
{
	// Token: 0x02000651 RID: 1617
	public class SimpleResponseParser : GFaceResponseParserImpl
	{
		// Token: 0x0600226D RID: 8813 RVA: 0x00090B38 File Offset: 0x0008EF38
		public SimpleResponseParser(string raw) : base(raw, null)
		{
		}

		// Token: 0x0600226E RID: 8814 RVA: 0x00090B42 File Offset: 0x0008EF42
		public SimpleResponseParser(string raw, bool bThrow) : base(raw, null, bThrow)
		{
		}
	}
}
