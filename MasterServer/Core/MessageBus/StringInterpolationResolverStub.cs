using System;
using HK2Net;

namespace MasterServer.Core.MessageBus
{
	// Token: 0x02000037 RID: 55
	[Service]
	[Singleton]
	public class StringInterpolationResolverStub : IStringInterpollationResolver
	{
		// Token: 0x060000D2 RID: 210 RVA: 0x0000804D File Offset: 0x0000644D
		public string Resolve(string source)
		{
			return source.Replace("{server_name}", Resources.ServerName);
		}
	}
}
