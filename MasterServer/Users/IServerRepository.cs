using System;
using HK2Net;

namespace MasterServer.Users
{
	// Token: 0x02000758 RID: 1880
	[Contract]
	public interface IServerRepository
	{
		// Token: 0x140000A7 RID: 167
		// (add) Token: 0x060026CF RID: 9935
		// (remove) Token: 0x060026D0 RID: 9936
		event OnServerBindHandler OnServerBind;

		// Token: 0x060026D1 RID: 9937
		bool IsOnline(string jid);

		// Token: 0x060026D2 RID: 9938
		string GetServerID(string online_id);

		// Token: 0x060026D3 RID: 9939
		void AddServer(string online_id, string server_id);

		// Token: 0x060026D4 RID: 9940
		void RemoveServer(string online_id);

		// Token: 0x060026D5 RID: 9941
		void RemoveAllServers();
	}
}
