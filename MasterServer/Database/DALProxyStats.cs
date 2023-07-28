using System;
using MasterServer.DAL;

namespace MasterServer.Database
{
	// Token: 0x0200003C RID: 60
	public class DALProxyStats : DALStats
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000F0 RID: 240 RVA: 0x000085DC File Offset: 0x000069DC
		// (set) Token: 0x060000F1 RID: 241 RVA: 0x000085E4 File Offset: 0x000069E4
		public string Procedure { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x000085ED File Offset: 0x000069ED
		// (set) Token: 0x060000F3 RID: 243 RVA: 0x000085F5 File Offset: 0x000069F5
		public int L1CacheMisses { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x000085FE File Offset: 0x000069FE
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x00008606 File Offset: 0x00006A06
		public int L1CacheHits { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x0000860F File Offset: 0x00006A0F
		// (set) Token: 0x060000F7 RID: 247 RVA: 0x00008617 File Offset: 0x00006A17
		public int L1CacheClear { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x00008620 File Offset: 0x00006A20
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x00008628 File Offset: 0x00006A28
		public int L2CacheMisses { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000FA RID: 250 RVA: 0x00008631 File Offset: 0x00006A31
		// (set) Token: 0x060000FB RID: 251 RVA: 0x00008639 File Offset: 0x00006A39
		public int L2CacheHits { get; set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000FC RID: 252 RVA: 0x00008642 File Offset: 0x00006A42
		// (set) Token: 0x060000FD RID: 253 RVA: 0x0000864A File Offset: 0x00006A4A
		public int L2CacheClear { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000FE RID: 254 RVA: 0x00008653 File Offset: 0x00006A53
		// (set) Token: 0x060000FF RID: 255 RVA: 0x0000865B File Offset: 0x00006A5B
		public TimeSpan CacheTime { get; set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000100 RID: 256 RVA: 0x00008664 File Offset: 0x00006A64
		// (set) Token: 0x06000101 RID: 257 RVA: 0x0000866C File Offset: 0x00006A6C
		public TimeSpan DALTime { get; set; }
	}
}
