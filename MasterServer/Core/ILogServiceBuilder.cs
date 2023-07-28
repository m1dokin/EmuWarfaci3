using System;
using HK2Net;

namespace MasterServer.Core
{
	// Token: 0x02000124 RID: 292
	[Contract]
	public interface ILogServiceBuilder
	{
		// Token: 0x060004BE RID: 1214
		Type Build(Type baseClass, Type[] ctorParams, Type _interface);
	}
}
