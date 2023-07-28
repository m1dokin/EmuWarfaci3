using System;
using HK2Net;
using Network.Interfaces;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002EE RID: 750
	[Contract]
	public interface IGiController : IRemoteService<GiCommandRequest, GiCommandResponse>, IDisposable
	{
	}
}
