using System;
using System.Collections.Generic;
using MasterServer.Database;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001A0 RID: 416
	[Serializable]
	internal class QueryStats
	{
		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060007BF RID: 1983 RVA: 0x0001DBB3 File Offset: 0x0001BFB3
		// (set) Token: 0x060007C0 RID: 1984 RVA: 0x0001DBBB File Offset: 0x0001BFBB
		public TimeSpan ProcessingTime { get; set; }

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060007C1 RID: 1985 RVA: 0x0001DBC4 File Offset: 0x0001BFC4
		// (set) Token: 0x060007C2 RID: 1986 RVA: 0x0001DBCC File Offset: 0x0001BFCC
		public TimeSpan AsyncTime { get; set; }

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060007C3 RID: 1987 RVA: 0x0001DBD5 File Offset: 0x0001BFD5
		// (set) Token: 0x060007C4 RID: 1988 RVA: 0x0001DBDD File Offset: 0x0001BFDD
		public bool Succeeded { get; set; }

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060007C5 RID: 1989 RVA: 0x0001DBE6 File Offset: 0x0001BFE6
		// (set) Token: 0x060007C6 RID: 1990 RVA: 0x0001DBEE File Offset: 0x0001BFEE
		public EOnlineError OnlineError { get; set; }

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x060007C7 RID: 1991 RVA: 0x0001DBF7 File Offset: 0x0001BFF7
		// (set) Token: 0x060007C8 RID: 1992 RVA: 0x0001DBFF File Offset: 0x0001BFFF
		public int CustomCode { get; set; }

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x060007C9 RID: 1993 RVA: 0x0001DC08 File Offset: 0x0001C008
		// (set) Token: 0x060007CA RID: 1994 RVA: 0x0001DC10 File Offset: 0x0001C010
		public IList<DALProxyStats> DALCalls
		{
			get
			{
				return this.m_dalCalls;
			}
			set
			{
				this.m_dalCalls = value;
			}
		}

		// Token: 0x04000491 RID: 1169
		private IList<DALProxyStats> m_dalCalls = new List<DALProxyStats>();
	}
}
