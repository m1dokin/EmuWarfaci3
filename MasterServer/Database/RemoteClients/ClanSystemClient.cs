using System;
using System.Collections.Generic;
using System.Reflection;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001F4 RID: 500
	internal class ClanSystemClient : DALCacheProxy<IDALService>, IClanSystemClient
	{
		// Token: 0x060009D9 RID: 2521 RVA: 0x00024948 File Offset: 0x00022D48
		internal void Reset(IClanSystem clanSystem)
		{
			this.m_clanSystem = clanSystem;
		}

		// Token: 0x060009DA RID: 2522 RVA: 0x00024954 File Offset: 0x00022D54
		public ClanMember GetMemberInfo(ulong profileId)
		{
			DALCacheProxy<IDALService>.Options<ClanMember> options = new DALCacheProxy<IDALService>.Options<ClanMember>
			{
				cache_domain = cache_domains.profile[profileId].clan_info,
				get_data = (() => this.m_clanSystem.GetMemberInfo(profileId))
			};
			return base.GetData<ClanMember>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009DB RID: 2523 RVA: 0x000249BC File Offset: 0x00022DBC
		public IEnumerable<SClanKick> GetClanKicks(ulong profileId)
		{
			DALCacheProxy<IDALService>.Options<SClanKick> options = new DALCacheProxy<IDALService>.Options<SClanKick>
			{
				cache_domain = cache_domains.profile[profileId].clan_kicks,
				get_data_stream = (() => this.m_clanSystem.GetClanKicks(profileId))
			};
			return base.GetDataStream<SClanKick>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009DC RID: 2524 RVA: 0x00024A24 File Offset: 0x00022E24
		public ClanInfo GetClanInfo(ulong clanId)
		{
			DALCacheProxy<IDALService>.Options<ClanInfo> options = new DALCacheProxy<IDALService>.Options<ClanInfo>
			{
				cache_domain = cache_domains.clan[clanId].clan_info,
				get_data = (() => this.m_clanSystem.GetClanInfo(clanId))
			};
			return base.GetData<ClanInfo>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009DD RID: 2525 RVA: 0x00024A8C File Offset: 0x00022E8C
		public ClanInfo GetClanInfoByName(string clanName)
		{
			DALCacheProxy<IDALService>.Options<ulong> options = new DALCacheProxy<IDALService>.Options<ulong>
			{
				cache_domain = cache_domains.clan[clanName],
				get_data = (() => this.m_clanSystem.GetClanIdByName(clanName))
			};
			ulong data = base.GetData<ulong>(MethodBase.GetCurrentMethod(), options);
			return this.GetClanInfo(data);
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x00024AF8 File Offset: 0x00022EF8
		public IEnumerable<ClanInfo> GetClanTop(int limit)
		{
			DALCacheProxy<IDALService>.Options<ClanInfo> options = new DALCacheProxy<IDALService>.Options<ClanInfo>
			{
				cache_domain = cache_domains.clan_top,
				get_data_stream = (() => this.m_clanSystem.GetClanTop(limit))
			};
			return base.GetDataStream<ClanInfo>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009DF RID: 2527 RVA: 0x00024B4C File Offset: 0x00022F4C
		public IEnumerable<ulong> GetClansForLeaderboardPrediction()
		{
			DALCacheProxy<IDALService>.Options<ulong> options = new DALCacheProxy<IDALService>.Options<ulong>
			{
				get_data_stream = (() => this.m_clanSystem.GetClansForLeaderboardPrediction())
			};
			return base.GetDataStream<ulong>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x00024B80 File Offset: 0x00022F80
		public ulong CreateClan(ulong profileId, string clanName, string description)
		{
			List<cache_domain> cache_domains = new List<cache_domain>
			{
				cache_domains.profile[profileId].clan_info,
				cache_domains.clan[clanName]
			};
			DALCacheProxy<IDALService>.SetOptionsScalar<ulong> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ulong>
			{
				cache_domains = cache_domains,
				set_func = (() => this.m_clanSystem.CreateClan(profileId, clanName, description))
			};
			ulong num = base.SetAndClearScalar<ulong>(MethodBase.GetCurrentMethod(), options);
			DALCacheProxy<IDALService>.SetOptionsBase options2 = new DALCacheProxy<IDALService>.SetOptionsBase
			{
				cache_domain = cache_domains.clan[num]
			};
			base.ClearCache(MethodBase.GetCurrentMethod(), options2);
			return num;
		}

		// Token: 0x060009E1 RID: 2529 RVA: 0x00024C58 File Offset: 0x00023058
		public void RemoveClan(ulong clanId)
		{
			IEnumerable<ClanMember> clanMembers = this.GetClanMembers(clanId);
			string name = this.GetClanInfo(clanId).Name;
			List<cache_domain> list = new List<cache_domain>
			{
				cache_domains.clan[clanId],
				cache_domains.clan[name]
			};
			foreach (ClanMember clanMember in clanMembers)
			{
				list.Add(cache_domains.profile[clanMember.ProfileID].clan_info);
				list.Add(cache_domains.profile[clanMember.ProfileID].clan_kicks);
			}
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domains = list,
				set_func = (() => this.m_clanSystem.RemoveClan(clanId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x00024D8C File Offset: 0x0002318C
		public void SetClanInfo(ulong clanId, string description)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.clan[clanId].clan_info,
				set_func = (() => this.m_clanSystem.SetClanInfo(clanId, description))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x00024DF8 File Offset: 0x000231F8
		public ulong AddClanPoints(ulong clanId, ulong targetId, ulong clanPoints)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ulong> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ulong>
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.profile[targetId].clan_info,
					cache_domains.clan[clanId].members,
					cache_domains.clan[clanId].clan_info
				},
				set_func = (() => this.m_clanSystem.AddClanPoints(clanId, targetId, clanPoints))
			};
			return base.SetAndClearScalar<ulong>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x00024EAC File Offset: 0x000232AC
		public IEnumerable<ClanMember> GetClanMembers(ulong clanId)
		{
			DALCacheProxy<IDALService>.Options<ClanMember> options = new DALCacheProxy<IDALService>.Options<ClanMember>
			{
				cache_domain = cache_domains.clan[clanId].members,
				get_data_stream = (() => this.m_clanSystem.GetClanMembers(clanId))
			};
			return base.GetDataStream<ClanMember>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x00024F14 File Offset: 0x00023314
		public EAddMemberResult AddClanMember(ulong clanId, ulong profileId, uint limit)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<EAddMemberResult> options = new DALCacheProxy<IDALService>.SetOptionsScalar<EAddMemberResult>
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.clan[clanId].clan_info,
					cache_domains.clan[clanId].members,
					cache_domains.profile[profileId].clan_info
				},
				set_func = (() => this.m_clanSystem.AddClanMember(clanId, profileId, limit))
			};
			return base.SetAndClearScalar<EAddMemberResult>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x00024FC8 File Offset: 0x000233C8
		public void KickClanMember(ulong clanId, ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.profile[profileId].clan_kicks,
					cache_domains.clan[clanId].members,
					cache_domains.clan[clanId].clan_info,
					cache_domains.profile[profileId].clan_info
				},
				set_func = (() => this.m_clanSystem.KickClanMember(clanId, profileId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x00025094 File Offset: 0x00023494
		public ulong RemoveClanMember(ulong clanId, ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ulong> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ulong>
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.profile[profileId].clan_info,
					cache_domains.clan[clanId].members,
					cache_domains.clan[clanId].clan_info
				},
				set_func = (() => this.m_clanSystem.RemoveClanMember(clanId, profileId))
			};
			ulong num = base.SetAndClearScalar<ulong>(MethodBase.GetCurrentMethod(), options);
			if (num != 0UL)
			{
				IMemcachedService service = ServicesManager.GetService<IMemcachedService>();
				if (service != null && service.Connected)
				{
					service.Remove(cache_domains.profile[num].clan_info);
				}
			}
			return num;
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x00025184 File Offset: 0x00023584
		public void SetUserClanRole(ulong clanId, ulong masterId, ulong profileId, uint role)
		{
			List<cache_domain> list = new List<cache_domain>(3)
			{
				cache_domains.profile[profileId].clan_info,
				cache_domains.clan[clanId].members
			};
			if (role == 1U)
			{
				list.Add(cache_domains.profile[masterId].clan_info);
			}
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domains = list,
				set_func = (() => this.m_clanSystem.SetUserClanRole(clanId, masterId, profileId, role))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x00025260 File Offset: 0x00023660
		public uint FixupClans()
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<uint> options = new DALCacheProxy<IDALService>.SetOptionsScalar<uint>
			{
				set_func = (() => this.m_clanSystem.FixupClans())
			};
			return base.SetAndClearScalar<uint>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009EA RID: 2538 RVA: 0x00025294 File Offset: 0x00023694
		public void FixupClansMasters()
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_clanSystem.FixupClansMasters())
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x000252C8 File Offset: 0x000236C8
		public bool FixupClan(ulong clan_id)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<bool> options = new DALCacheProxy<IDALService>.SetOptionsScalar<bool>
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.clan[clan_id].members,
					cache_domains.clan[clan_id].clan_info
				},
				set_func = (() => this.m_clanSystem.FixupClan(clan_id))
			};
			return base.SetAndClearScalar<bool>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x00025354 File Offset: 0x00023754
		public void DebugGenerateClans(uint count)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_clanSystem.DebugGenerateClans(count))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x0002539C File Offset: 0x0002379C
		public void FlushClanCacheForMember(ulong profileId)
		{
			ClanMember memberInfo = this.GetMemberInfo(profileId);
			if (memberInfo == null)
			{
				return;
			}
			DALCacheProxy<IDALService>.SetOptionsBase options = new DALCacheProxy<IDALService>.SetOptionsBase
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.clan[memberInfo.ClanID].clan_info,
					cache_domains.clan[memberInfo.ClanID].members,
					cache_domains.profile[profileId].clan_info
				}
			};
			base.ClearCache(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x00025428 File Offset: 0x00023828
		public void DebugDeleteAllClans()
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_clanSystem.DebugDeleteAllClans())
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x0400054F RID: 1359
		private IClanSystem m_clanSystem;
	}
}
