using System;
using System.Collections.Generic;

namespace MasterServer.Database
{
	// Token: 0x020001E3 RID: 483
	public class _profile_domain : cache_domain
	{
		// Token: 0x06000950 RID: 2384 RVA: 0x0002314C File Offset: 0x0002154C
		public _profile_domain(ulong id) : base("profile_" + id)
		{
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000951 RID: 2385 RVA: 0x00023164 File Offset: 0x00021564
		public cache_domain info
		{
			get
			{
				return new cache_domain(this._name + ".info");
			}
		}

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000952 RID: 2386 RVA: 0x0002317B File Offset: 0x0002157B
		public cache_domain items
		{
			get
			{
				return new cache_domain(this._name + ".items");
			}
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06000953 RID: 2387 RVA: 0x00023192 File Offset: 0x00021592
		public cache_domain unlocks
		{
			get
			{
				return new cache_domain(this._name + ".unlocks");
			}
		}

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x06000954 RID: 2388 RVA: 0x000231A9 File Offset: 0x000215A9
		public cache_domain stats
		{
			get
			{
				return new cache_domain(this._name + ".stats");
			}
		}

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06000955 RID: 2389 RVA: 0x000231C0 File Offset: 0x000215C0
		public cache_domain sponsors
		{
			get
			{
				return new cache_domain(this._name + ".sponsors");
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x06000956 RID: 2390 RVA: 0x000231D7 File Offset: 0x000215D7
		public cache_domain persistent_settings
		{
			get
			{
				return new cache_domain(this._name + ".persistent_settings");
			}
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000957 RID: 2391 RVA: 0x000231EE File Offset: 0x000215EE
		public cache_domain mission_performance
		{
			get
			{
				return new cache_domain(this._name + ".mission_perf_");
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000958 RID: 2392 RVA: 0x00023205 File Offset: 0x00021605
		public cache_domain achievements
		{
			get
			{
				return new cache_domain(this._name + ".achievements");
			}
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000959 RID: 2393 RVA: 0x0002321C File Offset: 0x0002161C
		public cache_domain leaderboard
		{
			get
			{
				return new cache_domain(this._name + ".leaderboard");
			}
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x0600095A RID: 2394 RVA: 0x00023233 File Offset: 0x00021633
		public cache_domain notifications
		{
			get
			{
				return new cache_domain(this._name + ".notifications");
			}
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x0600095B RID: 2395 RVA: 0x0002324A File Offset: 0x0002164A
		public cache_domain friends
		{
			get
			{
				return new cache_domain(this._name + ".friends");
			}
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x0600095C RID: 2396 RVA: 0x00023261 File Offset: 0x00021661
		public cache_domain clan_info
		{
			get
			{
				return new cache_domain(this._name + ".clan_info");
			}
		}

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x0600095D RID: 2397 RVA: 0x00023278 File Offset: 0x00021678
		public cache_domain pvp_rating_points
		{
			get
			{
				return new cache_domain(this._name + ".pvp_rating_points");
			}
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x0600095E RID: 2398 RVA: 0x0002328F File Offset: 0x0002168F
		public cache_domain pvp_mode_current_wins
		{
			get
			{
				return new cache_domain(this._name + ".pvp_mode_current_wins");
			}
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x0600095F RID: 2399 RVA: 0x000232A6 File Offset: 0x000216A6
		public cache_domain clan_kicks
		{
			get
			{
				return new cache_domain(this._name + ".clan_kicks");
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06000960 RID: 2400 RVA: 0x000232BD File Offset: 0x000216BD
		public cache_domain contract
		{
			get
			{
				return new cache_domain(this._name + ".contract");
			}
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06000961 RID: 2401 RVA: 0x000232D4 File Offset: 0x000216D4
		public cache_domain profile_progression
		{
			get
			{
				return new cache_domain(this._name + ".profile_progression");
			}
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000962 RID: 2402 RVA: 0x000232EB File Offset: 0x000216EB
		public cache_domain custom_rules_state
		{
			get
			{
				return new cache_domain(this._name + ".custom_rules_state");
			}
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000963 RID: 2403 RVA: 0x00023302 File Offset: 0x00021702
		public cache_domain rating_room_bans
		{
			get
			{
				return new cache_domain(this._name + ".rating_room_bans");
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000964 RID: 2404 RVA: 0x00023319 File Offset: 0x00021719
		public _cl_skill_factory skill
		{
			get
			{
				return new _cl_skill_factory(this._name);
			}
		}

		// Token: 0x06000965 RID: 2405 RVA: 0x00023328 File Offset: 0x00021728
		public override IEnumerable<cache_domain> Terminals()
		{
			foreach (cache_domain terminal in base.Terminals())
			{
				yield return terminal;
			}
			yield return this.info;
			yield return this.items;
			yield return this.unlocks;
			yield return this.stats;
			yield return this.sponsors;
			yield return this.persistent_settings;
			yield return this.mission_performance;
			yield return this.achievements;
			yield return this.leaderboard;
			yield return this.notifications;
			yield return this.friends;
			yield return this.clan_info;
			yield return this.pvp_rating_points;
			yield return this.clan_kicks;
			yield return this.contract;
			yield return this.profile_progression;
			yield return this.custom_rules_state;
			yield return this.rating_room_bans;
			yield break;
		}
	}
}
