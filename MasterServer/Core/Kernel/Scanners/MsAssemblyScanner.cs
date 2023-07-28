using System;
using System.Collections.Generic;
using HK2Net.Scanners;

namespace MasterServer.Core.Kernel.Scanners
{
	// Token: 0x02000035 RID: 53
	public class MsAssemblyScanner : AssemblyScanner
	{
		// Token: 0x060000CC RID: 204 RVA: 0x00007D20 File Offset: 0x00006120
		protected override IEnumerable<string> GetAssemblyNames()
		{
			yield return "MasterServer.DAL.Impl";
			yield return "MasterServer.Core.Services.Application";
			yield break;
		}
	}
}
