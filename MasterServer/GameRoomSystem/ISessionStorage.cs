using System;
using HK2Net;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200061D RID: 1565
	[Contract]
	internal interface ISessionStorage
	{
		// Token: 0x060021A0 RID: 8608
		void AddData(string session_id, ESessionData type, object data);

		// Token: 0x060021A1 RID: 8609
		void RemoveData(string session_id, ESessionData type);

		// Token: 0x060021A2 RID: 8610
		void RemoveAllData(string sessionId);

		// Token: 0x060021A3 RID: 8611
		T GetData<T>(string session_id, ESessionData type);

		// Token: 0x060021A4 RID: 8612
		bool ValidateSession(string jid, string sessionId);
	}
}
