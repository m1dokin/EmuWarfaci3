using System;

namespace MasterServer.GFaceAPI
{
	// Token: 0x0200063B RID: 1595
	public class GFaceFatalException : GFaceSystemException
	{
		// Token: 0x06002221 RID: 8737 RVA: 0x0008EBA5 File Offset: 0x0008CFA5
		public GFaceFatalException(GFaceError err) : base(err)
		{
		}
	}
}
