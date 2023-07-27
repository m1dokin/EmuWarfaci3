using System;
using MasterServer.DAL.RatingSystem;

namespace MasterServer.DAL.Impl
{
	// Token: 0x0200001F RID: 31
	internal class RatingGameBanSystem : IRatingGameBanSystem
	{
		// Token: 0x0600016A RID: 362 RVA: 0x0000DCF7 File Offset: 0x0000BEF7
		internal RatingGameBanSystem(DAL dal)
		{
			this.m_dal = dal;
			this.m_serializer = new RatingGamePlayerBanInfoSerializer();
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0000DD14 File Offset: 0x0000BF14
		public DALResultVoid BanRatingGameForPlayer(ulong profileId, TimeSpan banTimeout)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL BanPlayerForRatingGames(?pid, ?unban_time)", new object[]
			{
				"?pid",
				profileId,
				"?unban_time",
				(uint)banTimeout.TotalSeconds
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000DD74 File Offset: 0x0000BF74
		public DALResultVoid UnbanRatingGameForPlayer(ulong profileId)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL UnbanPlayerForRatingGames(?pid)", new object[]
			{
				"?pid",
				profileId
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000DDBC File Offset: 0x0000BFBC
		public DALResult<RatingGamePlayerBanInfo> GetPlayerBanInfo(ulong profileId)
		{
			CacheProxy.Options<RatingGamePlayerBanInfo> options = new CacheProxy.Options<RatingGamePlayerBanInfo>
			{
				db_serializer = this.m_serializer
			};
			options.query("CALL GetPlayerRatingUnbanTime(?pid)", new object[]
			{
				"?pid",
				profileId
			});
			return this.m_dal.CacheProxy.Get<RatingGamePlayerBanInfo>(options);
		}

		// Token: 0x0400006A RID: 106
		private readonly DAL m_dal;

		// Token: 0x0400006B RID: 107
		private readonly RatingGamePlayerBanInfoSerializer m_serializer;
	}
}
