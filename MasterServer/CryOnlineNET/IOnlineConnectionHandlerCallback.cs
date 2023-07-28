using System;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000184 RID: 388
	internal interface IOnlineConnectionHandlerCallback
	{
		// Token: 0x06000710 RID: 1808
		void OnConnectionStateChanged(EConnectionState current);

		// Token: 0x06000711 RID: 1809
		void OnUserStatus(string online_id, string prev_status, string new_status);

		// Token: 0x06000712 RID: 1810
		void OnQueryStats(OnlineQueryStats stats);

		// Token: 0x06000713 RID: 1811
		void OnLogMessage(EOnlineLogLevel level, string msg);
	}
}
