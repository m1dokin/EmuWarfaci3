using System;

namespace MasterServer.ElectronicCatalog.Exceptions
{
	// Token: 0x0200004F RID: 79
	internal class ECatGiveMoneyTransactionException : Exception
	{
		// Token: 0x06000138 RID: 312 RVA: 0x00009934 File Offset: 0x00007D34
		public ECatGiveMoneyTransactionException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
