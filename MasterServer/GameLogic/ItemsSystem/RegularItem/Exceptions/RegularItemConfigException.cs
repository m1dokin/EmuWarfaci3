using System;

namespace MasterServer.GameLogic.ItemsSystem.RegularItem.Exceptions
{
	// Token: 0x02000070 RID: 112
	internal class RegularItemConfigException : ApplicationException
	{
		// Token: 0x060001B4 RID: 436 RVA: 0x0000B10D File Offset: 0x0000950D
		public RegularItemConfigException(string message) : base(message)
		{
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000B116 File Offset: 0x00009516
		public RegularItemConfigException(string paramName, object paramValue) : base(string.Format("Parameter {0} is invalid (value = '{1}')", paramName, paramValue))
		{
		}
	}
}
