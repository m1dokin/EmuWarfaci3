using System;

namespace MasterServer.GFaceAPI
{
	// Token: 0x02000638 RID: 1592
	public class GFaceException : ApplicationException
	{
		// Token: 0x0600221D RID: 8733 RVA: 0x0008EB5B File Offset: 0x0008CF5B
		public GFaceException(GFaceError err) : base(err.ToString())
		{
			this.ErrorInfo = err;
		}

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x0600221E RID: 8734 RVA: 0x0008EB78 File Offset: 0x0008CF78
		public GErrorCode ErrorCode
		{
			get
			{
				return this.ErrorInfo.ErrorCode;
			}
		}

		// Token: 0x040010E1 RID: 4321
		public readonly GFaceError ErrorInfo;
	}
}
