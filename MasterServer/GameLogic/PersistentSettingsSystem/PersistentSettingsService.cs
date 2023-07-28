using System;
using HK2Net;
using MasterServer.Core.Services;
using MasterServer.Database;

namespace MasterServer.GameLogic.PersistentSettingsSystem
{
	// Token: 0x020003F6 RID: 1014
	[Service]
	[Singleton]
	internal class PersistentSettingsService : ServiceModule, IPersistentSettingsService
	{
		// Token: 0x060015F3 RID: 5619 RVA: 0x0005B951 File Offset: 0x00059D51
		public PersistentSettingsService(IDALService dalService)
		{
			this.m_dalService = dalService;
		}

		// Token: 0x060015F4 RID: 5620 RVA: 0x0005B960 File Offset: 0x00059D60
		public PersistentSettings GetProfileSettings(ulong profileId)
		{
			return new PersistentSettings(profileId, this.m_dalService);
		}

		// Token: 0x04000A84 RID: 2692
		private readonly IDALService m_dalService;
	}
}
