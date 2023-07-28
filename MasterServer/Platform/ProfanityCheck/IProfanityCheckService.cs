using System;
using System.Text;
using HK2Net;
using HK2Net.Attributes.Bootstrap;

namespace MasterServer.Platform.ProfanityCheck
{
	// Token: 0x020006AF RID: 1711
	[Contract]
	[BootstrapExplicit]
	public interface IProfanityCheckService
	{
		// Token: 0x060023F0 RID: 9200
		ProfanityCheckResult CheckProfileName(ulong userId, string profileName);

		// Token: 0x060023F1 RID: 9201
		ProfanityCheckResult CheckClanName(ulong userId, string clanName);

		// Token: 0x060023F2 RID: 9202
		ProfanityCheckResult CheckRoomName(ulong userId, string userNickname, string roomName);

		// Token: 0x060023F3 RID: 9203
		ProfanityCheckResult FilterClanDescription(ulong userId, StringBuilder clanDescription);
	}
}
