using System;
using HK2Net;

namespace MasterServer.Database
{
	// Token: 0x020001D3 RID: 467
	[Contract]
	public interface IDBUpdateService
	{
		// Token: 0x060008D3 RID: 2259
		DBUpdateStage GetCurrentUpdateStage();

		// Token: 0x060008D4 RID: 2260
		bool RunUpdateChain();

		// Token: 0x060008D5 RID: 2261
		void SyncWithSlaves();

		// Token: 0x060008D6 RID: 2262
		string GetDataGroupHash(string group);

		// Token: 0x060008D7 RID: 2263
		string GetECatDataGroupHash(string group);

		// Token: 0x060008D8 RID: 2264
		string GetDataGroupHash(string group, bool refresh);

		// Token: 0x060008D9 RID: 2265
		string GetECatDataGroupHash(string group, bool refresh);

		// Token: 0x060008DA RID: 2266
		void SetDataGroupHash(string group, string hash);

		// Token: 0x060008DB RID: 2267
		void SetECatDataGroupHash(string group, string hash);

		// Token: 0x060008DC RID: 2268
		void RegisterListener(IDBUpdateListener listener);

		// Token: 0x060008DD RID: 2269
		void UnregisterListener(IDBUpdateListener listener);
	}
}
