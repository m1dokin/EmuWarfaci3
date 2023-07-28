using System;
using HK2Net;

namespace MasterServer.Database
{
	// Token: 0x02000038 RID: 56
	[Contract]
	internal interface IBaseDALService
	{
		// Token: 0x14000008 RID: 8
		// (add) Token: 0x060000D3 RID: 211
		// (remove) Token: 0x060000D4 RID: 212
		event Action<DALProxyStats> OnDALStats;

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060000D5 RID: 213
		DALConfig Config { get; }
	}
}
