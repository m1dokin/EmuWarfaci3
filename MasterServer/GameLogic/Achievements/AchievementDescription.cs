using System;
using System.Collections.Generic;
using MasterServer.GameLogic.StatsTracking;

namespace MasterServer.GameLogic.Achievements
{
	// Token: 0x02000059 RID: 89
	public class AchievementDescription
	{
		// Token: 0x06000155 RID: 341 RVA: 0x00009E1E File Offset: 0x0000821E
		public AchievementDescription(uint id, EStatsEvent achKind, uint amount, bool msSide, string name)
		{
			this.Id = id;
			this.Amount = amount;
			this.Kind = achKind;
			this.MasterServerSide = msSide;
			this.Name = name;
			this.Filters = new List<IStatsFilter>();
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000156 RID: 342 RVA: 0x00009E56 File Offset: 0x00008256
		// (set) Token: 0x06000157 RID: 343 RVA: 0x00009E5E File Offset: 0x0000825E
		public uint Id { get; set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000158 RID: 344 RVA: 0x00009E67 File Offset: 0x00008267
		// (set) Token: 0x06000159 RID: 345 RVA: 0x00009E6F File Offset: 0x0000826F
		public uint Amount { get; set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600015A RID: 346 RVA: 0x00009E78 File Offset: 0x00008278
		// (set) Token: 0x0600015B RID: 347 RVA: 0x00009E80 File Offset: 0x00008280
		public EStatsEvent Kind { get; set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600015C RID: 348 RVA: 0x00009E89 File Offset: 0x00008289
		// (set) Token: 0x0600015D RID: 349 RVA: 0x00009E91 File Offset: 0x00008291
		public bool MasterServerSide { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600015E RID: 350 RVA: 0x00009E9A File Offset: 0x0000829A
		// (set) Token: 0x0600015F RID: 351 RVA: 0x00009EA2 File Offset: 0x000082A2
		public string Name { get; set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000160 RID: 352 RVA: 0x00009EAB File Offset: 0x000082AB
		// (set) Token: 0x06000161 RID: 353 RVA: 0x00009EB3 File Offset: 0x000082B3
		public List<IStatsFilter> Filters { get; set; }

		// Token: 0x06000162 RID: 354 RVA: 0x00009EBC File Offset: 0x000082BC
		public override string ToString()
		{
			return string.Format("Id: {0}, Name: {1}, Kind: {2}", this.Id, this.Name, this.Kind);
		}
	}
}
