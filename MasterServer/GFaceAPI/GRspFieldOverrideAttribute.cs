using System;

namespace MasterServer.GFaceAPI
{
	// Token: 0x0200064E RID: 1614
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class GRspFieldOverrideAttribute : Attribute
	{
		// Token: 0x0400111E RID: 4382
		public string MapTo;
	}
}
