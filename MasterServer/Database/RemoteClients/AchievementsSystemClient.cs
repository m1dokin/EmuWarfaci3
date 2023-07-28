using System;
using System.Collections.Generic;
using System.Reflection;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001F0 RID: 496
	internal class AchievementsSystemClient : DALCacheProxy<IDALService>, IAchievementsSystemClient
	{
		// Token: 0x060009AC RID: 2476 RVA: 0x000243A9 File Offset: 0x000227A9
		internal void Reset(IAchievementsSystem achievementsSystem)
		{
			this.m_achievementsSystem = achievementsSystem;
		}

		// Token: 0x060009AD RID: 2477 RVA: 0x000243B4 File Offset: 0x000227B4
		public IEnumerable<AchievementInfo> GetProfileAchievements(ulong profileId)
		{
			DALCacheProxy<IDALService>.Options<AchievementInfo> options = new DALCacheProxy<IDALService>.Options<AchievementInfo>
			{
				cache_domain = cache_domains.profile[profileId].achievements,
				get_data_stream = (() => this.m_achievementsSystem.GetProfileAchievements(profileId))
			};
			return base.GetDataStream<AchievementInfo>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009AE RID: 2478 RVA: 0x0002441C File Offset: 0x0002281C
		public SAchievementUpdate SetAchievementProgress(ulong profile_id, uint ach_id, int new_progress, uint max_progress, ulong completion_time)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<SAchievementUpdate> options = new DALCacheProxy<IDALService>.SetOptionsScalar<SAchievementUpdate>
			{
				cache_domain = cache_domains.profile[profile_id].achievements,
				set_func = (() => this.m_achievementsSystem.SetAchievementProgress(profile_id, ach_id, new_progress, max_progress, completion_time))
			};
			return base.SetAndClearScalar<SAchievementUpdate>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009AF RID: 2479 RVA: 0x000244A0 File Offset: 0x000228A0
		public SAchievementUpdate UpdateAchievementProgress(ulong profile_id, uint ach_id, int incr_progress, uint max_progress)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<SAchievementUpdate> options = new DALCacheProxy<IDALService>.SetOptionsScalar<SAchievementUpdate>
			{
				cache_domain = cache_domains.profile[profile_id].achievements,
				set_func = (() => this.m_achievementsSystem.UpdateAchievementProgress(profile_id, ach_id, incr_progress, max_progress))
			};
			return base.SetAndClearScalar<SAchievementUpdate>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009B0 RID: 2480 RVA: 0x0002451C File Offset: 0x0002291C
		public void DeleteProfileAchievements(ulong profile_id)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profile_id].achievements,
				set_func = (() => this.m_achievementsSystem.DeleteProfileAchievements(profile_id))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009B1 RID: 2481 RVA: 0x00024584 File Offset: 0x00022984
		public void DeleteProfileAchievement(ulong profile_id, uint achievment_id)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profile_id].achievements,
				set_func = (() => this.m_achievementsSystem.DeleteProfileAchievement(profile_id, achievment_id))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x0400054C RID: 1356
		private static readonly TimeSpan LEADERBOARD_EXPIRATION_TIME = new TimeSpan(0, 10, 0);

		// Token: 0x0400054D RID: 1357
		private IAchievementsSystem m_achievementsSystem;
	}
}
