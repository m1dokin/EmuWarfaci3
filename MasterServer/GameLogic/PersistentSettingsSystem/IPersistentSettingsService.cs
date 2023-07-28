using System;
using HK2Net;

namespace MasterServer.GameLogic.PersistentSettingsSystem
{
	// Token: 0x020003F4 RID: 1012
	[Contract]
	internal interface IPersistentSettingsService
	{
		// Token: 0x060015ED RID: 5613
		PersistentSettings GetProfileSettings(ulong profileId);
	}
}
