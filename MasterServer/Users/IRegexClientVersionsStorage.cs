using System;
using System.Text.RegularExpressions;
using HK2Net;

namespace MasterServer.Users
{
	// Token: 0x02000755 RID: 1877
	[Contract]
	internal interface IRegexClientVersionsStorage : IClientVersionsStorage<Regex>
	{
	}
}
