using System;

namespace MasterServer.Core
{
	// Token: 0x0200013A RID: 314
	[AttributeUsage(AttributeTargets.Field)]
	public class LogKeyAttribute : Attribute
	{
		// Token: 0x06000520 RID: 1312 RVA: 0x000166A9 File Offset: 0x00014AA9
		public LogKeyAttribute(Type type)
		{
			this.Type = type;
		}

		// Token: 0x0400022F RID: 559
		public Type Type;
	}
}
