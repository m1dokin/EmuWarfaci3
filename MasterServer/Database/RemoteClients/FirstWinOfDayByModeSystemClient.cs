using System;
using System.Collections.Generic;
using System.Reflection;
using MasterServer.DAL.FirstWinOfDayByMode;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000043 RID: 67
	internal class FirstWinOfDayByModeSystemClient : DALCacheProxy<IDALService>, IFirstWinOfDayByModeSystemClient
	{
		// Token: 0x06000111 RID: 273 RVA: 0x000094C8 File Offset: 0x000078C8
		public bool SetPvpModeFirstWin(ulong profileId, string mode, DateTime nextOccurrence)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<bool> options = new DALCacheProxy<IDALService>.SetOptionsScalar<bool>
			{
				set_func = (() => this.m_firstWinOfDayByModeSystem.SetPvpModeFirstWin(profileId, mode, nextOccurrence)),
				cache_domain = cache_domains.profile[profileId].pvp_mode_current_wins
			};
			return base.SetAndClearScalar<bool>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000953C File Offset: 0x0000793C
		public IEnumerable<PvpModeWinNextOccurrence> GetPvpModesWinNextOccurrence(ulong profileId)
		{
			DALCacheProxy<IDALService>.Options<PvpModeWinNextOccurrence> options = new DALCacheProxy<IDALService>.Options<PvpModeWinNextOccurrence>
			{
				cache_domain = cache_domains.profile[profileId].pvp_mode_current_wins,
				get_data_stream = (() => this.m_firstWinOfDayByModeSystem.GetPvpModesWinNextOccurrence(profileId))
			};
			return base.GetDataStream<PvpModeWinNextOccurrence>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000113 RID: 275 RVA: 0x000095A4 File Offset: 0x000079A4
		public void ResetPvpModesFirstWin(ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_firstWinOfDayByModeSystem.ResetPvpModesFirstWin(profileId)),
				cache_domain = cache_domains.profile[profileId].pvp_mode_current_wins
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00009609 File Offset: 0x00007A09
		internal void Reset(IFirstWinOfDayByModeSystem firstWinOfDayByModeSystem)
		{
			this.m_firstWinOfDayByModeSystem = firstWinOfDayByModeSystem;
		}

		// Token: 0x0400008A RID: 138
		private IFirstWinOfDayByModeSystem m_firstWinOfDayByModeSystem;
	}
}
