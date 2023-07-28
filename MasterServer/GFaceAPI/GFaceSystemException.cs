using System;

namespace MasterServer.GFaceAPI
{
	// Token: 0x02000639 RID: 1593
	public class GFaceSystemException : GFaceException
	{
		// Token: 0x0600221F RID: 8735 RVA: 0x0008EB93 File Offset: 0x0008CF93
		public GFaceSystemException(GFaceError err) : base(err)
		{
		}
	}
}
