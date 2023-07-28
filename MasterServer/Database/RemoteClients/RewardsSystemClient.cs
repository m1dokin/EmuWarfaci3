using System;
using System.Collections.Generic;
using System.Reflection;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000212 RID: 530
	internal class RewardsSystemClient : DALCacheProxy<IDALService>, IRewardsSystemClient
	{
		// Token: 0x06000B83 RID: 2947 RVA: 0x0002B6AC File Offset: 0x00029AAC
		internal void Reset(IRewardsSystem rewardsSystem)
		{
			this.m_rewardsSystem = rewardsSystem;
		}

		// Token: 0x06000B84 RID: 2948 RVA: 0x0002B6B8 File Offset: 0x00029AB8
		public IEnumerable<SSponsorPoints> GetSponsorPoints(ulong profileId)
		{
			DALCacheProxy<IDALService>.Options<SSponsorPoints> options = new DALCacheProxy<IDALService>.Options<SSponsorPoints>
			{
				cache_domain = cache_domains.profile[profileId].sponsors,
				get_data_stream = (() => this.m_rewardsSystem.GetSponsorPoints(profileId))
			};
			return base.GetDataStream<SSponsorPoints>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B85 RID: 2949 RVA: 0x0002B720 File Offset: 0x00029B20
		public void SetSponsorPoints(ulong profileId, uint sponsorId, ulong sponsorPts)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].sponsors,
				set_func = (() => this.m_rewardsSystem.SetSponsorPoints(profileId, sponsorId, sponsorPts))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B86 RID: 2950 RVA: 0x0002B794 File Offset: 0x00029B94
		public bool SetSponsorInfo(ulong profileId, uint sponsorId, ulong oldSpPts, SRankInfo new_sp)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<bool> options = new DALCacheProxy<IDALService>.SetOptionsScalar<bool>
			{
				query_retry = base.DAL.Config.QueryRetry,
				cache_domain = cache_domains.profile[profileId].sponsors,
				set_func = (() => this.m_rewardsSystem.SetSponsorInfo(profileId, sponsorId, oldSpPts, new_sp))
			};
			return base.SetAndClearScalar<bool>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B87 RID: 2951 RVA: 0x0002B828 File Offset: 0x00029C28
		public void SetNextUnlockItem(ulong profileId, uint sponsorId, ulong nextUnlockItemId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].sponsors,
				set_func = (() => this.m_rewardsSystem.SetNextUnlockItem(profileId, sponsorId, nextUnlockItemId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x04000562 RID: 1378
		private IRewardsSystem m_rewardsSystem;
	}
}
