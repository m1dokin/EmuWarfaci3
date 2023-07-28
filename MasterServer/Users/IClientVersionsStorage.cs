using System;
using System.Collections.Generic;

namespace MasterServer.Users
{
	// Token: 0x02000752 RID: 1874
	internal interface IClientVersionsStorage<TVersion>
	{
		// Token: 0x060026AD RID: 9901
		bool IsVersionsSetUpToDate();

		// Token: 0x060026AE RID: 9902
		IEnumerable<TVersion> GetVersions();

		// Token: 0x060026AF RID: 9903
		void StoreVersions(IEnumerable<TVersion> versions);
	}
}
