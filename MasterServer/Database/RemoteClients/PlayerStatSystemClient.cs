using System;
using System.Collections.Generic;
using System.Reflection;
using MasterServer.DAL;
using MasterServer.DAL.PlayerStats;
using OLAPHypervisor;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x0200020A RID: 522
	internal class PlayerStatSystemClient : DALCacheProxy<IDALService>, IPlayerStatSystemClient
	{
		// Token: 0x06000B11 RID: 2833 RVA: 0x00029982 File Offset: 0x00027D82
		internal void Reset(IPlayerStatsSystem system)
		{
			this.m_playerStatsSystem = system;
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x0002998C File Offset: 0x00027D8C
		public void UpdatePlayerStats(ulong profileId, List<Measure> data)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].stats,
				set_func = (() => this.m_playerStatsSystem.UpdatePlayerStats(profileId, data))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x000299F8 File Offset: 0x00027DF8
		public PlayerStatistics GetPlayerStats(ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<PlayerStatistics> options = new DALCacheProxy<IDALService>.SetOptionsScalar<PlayerStatistics>
			{
				cache_domain = cache_domains.profile[profileId].stats,
				set_func = (() => this.m_playerStatsSystem.GetPlayerStats(profileId))
			};
			return base.SetAndClearScalar<PlayerStatistics>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B14 RID: 2836 RVA: 0x00029A60 File Offset: 0x00027E60
		public void ResetPlayerStats(ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].stats,
				set_func = (() => this.m_playerStatsSystem.ResetPlayerStats(profileId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x0400055C RID: 1372
		private IPlayerStatsSystem m_playerStatsSystem;
	}
}
