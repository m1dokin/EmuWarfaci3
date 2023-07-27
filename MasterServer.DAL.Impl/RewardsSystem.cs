using System;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000021 RID: 33
	internal class RewardsSystem : IRewardsSystem
	{
		// Token: 0x06000174 RID: 372 RVA: 0x0000E27E File Offset: 0x0000C47E
		public RewardsSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000E298 File Offset: 0x0000C498
		public DALResultMulti<SSponsorPoints> GetSponsorPoints(ulong profileId)
		{
			CacheProxy.Options<SSponsorPoints> options = new CacheProxy.Options<SSponsorPoints>
			{
				db_serializer = this.m_sponsorPointsSerializer
			};
			options.query("CALL GetProfileSponsorPoints(?pid)", new object[]
			{
				"?pid",
				profileId
			});
			return this.m_dal.CacheProxy.GetStream<SSponsorPoints>(options);
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000E2EC File Offset: 0x0000C4EC
		public DALResultVoid SetSponsorPoints(ulong profile_id, uint sponsor_id, ulong sponsor_points)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL SetSponsorPoints(?pid, ?spid, ?sppts)", new object[]
			{
				"?pid",
				profile_id,
				"?spid",
				sponsor_id,
				"?sppts",
				sponsor_points
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0000E354 File Offset: 0x0000C554
		public DALResult<bool> SetSponsorInfo(ulong profile_id, uint sponsor_id, ulong old_spPts, SRankInfo new_sp)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT SetSponsorInfo(?pid, ?spid, ?oldSpPts, ?sppts, ?spStage, ?spStageStart, ?spNextStageStart)", new object[]
			{
				"?pid",
				profile_id,
				"?spid",
				sponsor_id,
				"?oldSpPts",
				old_spPts,
				"?sppts",
				new_sp.Points,
				"?spStage",
				new_sp.RankId,
				"?spStageStart",
				new_sp.RankStart,
				"?spNextStageStart",
				new_sp.NextRankStart
			});
			return new DALResult<bool>(this.m_dal.CacheProxy.SetScalar(setOptions).ToString() == "1", setOptions.stats);
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000E438 File Offset: 0x0000C638
		public DALResultVoid SetNextUnlockItem(ulong profile_id, uint sponsor_id, ulong next_unlock_item_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL SetNextUnlockItem(?pid, ?spid, ?nuitd)", new object[]
			{
				"?pid",
				profile_id,
				"?spid",
				sponsor_id,
				"?nuitd",
				next_unlock_item_id
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x04000072 RID: 114
		private DAL m_dal;

		// Token: 0x04000073 RID: 115
		private SponsorPointsSerializer m_sponsorPointsSerializer = new SponsorPointsSerializer();
	}
}
