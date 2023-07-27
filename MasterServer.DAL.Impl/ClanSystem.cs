using System;

namespace MasterServer.DAL.Impl
{
	// Token: 0x0200000B RID: 11
	internal class ClanSystem : IClanSystem
	{
		// Token: 0x06000031 RID: 49 RVA: 0x00003296 File Offset: 0x00001496
		public ClanSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000032C8 File Offset: 0x000014C8
		public DALResultMulti<ClanMember> GetClanMembers(ulong clan_id)
		{
			CacheProxy.Options<ClanMember> options = new CacheProxy.Options<ClanMember>
			{
				db_serializer = this.m_clanMemberSerializer
			};
			options.query("CALL GetClanMembers(?cid)", new object[]
			{
				"?cid",
				clan_id
			});
			return this.m_dal.CacheProxy.GetStream<ClanMember>(options);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x0000331C File Offset: 0x0000151C
		public DALResult<EAddMemberResult> AddClanMember(ulong clan_id, ulong profile_id, uint limit)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT AddClanMember(?cid, ?pid, ?limit)", new object[]
			{
				"?cid",
				clan_id,
				"?pid",
				profile_id,
				"?limit",
				limit
			});
			setOptions.db_transaction = true;
			string value = this.m_dal.CacheProxy.SetScalar(setOptions).ToString();
			EAddMemberResult val = (EAddMemberResult)Enum.Parse(EAddMemberResult.Succeed.GetType(), value);
			return new DALResult<EAddMemberResult>(val, setOptions.stats);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000033B8 File Offset: 0x000015B8
		public DALResult<ulong> RemoveClanMember(ulong clan_id, ulong profile_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT RemoveClanMember(?cid, ?pid)", new object[]
			{
				"?cid",
				clan_id,
				"?pid",
				profile_id
			});
			setOptions.db_transaction = true;
			ulong val = (ulong)this.m_dal.CacheProxy.SetScalar(setOptions);
			return new DALResult<ulong>(val, setOptions.stats);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003428 File Offset: 0x00001628
		public DALResultVoid KickClanMember(ulong clan_id, ulong profile_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL KickClanMember(?cid, ?pid)", new object[]
			{
				"?cid",
				clan_id,
				"?pid",
				profile_id
			});
			setOptions.db_transaction = true;
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003488 File Offset: 0x00001688
		public DALResultMulti<SClanKick> GetClanKicks(ulong profile_id)
		{
			CacheProxy.Options<SClanKick> options = new CacheProxy.Options<SClanKick>
			{
				db_serializer = this.m_clanKickSerializer
			};
			options.query("CALL GetClanKicks(?pid)", new object[]
			{
				"?pid",
				profile_id
			});
			return this.m_dal.CacheProxy.GetStream<SClanKick>(options);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000034DC File Offset: 0x000016DC
		public DALResult<ClanInfo> GetClanInfo(ulong clan_id)
		{
			CacheProxy.Options<ClanInfo> options = new CacheProxy.Options<ClanInfo>
			{
				db_serializer = this.m_clanInfoSerializer
			};
			options.query("CALL GetClanInfo(?cid)", new object[]
			{
				"?cid",
				clan_id
			});
			return this.m_dal.CacheProxy.Get<ClanInfo>(options);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003530 File Offset: 0x00001730
		public DALResult<ulong> GetClanIdByName(string clanName)
		{
			CacheProxy.Options<ulong> options = new CacheProxy.Options<ulong>
			{
				db_serializer = new UInt64FieldSerializer("clan_id")
			};
			options.query("CALL GetClanIdByName(?cname)", new object[]
			{
				"?cname",
				clanName
			});
			return this.m_dal.CacheProxy.Get<ulong>(options);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003584 File Offset: 0x00001784
		public DALResultMulti<ClanInfo> GetClanTop(int limit)
		{
			CacheProxy.Options<ClanInfo> options = new CacheProxy.Options<ClanInfo>
			{
				db_serializer = this.m_clanInfoSerializer
			};
			options.query("CALL GetClanTop(?limit)", new object[]
			{
				"?limit",
				limit
			});
			return this.m_dal.CacheProxy.GetStream<ClanInfo>(options);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000035D8 File Offset: 0x000017D8
		public DALResultMulti<ulong> GetClansForLeaderboardPrediction()
		{
			CacheProxy.Options<ulong> options = new CacheProxy.Options<ulong>
			{
				db_serializer = new UInt64FieldSerializer("clan_points_count")
			};
			options.query("CALL GetClansForLeaderboardPrediction()", new object[0]);
			return this.m_dal.CacheProxy.GetStream<ulong>(options);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003620 File Offset: 0x00001820
		public DALResult<ClanMember> GetMemberInfo(ulong profile_id)
		{
			CacheProxy.Options<ClanMember> options = new CacheProxy.Options<ClanMember>
			{
				db_serializer = this.m_clanMemberSerializer
			};
			options.query("CALL GetClanMemberInfo(?pid)", new object[]
			{
				"?pid",
				profile_id
			});
			return this.m_dal.CacheProxy.Get<ClanMember>(options);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003674 File Offset: 0x00001874
		public DALResult<ulong> CreateClan(ulong master_pid, string clan_name, string description)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT CreateClan(?pid, ?clan_name, ?descr)", new object[]
			{
				"?pid",
				master_pid,
				"?clan_name",
				clan_name,
				"?descr",
				description
			});
			setOptions.db_transaction = true;
			ulong val = (ulong)this.m_dal.CacheProxy.SetScalar(setOptions);
			return new DALResult<ulong>(val, setOptions.stats);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000036EC File Offset: 0x000018EC
		public DALResultVoid RemoveClan(ulong clan_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.db_transaction = true;
			setOptions.query("CALL RemoveClan(?pid)", new object[]
			{
				"?pid",
				clan_id
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000373C File Offset: 0x0000193C
		public DALResultVoid SetClanInfo(ulong clan_id, string description)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL SetClanInfo(?cid, ?desc)", new object[]
			{
				"?cid",
				clan_id,
				"?desc",
				description
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003790 File Offset: 0x00001990
		public DALResult<ulong> AddClanPoints(ulong clan_id, ulong target_id, ulong clan_points)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT AddClanPoints(?cid, ?pid, ?points)", new object[]
			{
				"?cid",
				clan_id,
				"?pid",
				target_id,
				"?points",
				clan_points
			});
			setOptions.db_transaction = true;
			ulong val = (ulong)this.m_dal.CacheProxy.SetScalar(setOptions);
			return new DALResult<ulong>(val, setOptions.stats);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003814 File Offset: 0x00001A14
		public DALResultVoid SetUserClanRole(ulong clan_id, ulong master_id, ulong profile_id, uint role)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL SetUserClanRole(?cid, ?pid, ?mid, ?role)", new object[]
			{
				"?cid",
				clan_id,
				"?pid",
				profile_id,
				"?mid",
				master_id,
				"?role",
				role
			});
			setOptions.db_transaction = true;
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003898 File Offset: 0x00001A98
		public DALResult<uint> FixupClans()
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT FixupClans()", new object[0]);
			setOptions.db_transaction = true;
			uint val = (uint)this.m_dal.CacheProxy.SetScalar(setOptions);
			return new DALResult<uint>(val, setOptions.stats);
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000038E8 File Offset: 0x00001AE8
		public DALResult<bool> FixupClan(ulong clan_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT FixupClan(?cid)", new object[]
			{
				"?cid",
				clan_id
			});
			setOptions.db_transaction = true;
			int num = (int)this.m_dal.CacheProxy.SetScalar(setOptions);
			return new DALResult<bool>(num != 0, setOptions.stats);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003950 File Offset: 0x00001B50
		public DALResultVoid FixupClansMasters()
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL FixupClansMasters()", new object[0]);
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00003988 File Offset: 0x00001B88
		public DALResultVoid DebugGenerateClans(uint count)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL DebugGenerateClans(?count)", new object[]
			{
				"?count",
				count
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000039D0 File Offset: 0x00001BD0
		public DALResultVoid DebugDeleteAllClans()
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL DebugDeleteAllClans()", new object[0]);
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x04000015 RID: 21
		private DAL m_dal;

		// Token: 0x04000016 RID: 22
		private ClanInfoSerializer m_clanInfoSerializer = new ClanInfoSerializer();

		// Token: 0x04000017 RID: 23
		private ClanMemberSerializer m_clanMemberSerializer = new ClanMemberSerializer();

		// Token: 0x04000018 RID: 24
		private ClanKickSerializer m_clanKickSerializer = new ClanKickSerializer();
	}
}
