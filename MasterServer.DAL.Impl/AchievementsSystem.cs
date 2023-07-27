using System;
using MasterServer.Database;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000003 RID: 3
	internal class AchievementsSystem : IAchievementsSystem
	{
		// Token: 0x0600000D RID: 13 RVA: 0x000024CF File Offset: 0x000006CF
		public AchievementsSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000024F4 File Offset: 0x000006F4
		public DALResultMulti<AchievementInfo> GetProfileAchievements(ulong profile_id)
		{
			CacheProxy.Options<AchievementInfo> options = new CacheProxy.Options<AchievementInfo>
			{
				db_mode = DBAccessMode.Slave,
				db_serializer = this.m_achievementInfoSerializer
			};
			options.query("CALL GetProfileAchievements(?pid)", new object[]
			{
				"?pid",
				profile_id
			});
			return this.m_dal.CacheProxy.GetStream<AchievementInfo>(options);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002550 File Offset: 0x00000750
		public DALResult<SAchievementUpdate> SetAchievementProgress(ulong profile_id, uint ach_id, int new_progress, uint max_progress, ulong completion_time)
		{
			CacheProxy.Options<SAchievementUpdate> options = new CacheProxy.Options<SAchievementUpdate>
			{
				db_serializer = this.m_achievementUpdateSerializer
			};
			options.query("CALL SetAchievementProgress(?pid, ?achid, ?nprg, ?mprg, ?cmpt)", new object[]
			{
				"?pid",
				profile_id,
				"?achid",
				ach_id,
				"?nprg",
				new_progress,
				"?mprg",
				max_progress,
				"?cmpt",
				completion_time
			});
			return this.m_dal.CacheProxy.Get<SAchievementUpdate>(options);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000025EC File Offset: 0x000007EC
		public DALResult<SAchievementUpdate> UpdateAchievementProgress(ulong profile_id, uint ach_id, int incr_progress, uint max_progress)
		{
			CacheProxy.Options<SAchievementUpdate> options = new CacheProxy.Options<SAchievementUpdate>
			{
				db_serializer = this.m_achievementUpdateSerializer
			};
			options.query("CALL UpdateAchievementProgress(?pid, ?achid, ?iprg, ?mprg)", new object[]
			{
				"?pid",
				profile_id,
				"?achid",
				ach_id,
				"?iprg",
				incr_progress,
				"?mprg",
				max_progress
			});
			return this.m_dal.CacheProxy.Get<SAchievementUpdate>(options);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002674 File Offset: 0x00000874
		public DALResultVoid DeleteProfileAchievements(ulong profile_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL DeleteProfileAchievements(?pid)", new object[]
			{
				"?pid",
				profile_id
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000026BC File Offset: 0x000008BC
		public DALResultVoid DeleteProfileAchievement(ulong profile_id, uint achievment_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL DeleteProfileAchievement(?pid, ?ach)", new object[]
			{
				"?pid",
				profile_id,
				"?ach",
				achievment_id
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x04000005 RID: 5
		private readonly DAL m_dal;

		// Token: 0x04000006 RID: 6
		private readonly AchievementInfoSerializer m_achievementInfoSerializer = new AchievementInfoSerializer();

		// Token: 0x04000007 RID: 7
		private readonly AchievementUpdateSerializer m_achievementUpdateSerializer = new AchievementUpdateSerializer();
	}
}
