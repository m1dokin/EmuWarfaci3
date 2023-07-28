using System;
using HK2Net;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200061E RID: 1566
	[Contract]
	internal interface ISessionStorageDebug : ISessionStorage
	{
		// Token: 0x060021A5 RID: 8613
		void DbgDumpStorage();
	}
}
