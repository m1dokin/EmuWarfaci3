using System;

namespace MasterServer.GFaceAPI
{
	// Token: 0x0200063D RID: 1597
	public class GFaceThirdPartyException : GFaceSystemException
	{
		// Token: 0x06002223 RID: 8739 RVA: 0x0008EBB7 File Offset: 0x0008CFB7
		public GFaceThirdPartyException(GFaceError err) : base(err)
		{
		}
	}
}
