using System;
using HK2Net;
using MasterServer.Matchmaking.Data;

namespace MasterServer.Matchmaking
{
	// Token: 0x02000510 RID: 1296
	[Contract]
	internal interface IMMEntityDTOFactory
	{
		// Token: 0x06001C20 RID: 7200
		MMEntityDTO Create(MMEntityInfo mmEntityInfo);
	}
}
