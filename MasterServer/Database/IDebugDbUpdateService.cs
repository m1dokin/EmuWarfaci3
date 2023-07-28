using System;
using HK2Net;

namespace MasterServer.Database
{
	// Token: 0x020001D4 RID: 468
	[Contract]
	public interface IDebugDbUpdateService
	{
		// Token: 0x060008DE RID: 2270
		void DropProcedure(string procedureName);

		// Token: 0x060008DF RID: 2271
		void RestoreProcedures();
	}
}
