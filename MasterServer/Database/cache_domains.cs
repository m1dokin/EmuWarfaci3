using System;

namespace MasterServer.Database
{
	// Token: 0x020001E7 RID: 487
	public static class cache_domains
	{
		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06000975 RID: 2421 RVA: 0x00023D3C File Offset: 0x0002213C
		public static cache_domain all
		{
			get
			{
				return new cache_domain("*");
			}
		}

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000976 RID: 2422 RVA: 0x00023D48 File Offset: 0x00022148
		public static cache_domain generation
		{
			get
			{
				return new cache_domain("_generation_");
			}
		}

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000977 RID: 2423 RVA: 0x00023D54 File Offset: 0x00022154
		public static cache_domain game_items
		{
			get
			{
				return new cache_domain("game_items");
			}
		}

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000978 RID: 2424 RVA: 0x00023D60 File Offset: 0x00022160
		public static cache_domain default_profile
		{
			get
			{
				return new cache_domain("default_profile");
			}
		}

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06000979 RID: 2425 RVA: 0x00023D6C File Offset: 0x0002216C
		public static cache_domain ecat_test_route
		{
			get
			{
				return new cache_domain("ecat_test_route");
			}
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x0600097A RID: 2426 RVA: 0x00023D78 File Offset: 0x00022178
		public static cache_domain mission_performance
		{
			get
			{
				return new cache_domain("mission_performance");
			}
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x0600097B RID: 2427 RVA: 0x00023D84 File Offset: 0x00022184
		public static cache_domain clan_performance_master_record
		{
			get
			{
				return new cache_domain("clan_performance_mr");
			}
		}

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x0600097C RID: 2428 RVA: 0x00023D90 File Offset: 0x00022190
		public static cache_domain announcements
		{
			get
			{
				return new cache_domain("announcement");
			}
		}

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x0600097D RID: 2429 RVA: 0x00023D9C File Offset: 0x0002219C
		public static cache_domain clan_top
		{
			get
			{
				return new cache_domain("clan_top");
			}
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x0600097E RID: 2430 RVA: 0x00023DA8 File Offset: 0x000221A8
		public static cache_domain voucher_globals
		{
			get
			{
				return new cache_domain("voucher_globals");
			}
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x0600097F RID: 2431 RVA: 0x00023DB4 File Offset: 0x000221B4
		public static cache_domain rating_season
		{
			get
			{
				return new cache_domain("rating_season");
			}
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x06000980 RID: 2432 RVA: 0x00023DC0 File Offset: 0x000221C0
		public static cache_domains._cl_user_factory user
		{
			get
			{
				return default(cache_domains._cl_user_factory);
			}
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000981 RID: 2433 RVA: 0x00023DD8 File Offset: 0x000221D8
		public static cache_domains._cl_profile_factory profile
		{
			get
			{
				return default(cache_domains._cl_profile_factory);
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000982 RID: 2434 RVA: 0x00023DF0 File Offset: 0x000221F0
		public static cache_domains._cl_clan_factory clan
		{
			get
			{
				return default(cache_domains._cl_clan_factory);
			}
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000983 RID: 2435 RVA: 0x00023E08 File Offset: 0x00022208
		public static cache_domains._cl_profile_mapping_factory profile_mapping
		{
			get
			{
				return default(cache_domains._cl_profile_mapping_factory);
			}
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06000984 RID: 2436 RVA: 0x00023E1E File Offset: 0x0002221E
		public static cache_domain shop_offers
		{
			get
			{
				return new cache_domain("shop_offers");
			}
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06000985 RID: 2437 RVA: 0x00023E2A File Offset: 0x0002222A
		public static cache_domain catalog_items
		{
			get
			{
				return new cache_domain("catalog_items");
			}
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000986 RID: 2438 RVA: 0x00023E38 File Offset: 0x00022238
		public static cache_domains._cl_customer_factory customer
		{
			get
			{
				return default(cache_domains._cl_customer_factory);
			}
		}

		// Token: 0x020001E8 RID: 488
		public struct _cl_user_factory
		{
			// Token: 0x17000152 RID: 338
			public _user_domain this[ulong uid]
			{
				get
				{
					return new _user_domain(uid);
				}
			}
		}

		// Token: 0x020001E9 RID: 489
		public struct _cl_profile_factory
		{
			// Token: 0x17000153 RID: 339
			public _profile_domain this[ulong pid]
			{
				get
				{
					return new _profile_domain(pid);
				}
			}
		}

		// Token: 0x020001EA RID: 490
		public struct _cl_clan_factory
		{
			// Token: 0x17000154 RID: 340
			public _clan_domain this[ulong cid]
			{
				get
				{
					return new _clan_domain(cid);
				}
			}

			// Token: 0x17000155 RID: 341
			public _clan_domain this[string cName]
			{
				get
				{
					return new _clan_domain(cName);
				}
			}
		}

		// Token: 0x020001EB RID: 491
		public struct _cl_profile_mapping_factory
		{
			// Token: 0x17000156 RID: 342
			public cache_domain this[string nick]
			{
				get
				{
					return new cache_domain("profile_map_" + nick);
				}
			}
		}

		// Token: 0x020001EC RID: 492
		public struct _cl_customer_factory
		{
			// Token: 0x17000157 RID: 343
			public _customer_domain this[ulong id]
			{
				get
				{
					return new _customer_domain(id);
				}
			}
		}
	}
}
