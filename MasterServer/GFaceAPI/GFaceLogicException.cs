using System;

namespace MasterServer.GFaceAPI
{
	// Token: 0x0200063F RID: 1599
	public class GFaceLogicException : GFaceException
	{
		// Token: 0x06002225 RID: 8741 RVA: 0x0008EBC0 File Offset: 0x0008CFC0
		public GFaceLogicException(GFaceError err) : base(err)
		{
		}
	}
}
