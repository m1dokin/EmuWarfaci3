using System;
using MasterServer.Database;
using Util.Common;

namespace MasterServer.DAL.Impl
{
	// Token: 0x0200001E RID: 30
	internal class ProfileSystem : IProfileSystem
	{
		// Token: 0x06000150 RID: 336 RVA: 0x0000D2AE File Offset: 0x0000B4AE
		public ProfileSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x0000D2EC File Offset: 0x0000B4EC
		public DALResultMulti<SAuthProfile> GetUserProfiles(ulong user_id)
		{
			CacheProxy.Options<SAuthProfile> options = new CacheProxy.Options<SAuthProfile>
			{
				db_serializer = this.m_authProfileSerializer
			};
			options.query("CALL GetUserProfiles(?uid)", new object[]
			{
				"?uid",
				user_id
			});
			return this.m_dal.CacheProxy.GetStream<SAuthProfile>(options);
		}

		// Token: 0x06000152 RID: 338 RVA: 0x0000D340 File Offset: 0x0000B540
		public DALResultVoid CreateProfile(ulong profile_id, ulong user_id, string nickname)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				db_transaction = true
			};
			setOptions.query("CALL CreateProfile(?pid, ?uid, ?nick)", new object[]
			{
				"?pid",
				profile_id,
				"?uid",
				user_id,
				"?nick",
				nickname
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000153 RID: 339 RVA: 0x0000D3AC File Offset: 0x0000B5AC
		public DALResultVoid DeleteProfile(ulong user_id, ulong profile_id, string nickname)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL DeleteProfile(?pid)", new object[]
			{
				"?pid",
				profile_id
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x0000D3F4 File Offset: 0x0000B5F4
		public DALResult<ulong> CreateProfile(ulong user_id, string nickname, string head)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				db_transaction = true
			};
			setOptions.query("SELECT NewProfile(?uid, ?nick, ?head)", new object[]
			{
				"?uid",
				user_id,
				"?nick",
				nickname,
				"?head",
				head
			});
			ulong val = ulong.Parse(this.m_dal.CacheProxy.SetScalar(setOptions).ToString());
			return new DALResult<ulong>(val, setOptions.stats);
		}

		// Token: 0x06000155 RID: 341 RVA: 0x0000D474 File Offset: 0x0000B674
		public DALResult<ulong> CreateUser(string csb, string nickname, string pwd, string mail)
		{
			DALResult<ulong> result;
			using (CustomConnectionPool customConnectionPool = new CustomConnectionPool())
			{
				customConnectionPool.Init(csb);
				CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
				{
					connection_pool = customConnectionPool
				};
				setOptions.query("SELECT AddUser(?nick, ?pwd, ?mail)", new object[]
				{
					"?nick",
					nickname,
					"?pwd",
					pwd,
					"?mail",
					mail
				});
				ulong val = ulong.Parse(this.m_dal.CacheProxy.SetScalar(setOptions).ToString());
				result = new DALResult<ulong>(val, setOptions.stats);
			}
			return result;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0000D524 File Offset: 0x0000B724
		public DALResultVoid SetProfileCurClass(ulong profile_id, uint curClass)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL UpdateProfileCurClass(?pid, ?class)", new object[]
			{
				"?pid",
				profile_id,
				"?class",
				curClass
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000157 RID: 343 RVA: 0x0000D57C File Offset: 0x0000B77C
		public DALResult<bool> SetProfileRankInfo(ulong profile_id, ulong old_experience, SRankInfo new_rank_info)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT SetProfileRankInfo(?pid, ?experience, ?old_exp, ?rankId, ?rankStart, ?nextRankStart)", new object[]
			{
				"?pid",
				profile_id,
				"?old_exp",
				old_experience,
				"?experience",
				new_rank_info.Points,
				"?rankId",
				new_rank_info.RankId,
				"?rankStart",
				new_rank_info.RankStart,
				"?nextRankStart",
				new_rank_info.NextRankStart
			});
			return new DALResult<bool>(this.m_dal.CacheProxy.SetScalar(setOptions).ToString() == "1", setOptions.stats);
		}

		// Token: 0x06000158 RID: 344 RVA: 0x0000D650 File Offset: 0x0000B850
		public DALResult<SProfileInfo> GetProfileInfo(ulong profileId)
		{
			CacheProxy.Options<SProfileInfo> options = new CacheProxy.Options<SProfileInfo>
			{
				db_serializer = this.m_profileInfoSerializer
			};
			options.query("CALL GetProfileInfo(?pid)", new object[]
			{
				"?pid",
				profileId
			});
			return this.m_dal.CacheProxy.Get<SProfileInfo>(options);
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000D6A4 File Offset: 0x0000B8A4
		public DALResultVoid SetProfileBanner(ulong profile_id, SBannerInfo banner)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL SetProfileBanner(?pid, ?badge, ?mark, ?stripe)", new object[]
			{
				"?pid",
				profile_id,
				"?badge",
				banner.Badge,
				"?mark",
				banner.Mark,
				"?stripe",
				banner.Stripe
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000D730 File Offset: 0x0000B930
		public DALResult<SProfileInfo> GetProfileByNickname(string nickname)
		{
			CacheProxy.Options<SProfileInfo> options = new CacheProxy.Options<SProfileInfo>
			{
				db_serializer = this.m_profileInfoSerializer
			};
			options.query("CALL GetProfileByNickname(?nick)", new object[]
			{
				"?nick",
				nickname
			});
			return this.m_dal.CacheProxy.Get<SProfileInfo>(options);
		}

		// Token: 0x0600015B RID: 347 RVA: 0x0000D780 File Offset: 0x0000B980
		public DALResult<ulong> GetLastSeenDate(ulong profile_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT GetLastSeenDate(?pid)", new object[]
			{
				"?pid",
				profile_id
			});
			ulong val = ulong.Parse(this.m_dal.CacheProxy.SetScalar(setOptions).ToString());
			return new DALResult<ulong>(val, setOptions.stats);
		}

		// Token: 0x0600015C RID: 348 RVA: 0x0000D7E0 File Offset: 0x0000B9E0
		public DALResultVoid UpdateLastSeenDate(ulong profile_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL UpdateLastSeenDate(?pid)", new object[]
			{
				"?pid",
				profile_id
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x0600015D RID: 349 RVA: 0x0000D828 File Offset: 0x0000BA28
		public DALResultVoid UpdateLastSeenDate(ulong profile_id, DateTime lastSeenDate)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL SetLastSeenDate(?pid, ?date)", new object[]
			{
				"?pid",
				profile_id,
				"?date",
				TimeUtils.LocalTimeToUTCTimestamp(lastSeenDate)
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000D884 File Offset: 0x0000BA84
		public DALResultVoid UpdateMuteTime(ulong profile_id, DateTime mute_time)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL UpdateMuteTime(?pid, ?date)", new object[]
			{
				"?pid",
				profile_id,
				"?date",
				TimeUtils.LocalTimeToUTCTimestamp(mute_time)
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x0600015F RID: 351 RVA: 0x0000D8E0 File Offset: 0x0000BAE0
		public DALResultVoid UpdateBanTime(ulong profile_id, DateTime ban_time)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL UpdateBanTime(?pid, ?date)", new object[]
			{
				"?pid",
				profile_id,
				"?date",
				TimeUtils.LocalTimeToUTCTimestamp(ban_time)
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000D93C File Offset: 0x0000BB3C
		public DALResultVoid UpdateProfileHead(ulong profileId, string head)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL UpdateProfileHead(?pid, ?head)", new object[]
			{
				"?pid",
				profileId,
				"?head",
				head
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000D990 File Offset: 0x0000BB90
		public DALResultMulti<SPersistentSettings> GetPersistentSettings(ulong profile_id)
		{
			CacheProxy.Options<SPersistentSettings> options = new CacheProxy.Options<SPersistentSettings>
			{
				db_serializer = this.m_persistentSettingsSerializer
			};
			options.query("CALL GetPersistentSettings(?pid)", new object[]
			{
				"?pid",
				profile_id
			});
			return this.m_dal.CacheProxy.GetStream<SPersistentSettings>(options);
		}

		// Token: 0x06000162 RID: 354 RVA: 0x0000D9E4 File Offset: 0x0000BBE4
		public DALResultVoid SetPersistentSettings(ulong profile_id, string group, string value)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL SetPersistentSettings(?pid, ?group, ?val)", new object[]
			{
				"?pid",
				profile_id,
				"?group",
				group,
				"?val",
				value
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000163 RID: 355 RVA: 0x0000DA44 File Offset: 0x0000BC44
		public DALResultVoid ClearPersistentSettings(ulong profile_id, string group)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL ClearPersistentSettings(?pid, ?group)", new object[]
			{
				"?pid",
				profile_id,
				"?group",
				group
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000164 RID: 356 RVA: 0x0000DA98 File Offset: 0x0000BC98
		public DALResultVoid ClearPersistentSettingsFull(ulong profile_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL ClearPersistentSettingsFull(?pid)", new object[]
			{
				"?pid",
				profile_id
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000165 RID: 357 RVA: 0x0000DAE0 File Offset: 0x0000BCE0
		public DALResult<TimeSpan> UpdateTimeToRank(ulong profile_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT UpdateTimeToRank(?pid)", new object[]
			{
				"?pid",
				profile_id
			});
			int seconds = int.Parse(this.m_dal.CacheProxy.SetScalar(setOptions).ToString());
			return new DALResult<TimeSpan>(new TimeSpan(0, 0, seconds), setOptions.stats);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x0000DB44 File Offset: 0x0000BD44
		public DALResult<bool> UpdateProfileNickname(ulong profileId, string newNickname)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT UpdateProfileNickname(?pid, ?newNickname)", new object[]
			{
				"?pid",
				profileId,
				"?newNickname",
				newNickname
			});
			return new DALResult<bool>(this.m_dal.CacheProxy.SetScalar(setOptions).ToString() == "1", setOptions.stats);
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0000DBB0 File Offset: 0x0000BDB0
		public DALResultMulti<SFriend> GetFriends(ulong profile_id)
		{
			CacheProxy.Options<SFriend> options = new CacheProxy.Options<SFriend>
			{
				db_serializer = this.m_friendSerializer
			};
			options.query("CALL GetFriends(?pid)", new object[]
			{
				"?pid",
				profile_id
			});
			return this.m_dal.CacheProxy.GetStream<SFriend>(options);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0000DC04 File Offset: 0x0000BE04
		public DALResult<EAddMemberResult> AddFriend(ulong profile_id_1, ulong profile_id_2, uint limit)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT AddFriend(?pid1, ?pid2, ?limit)", new object[]
			{
				"?pid1",
				profile_id_1,
				"?pid2",
				profile_id_2,
				"?limit",
				limit
			});
			setOptions.db_transaction = true;
			string value = this.m_dal.CacheProxy.SetScalar(setOptions).ToString();
			EAddMemberResult val = (EAddMemberResult)Enum.Parse(EAddMemberResult.Succeed.GetType(), value);
			return new DALResult<EAddMemberResult>(val, setOptions.stats);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000DCA0 File Offset: 0x0000BEA0
		public DALResultVoid RemoveFriend(ulong profile_id_1, ulong profile_id_2)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL RemoveFriend(?pid1, ?pid2)", new object[]
			{
				"?pid1",
				profile_id_1,
				"?pid2",
				profile_id_2
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x04000065 RID: 101
		private DAL m_dal;

		// Token: 0x04000066 RID: 102
		private SAuthProfileSerializer m_authProfileSerializer = new SAuthProfileSerializer();

		// Token: 0x04000067 RID: 103
		private SProfileInfoSerializer m_profileInfoSerializer = new SProfileInfoSerializer();

		// Token: 0x04000068 RID: 104
		private PersistentSettingsSerializer m_persistentSettingsSerializer = new PersistentSettingsSerializer();

		// Token: 0x04000069 RID: 105
		private SFriendSerializer m_friendSerializer = new SFriendSerializer();
	}
}
