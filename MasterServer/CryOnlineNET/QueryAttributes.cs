using System;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x0200016D RID: 365
	internal class QueryAttributes : Attribute
	{
		// Token: 0x04000406 RID: 1030
		public string TagName;

		// Token: 0x04000407 RID: 1031
		public ECompressType CompressionType;

		// Token: 0x04000408 RID: 1032
		public string QoSClass;
	}
}
