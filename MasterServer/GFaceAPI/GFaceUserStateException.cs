using System;

namespace MasterServer.GFaceAPI
{
	// Token: 0x02000641 RID: 1601
	public class GFaceUserStateException : GFaceLogicException
	{
		// Token: 0x06002227 RID: 8743 RVA: 0x0008EBDB File Offset: 0x0008CFDB
		public GFaceUserStateException(GFaceError err) : base(err)
		{
		}
	}
}
