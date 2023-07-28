using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x0200020E RID: 526
	internal class ProfileSystemClient : DALCacheProxy<IDALService>, IProfileSystemClient
	{
		// Token: 0x06000B51 RID: 2897 RVA: 0x0002A3F9 File Offset: 0x000287F9
		internal void Reset(IProfileSystem profileSystem)
		{
			this.m_profileSystem = profileSystem;
		}

		// Token: 0x06000B52 RID: 2898 RVA: 0x0002A404 File Offset: 0x00028804
		public IEnumerable<SAuthProfile> GetUserProfiles(ulong userId)
		{
			DALCacheProxy<IDALService>.Options<SAuthProfile> options = new DALCacheProxy<IDALService>.Options<SAuthProfile>
			{
				cache_domain = cache_domains.user[userId].profiles,
				get_data_stream = (() => this.m_profileSystem.GetUserProfiles(userId))
			};
			return base.GetDataStream<SAuthProfile>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B53 RID: 2899 RVA: 0x0002A46C File Offset: 0x0002886C
		public void CreateProfile(ulong profileId, ulong userId, string nickname)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.profile[profileId],
					cache_domains.user[userId].profiles
				},
				set_func = (() => this.m_profileSystem.CreateProfile(profileId, userId, nickname))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x0002A500 File Offset: 0x00028900
		public ulong CreateProfile(ulong userId, string nickname, string head)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ulong> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ulong>
			{
				cache_domain = cache_domains.user[userId].profiles,
				set_func = (() => this.m_profileSystem.CreateProfile(userId, nickname, head))
			};
			return base.SetAndClearScalar<ulong>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B55 RID: 2901 RVA: 0x0002A574 File Offset: 0x00028974
		public void DeleteProfile(ulong userId, ulong profileId, string nickname)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.profile[profileId],
					cache_domains.user[userId].profiles,
					cache_domains.profile_mapping[nickname]
				},
				set_func = (() => this.m_profileSystem.DeleteProfile(userId, profileId, nickname))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B56 RID: 2902 RVA: 0x0002A620 File Offset: 0x00028A20
		public ulong CreateUser(string csb, string nickname, string pwd, string mail)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ulong> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ulong>
			{
				set_func = (() => this.m_profileSystem.CreateUser(csb, nickname, pwd, mail))
			};
			return base.SetAndClearScalar<ulong>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B57 RID: 2903 RVA: 0x0002A680 File Offset: 0x00028A80
		public void FlushProfileCache(ulong userId, ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptionsBase options = new DALCacheProxy<IDALService>.SetOptionsBase
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.user[userId],
					cache_domains.profile[profileId]
				}
			};
			base.ClearCache(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B58 RID: 2904 RVA: 0x0002A6D0 File Offset: 0x00028AD0
		public void FlushCatalogCache(ulong userId)
		{
			DALCacheProxy<IDALService>.SetOptionsBase options = new DALCacheProxy<IDALService>.SetOptionsBase
			{
				cache_domain = cache_domains.customer[userId]
			};
			base.ClearCache(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x0002A708 File Offset: 0x00028B08
		public void SetProfileCurClass(ulong profileId, uint curClass)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].info,
				set_func = (() => this.m_profileSystem.SetProfileCurClass(profileId, curClass))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B5A RID: 2906 RVA: 0x0002A774 File Offset: 0x00028B74
		public bool SetProfileRankInfo(ulong profileId, ulong old_experience, SRankInfo info)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<bool> options = new DALCacheProxy<IDALService>.SetOptionsScalar<bool>
			{
				cache_domain = cache_domains.profile[profileId].info,
				set_func = (() => this.m_profileSystem.SetProfileRankInfo(profileId, old_experience, info))
			};
			return base.SetAndClearScalar<bool>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B5B RID: 2907 RVA: 0x0002A7E8 File Offset: 0x00028BE8
		public SProfileInfo GetProfileInfo(ulong profileId)
		{
			DALCacheProxy<IDALService>.Options<SProfileInfo> options = new DALCacheProxy<IDALService>.Options<SProfileInfo>
			{
				cache_domain = cache_domains.profile[profileId].info,
				get_data = (() => this.m_profileSystem.GetProfileInfo(profileId))
			};
			return base.GetData<SProfileInfo>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B5C RID: 2908 RVA: 0x0002A850 File Offset: 0x00028C50
		public ulong GetProfileIDByNickname(string nickname)
		{
			return this.GetProfileByNickname(nickname).Id;
		}

		// Token: 0x06000B5D RID: 2909 RVA: 0x0002A86C File Offset: 0x00028C6C
		public SProfileInfo GetProfileByNickname(string nickname)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<SProfileInfo> options = new DALCacheProxy<IDALService>.SetOptionsScalar<SProfileInfo>
			{
				cache_domain = cache_domains.profile_mapping[nickname],
				set_func = (() => this.m_profileSystem.GetProfileByNickname(nickname))
			};
			return base.SetAndClearScalar<SProfileInfo>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B5E RID: 2910 RVA: 0x0002A8CC File Offset: 0x00028CCC
		public ulong GetLastSeenDate(ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ulong> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ulong>
			{
				cache_domain = cache_domains.profile[profileId].info,
				set_func = (() => this.m_profileSystem.GetLastSeenDate(profileId))
			};
			return base.SetAndClearScalar<ulong>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B5F RID: 2911 RVA: 0x0002A934 File Offset: 0x00028D34
		public void UpdateLastSeenDate(ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].info,
				set_func = (() => this.m_profileSystem.UpdateLastSeenDate(profileId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B60 RID: 2912 RVA: 0x0002A99C File Offset: 0x00028D9C
		public void UpdateLastSeenDate(ulong profileId, DateTime lastSeenDate)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].info,
				set_func = (() => this.m_profileSystem.UpdateLastSeenDate(profileId, lastSeenDate))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x0002AA08 File Offset: 0x00028E08
		public bool UpdateProfileNickname(ulong profileId, string newNickname)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<bool> options = new DALCacheProxy<IDALService>.SetOptionsScalar<bool>
			{
				cache_domain = cache_domains.profile[profileId].info,
				set_func = (() => this.m_profileSystem.UpdateProfileNickname(profileId, newNickname))
			};
			return base.SetAndClearScalar<bool>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B62 RID: 2914 RVA: 0x0002AA74 File Offset: 0x00028E74
		public void UpdateProfileHead(ulong profileId, string head)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].info,
				set_func = (() => this.m_profileSystem.UpdateProfileHead(profileId, head))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B63 RID: 2915 RVA: 0x0002AAE0 File Offset: 0x00028EE0
		public void UpdateMuteTime(ulong profileId, DateTime muteTime)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].info,
				set_func = (() => this.m_profileSystem.UpdateMuteTime(profileId, muteTime))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B64 RID: 2916 RVA: 0x0002AB4C File Offset: 0x00028F4C
		public void UpdateBanTime(ulong profileId, DateTime banTime)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].info,
				set_func = (() => this.m_profileSystem.UpdateBanTime(profileId, banTime))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B65 RID: 2917 RVA: 0x0002ABB8 File Offset: 0x00028FB8
		public void SetProfileBanner(ulong profileId, SBannerInfo banner)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].info,
				set_func = (() => this.m_profileSystem.SetProfileBanner(profileId, banner))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B66 RID: 2918 RVA: 0x0002AC24 File Offset: 0x00029024
		public IEnumerable<SPersistentSettings> GetPersistentSettings(ulong profileId)
		{
			DALCacheProxy<IDALService>.Options<SPersistentSettings> options = new DALCacheProxy<IDALService>.Options<SPersistentSettings>
			{
				cache_domain = cache_domains.profile[profileId].persistent_settings,
				get_data_stream = (() => this.m_profileSystem.GetPersistentSettings(profileId))
			};
			return base.GetDataStream<SPersistentSettings>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B67 RID: 2919 RVA: 0x0002AC8C File Offset: 0x0002908C
		public void SetPersistentSettings(ulong profileId, string group, string value)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].persistent_settings,
				set_func = (() => this.m_profileSystem.SetPersistentSettings(profileId, group, value))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B68 RID: 2920 RVA: 0x0002AD00 File Offset: 0x00029100
		public void ClearPersistentSettings(ulong profileId, string group)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].persistent_settings,
				set_func = (() => this.m_profileSystem.ClearPersistentSettings(profileId, group))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B69 RID: 2921 RVA: 0x0002AD6C File Offset: 0x0002916C
		public void ClearPersistentSettingsFull(ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].persistent_settings,
				set_func = (() => this.m_profileSystem.ClearPersistentSettingsFull(profileId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B6A RID: 2922 RVA: 0x0002ADD4 File Offset: 0x000291D4
		public IEnumerable<SFriend> GetFriends(ulong profileId)
		{
			DALCacheProxy<IDALService>.Options<SFriend> options = new DALCacheProxy<IDALService>.Options<SFriend>
			{
				cache_domain = cache_domains.profile[profileId].friends,
				get_data_stream = (() => this.m_profileSystem.GetFriends(profileId))
			};
			return base.GetDataStream<SFriend>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B6B RID: 2923 RVA: 0x0002AE3C File Offset: 0x0002923C
		public EAddMemberResult AddFriend(ulong profileId1, ulong profileId2, uint limit)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<EAddMemberResult> options = new DALCacheProxy<IDALService>.SetOptionsScalar<EAddMemberResult>
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.profile[profileId1].friends,
					cache_domains.profile[profileId2].friends
				},
				set_func = (() => this.m_profileSystem.AddFriend(profileId1, profileId2, limit))
			};
			return base.SetAndClearScalar<EAddMemberResult>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B6C RID: 2924 RVA: 0x0002AED4 File Offset: 0x000292D4
		public void RemoveFriend(ulong profileId1, ulong profileId2)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.profile[profileId1].friends,
					cache_domains.profile[profileId2].friends
				},
				set_func = (() => this.m_profileSystem.RemoveFriend(profileId1, profileId2))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B6D RID: 2925 RVA: 0x0002AF68 File Offset: 0x00029368
		public TimeSpan UpdateTimeToRank(ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<TimeSpan> options = new DALCacheProxy<IDALService>.SetOptionsScalar<TimeSpan>
			{
				set_func = (() => this.m_profileSystem.UpdateTimeToRank(profileId))
			};
			return base.SetAndClearScalar<TimeSpan>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B6E RID: 2926 RVA: 0x0002AFB0 File Offset: 0x000293B0
		public void FlushProfileFriendsCache(ulong commonFriendPID)
		{
			IEnumerable<ulong> source = from f in this.GetFriends(commonFriendPID)
			select f.ProfileID;
			DALCacheProxy<IDALService>.SetOptionsBase setOptionsBase = new DALCacheProxy<IDALService>.SetOptionsBase();
			setOptionsBase.cache_domains = from pid in source
			select cache_domains.profile[pid].friends;
			DALCacheProxy<IDALService>.SetOptionsBase options = setOptionsBase;
			base.ClearCache(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x0400055E RID: 1374
		private IProfileSystem m_profileSystem;
	}
}
