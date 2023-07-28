using System;
using System.Reflection;
using MasterServer.DAL.RatingSystem;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000045 RID: 69
	internal class RatingGameBanSystemClient : DALCacheProxy<IDALService>, IRatingGameBanSystemClient
	{
		// Token: 0x06000119 RID: 281 RVA: 0x00009688 File Offset: 0x00007A88
		public void BanRatingGameForPlayer(ulong profileId, TimeSpan unbanTime)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].rating_room_bans,
				set_func = (() => this.m_ratingGameBanSystem.BanRatingGameForPlayer(profileId, unbanTime))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x0600011A RID: 282 RVA: 0x000096F4 File Offset: 0x00007AF4
		public void UnbanRatingGameForPlayer(ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].rating_room_bans,
				set_func = (() => this.m_ratingGameBanSystem.UnbanRatingGameForPlayer(profileId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000975C File Offset: 0x00007B5C
		public RatingGamePlayerBanInfo GetPlayerBanInfo(ulong profileId)
		{
			DALCacheProxy<IDALService>.Options<RatingGamePlayerBanInfo> options = new DALCacheProxy<IDALService>.Options<RatingGamePlayerBanInfo>
			{
				cache_domain = cache_domains.profile[profileId].rating_room_bans,
				get_data = (() => this.m_ratingGameBanSystem.GetPlayerBanInfo(profileId))
			};
			return base.GetData<RatingGamePlayerBanInfo>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x000097C1 File Offset: 0x00007BC1
		internal void Reset(IRatingGameBanSystem ratingGameBanSystem)
		{
			this.m_ratingGameBanSystem = ratingGameBanSystem;
		}

		// Token: 0x0400008B RID: 139
		private IRatingGameBanSystem m_ratingGameBanSystem;
	}
}
