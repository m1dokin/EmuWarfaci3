using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MasterServer.Common
{
	// Token: 0x02000014 RID: 20
	public class AspectWrapperBase
	{
		// Token: 0x0600004D RID: 77 RVA: 0x00005560 File Offset: 0x00003960
		public AspectWrapperBase(Type iface)
		{
			List<Aspect> list = new List<Aspect>();
			foreach (MethodInfo methodInfo in iface.GetMethods())
			{
				list.AddRange(from attr in methodInfo.GetCustomAttributes(false)
				where attr is Aspect
				select (Aspect)attr);
			}
			this.m_aspects = list.ToArray();
		}

		// Token: 0x0400002A RID: 42
		protected Aspect[] m_aspects;
	}
}
