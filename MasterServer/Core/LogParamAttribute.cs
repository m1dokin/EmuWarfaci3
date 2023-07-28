using System;

namespace MasterServer.Core
{
	// Token: 0x02000139 RID: 313
	[AttributeUsage(AttributeTargets.Parameter)]
	public class LogParamAttribute : Attribute
	{
		// Token: 0x0600051F RID: 1311 RVA: 0x0001669A File Offset: 0x00014A9A
		public LogParamAttribute(LogKey key)
		{
			this.Key = key;
		}

		// Token: 0x0400022E RID: 558
		public LogKey Key;
	}
}
