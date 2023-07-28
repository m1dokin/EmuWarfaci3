using System;

namespace MasterServer.GFaceAPI
{
	// Token: 0x02000640 RID: 1600
	public class GFaceSessionException : GFaceLogicException
	{
		// Token: 0x06002226 RID: 8742 RVA: 0x0008EBD2 File Offset: 0x0008CFD2
		public GFaceSessionException(GFaceError err) : base(err)
		{
		}
	}
}
