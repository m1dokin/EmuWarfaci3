using System;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000067 RID: 103
	internal class ProfileNotFoundException : Exception
	{
		// Token: 0x0600018C RID: 396 RVA: 0x0000A419 File Offset: 0x00008819
		public ProfileNotFoundException(string nickName) : base(string.Format("Can't find profile with nickname {0}", nickName))
		{
		}
	}
}
