using System;
using HK2Net;
using MasterServer.Core;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002DC RID: 732
	[Contract]
	internal interface IGameInterface
	{
		// Token: 0x06000FAC RID: 4012
		IGameInterfaceContext CreateContext(AccessLevel access_level);

		// Token: 0x06000FAD RID: 4013
		IGameInterfaceContext CreateContext(AccessLevel access_level, ILogGroup log_group);
	}
}
