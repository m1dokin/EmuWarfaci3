using System;
using HK2Net;

namespace MasterServer.GameLogic.LobbyChat
{
	// Token: 0x0200039A RID: 922
	[Contract]
	internal interface IChatConferences
	{
		// Token: 0x06001480 RID: 5248
		ChatChannelID GenerateChannelId(EChatChannel channel, ulong profileID);
	}
}
