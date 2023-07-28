using System;
using System.Collections.Generic;
using System.Reflection;
using MasterServer.DAL;
using MasterServer.DAL.Utils;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001F6 RID: 502
	internal class ColdStorageSystemClient : DALCacheProxy<IDALService>, IColdStorageSystemClient
	{
		// Token: 0x060009FA RID: 2554 RVA: 0x000256DF File Offset: 0x00023ADF
		internal void Reset(IColdStorageSystem coldStorage)
		{
			this.m_coldStorage = coldStorage;
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x000256E8 File Offset: 0x00023AE8
		public bool? IsProfileCold(ulong profile_id)
		{
			DALCacheProxy<IDALService>.Options<bool?> options = new DALCacheProxy<IDALService>.Options<bool?>
			{
				get_data = (() => this.m_coldStorage.IsProfileCold(profile_id))
			};
			return base.GetData<bool?>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009FC RID: 2556 RVA: 0x00025730 File Offset: 0x00023B30
		public TouchProfileResult TouchProfile(ulong profile_id, DBVersion current_schema)
		{
			DALCacheProxy<IDALService>.Options<TouchProfileResult> options = new DALCacheProxy<IDALService>.Options<TouchProfileResult>
			{
				get_data = (() => this.m_coldStorage.TouchProfile(profile_id, current_schema))
			};
			return base.GetData<TouchProfileResult>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x00025780 File Offset: 0x00023B80
		public bool MoveProfileToCold(ulong profile_id, TimeSpan threshold, DBVersion current_schema)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<bool> options = new DALCacheProxy<IDALService>.SetOptionsScalar<bool>
			{
				set_func = (() => this.m_coldStorage.MoveProfileToCold(profile_id, threshold, current_schema)),
				cache_domain = cache_domains.profile[profile_id]
			};
			return base.SetAndClearScalar<bool>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x000257F0 File Offset: 0x00023BF0
		public ColdProfileData GetColdProfileData(ulong profile_id, DBVersion current_schema)
		{
			DALCacheProxy<IDALService>.Options<ColdProfileData> options = new DALCacheProxy<IDALService>.Options<ColdProfileData>
			{
				get_data = (() => this.m_coldStorage.GetColdProfileData(profile_id, current_schema))
			};
			return base.GetData<ColdProfileData>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009FF RID: 2559 RVA: 0x00025840 File Offset: 0x00023C40
		public IEnumerable<ulong> GetUnusedProfiles(TimeSpan threshold, int limit)
		{
			DALCacheProxy<IDALService>.Options<ulong> options = new DALCacheProxy<IDALService>.Options<ulong>
			{
				get_data_stream = (() => this.m_coldStorage.GetUnusedProfiles(threshold, limit))
			};
			return base.GetDataStream<ulong>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x00025890 File Offset: 0x00023C90
		public IEnumerable<ulong> GetColdProfiles(int limit)
		{
			DALCacheProxy<IDALService>.Options<ulong> options = new DALCacheProxy<IDALService>.Options<ulong>
			{
				get_data_stream = (() => this.m_coldStorage.GetColdProfiles(limit))
			};
			return base.GetDataStream<ulong>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x04000550 RID: 1360
		private IColdStorageSystem m_coldStorage;
	}
}
