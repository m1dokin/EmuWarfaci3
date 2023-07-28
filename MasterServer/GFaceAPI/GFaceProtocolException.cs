using System;

namespace MasterServer.GFaceAPI
{
	// Token: 0x0200063A RID: 1594
	public class GFaceProtocolException : GFaceSystemException
	{
		// Token: 0x06002220 RID: 8736 RVA: 0x0008EB9C File Offset: 0x0008CF9C
		public GFaceProtocolException(GFaceError err) : base(err)
		{
		}
	}
}
