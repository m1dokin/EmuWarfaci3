using System;
using System.Reflection;
using MasterServer.DAL;
using MasterServer.GameLogic.SkillSystem;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000214 RID: 532
	internal class SkillSystemClient : DALCacheProxy<IDALService>, ISkillSystemClient
	{
		// Token: 0x06000B8C RID: 2956 RVA: 0x0002B94D File Offset: 0x00029D4D
		internal void Reset(ISkillSystem skillSystem)
		{
			this.m_skillSystem = skillSystem;
		}

		// Token: 0x06000B8D RID: 2957 RVA: 0x0002B958 File Offset: 0x00029D58
		public SkillInfo GetSkill(ulong profileId, SkillType skillType)
		{
			DALCacheProxy<IDALService>.Options<SkillInfo> options = new DALCacheProxy<IDALService>.Options<SkillInfo>
			{
				cache_domain = cache_domains.profile[profileId].skill[skillType],
				get_data = (() => this.m_skillSystem.GetSkill(profileId, skillType.ToString().ToLowerInvariant()))
			};
			return base.GetData<SkillInfo>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B8E RID: 2958 RVA: 0x0002B9D4 File Offset: 0x00029DD4
		public void DeleteSkill(ulong profileId, SkillType skillType)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].skill[skillType],
				set_func = (() => this.m_skillSystem.DeleteSkill(profileId, skillType.ToString().ToLowerInvariant()))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B8F RID: 2959 RVA: 0x0002BA50 File Offset: 0x00029E50
		public SkillInfo AddSkill(ulong profileId, SkillType skillType, double value, double curveCoef)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<SkillInfo> options = new DALCacheProxy<IDALService>.SetOptionsScalar<SkillInfo>
			{
				cache_domain = cache_domains.profile[profileId].skill[skillType],
				query_retry = base.DAL.Config.QueryRetry,
				set_func = (() => this.m_skillSystem.AddSkill(profileId, skillType.ToString().ToLowerInvariant(), value, curveCoef))
			};
			return base.SetAndStore<SkillInfo>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x04000563 RID: 1379
		private ISkillSystem m_skillSystem;
	}
}
