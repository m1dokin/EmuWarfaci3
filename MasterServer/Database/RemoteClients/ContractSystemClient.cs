using System;
using System.Reflection;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001FA RID: 506
	internal class ContractSystemClient : DALCacheProxy<IDALService>, IContractSystemClient
	{
		// Token: 0x06000A21 RID: 2593 RVA: 0x00025D86 File Offset: 0x00024186
		internal void Reset(IContractSystem contractSystem)
		{
			this.m_contractSystem = contractSystem;
		}

		// Token: 0x06000A22 RID: 2594 RVA: 0x00025D90 File Offset: 0x00024190
		public ProfileContract AddContract(ulong profileId, ulong rotationId, TimeSpan nextRotateTime)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ProfileContract> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ProfileContract>
			{
				cache_domain = cache_domains.profile[profileId].contract,
				set_func = (() => this.m_contractSystem.AddContract(profileId, rotationId, nextRotateTime))
			};
			return base.SetAndClearScalar<ProfileContract>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x00025E04 File Offset: 0x00024204
		public ProfileContract SetContractProgress(ulong profileId, uint progress)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ProfileContract> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ProfileContract>
			{
				cache_domain = cache_domains.profile[profileId].contract,
				set_func = (() => this.m_contractSystem.SetContractProgress(profileId, progress))
			};
			return base.SetAndClearScalar<ProfileContract>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x00025E70 File Offset: 0x00024270
		public ProfileContract GetContractInfo(ulong profileId)
		{
			DALCacheProxy<IDALService>.Options<ProfileContract> options = new DALCacheProxy<IDALService>.Options<ProfileContract>
			{
				cache_domain = cache_domains.profile[profileId].contract,
				get_data = (() => this.m_contractSystem.GetContractInfo(profileId))
			};
			return base.GetData<ProfileContract>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x00025ED8 File Offset: 0x000242D8
		public ProfileContract ActivateContract(ulong profileId, ulong itemId, string itemName, uint progressTotal)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ProfileContract> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ProfileContract>
			{
				cache_domain = cache_domains.profile[profileId].contract,
				set_func = (() => this.m_contractSystem.ActivateContract(profileId, itemId, itemName, progressTotal))
			};
			return base.SetAndClearScalar<ProfileContract>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x00025F54 File Offset: 0x00024354
		public ProfileContract DeactivateContract(ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ProfileContract> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ProfileContract>
			{
				cache_domain = cache_domains.profile[profileId].contract,
				set_func = (() => this.m_contractSystem.DeactivateContract(profileId))
			};
			return base.SetAndClearScalar<ProfileContract>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x00025FBC File Offset: 0x000243BC
		public void SetContractInfo(ulong profileId, uint rotationId, ulong profileItemId, string contractName, uint currentProgress, uint totalProgress, TimeSpan localTimeToUtcTimestamp)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].contract,
				set_func = (() => this.m_contractSystem.SetContractInfo(profileId, rotationId, profileItemId, contractName, currentProgress, totalProgress, localTimeToUtcTimestamp))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x04000552 RID: 1362
		private IContractSystem m_contractSystem;
	}
}
