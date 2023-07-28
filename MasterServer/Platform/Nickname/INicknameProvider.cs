using System;
using HK2Net;
using HK2Net.Attributes.Bootstrap;

namespace MasterServer.Platform.Nickname
{
	// Token: 0x0200068A RID: 1674
	[Contract]
	[BootstrapExplicit]
	internal interface INicknameProvider
	{
		// Token: 0x06002322 RID: 8994
		string GetNickname(ulong userId, ulong profileId);
	}
}
