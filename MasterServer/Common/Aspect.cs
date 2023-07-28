using System;
using System.Reflection;

namespace MasterServer.Common
{
	// Token: 0x02000013 RID: 19
	public interface Aspect
	{
		// Token: 0x0600004C RID: 76
		void PreCall(object thiz, MethodInfo method);
	}
}
