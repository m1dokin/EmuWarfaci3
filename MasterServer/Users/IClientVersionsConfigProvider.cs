using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HK2Net;

namespace MasterServer.Users
{
	// Token: 0x02000748 RID: 1864
	[Contract]
	internal interface IClientVersionsConfigProvider
	{
		// Token: 0x06002678 RID: 9848
		IEnumerable<Regex> GetSupportedVersions();

		// Token: 0x06002679 RID: 9849
		IEnumerable<string> GetInitialVersionSet();
	}
}
