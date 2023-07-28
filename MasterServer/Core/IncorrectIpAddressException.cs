using System;

namespace MasterServer.Core
{
	// Token: 0x02000113 RID: 275
	internal class IncorrectIpAddressException : Exception
	{
		// Token: 0x0600046F RID: 1135 RVA: 0x000138EC File Offset: 0x00011CEC
		public IncorrectIpAddressException(string message) : base(message)
		{
		}
	}
}
