using System;
using HK2Net;

namespace MasterServer.MySqlQueries
{
	// Token: 0x0200067B RID: 1659
	[Service]
	[Singleton]
	internal class EmptyUserIdValidator : IUserIdValidator
	{
		// Token: 0x06002302 RID: 8962 RVA: 0x0009281E File Offset: 0x00090C1E
		public bool ValidateAgainstJid(string jid, ulong userId)
		{
			return true;
		}
	}
}
