using System;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002E2 RID: 738
	[Service]
	[Singleton]
	internal class GameInterface : ServiceModule, IGameInterface
	{
		// Token: 0x0600102C RID: 4140 RVA: 0x0003F423 File Offset: 0x0003D823
		public GameInterface()
		{
			this.m_wrapper_type = AspectFactory.CreateWrapper(typeof(IGameInterfaceContext));
		}

		// Token: 0x0600102D RID: 4141 RVA: 0x0003F440 File Offset: 0x0003D840
		public IGameInterfaceContext CreateContext(AccessLevel access_level)
		{
			ILogService service = ServicesManager.GetService<ILogService>();
			return this.CreateContext(access_level, service.Event);
		}

		// Token: 0x0600102E RID: 4142 RVA: 0x0003F460 File Offset: 0x0003D860
		public IGameInterfaceContext CreateContext(AccessLevel access_level, ILogGroup log_group)
		{
			GameInterfaceContext wrapped = new GameInterfaceContext(access_level, log_group);
			return AspectFactory.CreateInstance<IGameInterfaceContext>(this.m_wrapper_type, wrapped);
		}

		// Token: 0x0400076D RID: 1901
		private Type m_wrapper_type;
	}
}
