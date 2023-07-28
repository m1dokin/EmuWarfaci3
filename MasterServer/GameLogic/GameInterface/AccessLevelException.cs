using System;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002E1 RID: 737
	public class AccessLevelException : Exception
	{
		// Token: 0x0600102B RID: 4139 RVA: 0x0003F416 File Offset: 0x0003D816
		public AccessLevelException() : base("Insufficient access level")
		{
		}
	}
}
