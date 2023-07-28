using System;
using HK2Net;

namespace MasterServer.MySqlQueries
{
	// Token: 0x0200067E RID: 1662
	[Service]
	[Singleton]
	internal class UserIdValidator : IUserIdValidator
	{
		// Token: 0x06002307 RID: 8967 RVA: 0x00092F10 File Offset: 0x00091310
		public bool ValidateAgainstJid(string jid, ulong userId)
		{
			string s = jid.Split(new char[]
			{
				'@'
			})[0];
			ulong num;
			return ulong.TryParse(s, out num) && num == userId;
		}
	}
}
