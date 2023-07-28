using System;

namespace MasterServer.GFaceAPI
{
	// Token: 0x02000650 RID: 1616
	public class CompleteResponseParser<TResult> : GFaceResponseParserImpl
	{
		// Token: 0x0600226A RID: 8810 RVA: 0x00090B08 File Offset: 0x0008EF08
		public CompleteResponseParser(string raw) : base(raw, typeof(TResult), true)
		{
		}

		// Token: 0x0600226B RID: 8811 RVA: 0x00090B1C File Offset: 0x0008EF1C
		public CompleteResponseParser(string raw, bool bThrow) : base(raw, typeof(TResult), bThrow)
		{
		}

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x0600226C RID: 8812 RVA: 0x00090B30 File Offset: 0x0008EF30
		public TResult Response
		{
			get
			{
				return base.GetResponse<TResult>();
			}
		}
	}
}
