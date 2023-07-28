using System;
using HK2Net;
using HK2Net.Attributes.Bootstrap;

namespace MasterServer.MySqlQueries
{
	// Token: 0x0200067C RID: 1660
	[Contract]
	[BootstrapExplicit]
	internal interface IUserIdValidator
	{
		// Token: 0x06002303 RID: 8963
		bool ValidateAgainstJid(string jid, ulong userId);
	}
}
