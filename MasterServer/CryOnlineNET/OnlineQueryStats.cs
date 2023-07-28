using System;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000171 RID: 369
	public class OnlineQueryStats
	{
		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000698 RID: 1688 RVA: 0x0001A6C1 File Offset: 0x00018AC1
		// (set) Token: 0x06000699 RID: 1689 RVA: 0x0001A6C9 File Offset: 0x00018AC9
		public string Tag { get; set; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x0600069A RID: 1690 RVA: 0x0001A6D2 File Offset: 0x00018AD2
		// (set) Token: 0x0600069B RID: 1691 RVA: 0x0001A6DA File Offset: 0x00018ADA
		public QueryType Type { get; set; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x0600069C RID: 1692 RVA: 0x0001A6E3 File Offset: 0x00018AE3
		// (set) Token: 0x0600069D RID: 1693 RVA: 0x0001A6EB File Offset: 0x00018AEB
		public bool Succeeded { get; set; }

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x0600069E RID: 1694 RVA: 0x0001A6F4 File Offset: 0x00018AF4
		// (set) Token: 0x0600069F RID: 1695 RVA: 0x0001A6FC File Offset: 0x00018AFC
		public uint InboundCompressedSize { get; set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060006A0 RID: 1696 RVA: 0x0001A705 File Offset: 0x00018B05
		// (set) Token: 0x060006A1 RID: 1697 RVA: 0x0001A70D File Offset: 0x00018B0D
		public uint InboundDataSize { get; set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060006A2 RID: 1698 RVA: 0x0001A716 File Offset: 0x00018B16
		// (set) Token: 0x060006A3 RID: 1699 RVA: 0x0001A71E File Offset: 0x00018B1E
		public uint OutboundCompressedSize { get; set; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060006A4 RID: 1700 RVA: 0x0001A727 File Offset: 0x00018B27
		// (set) Token: 0x060006A5 RID: 1701 RVA: 0x0001A72F File Offset: 0x00018B2F
		public uint OutboundDataSize { get; set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060006A6 RID: 1702 RVA: 0x0001A738 File Offset: 0x00018B38
		// (set) Token: 0x060006A7 RID: 1703 RVA: 0x0001A740 File Offset: 0x00018B40
		public TimeSpan ServicingTime { get; set; }
	}
}
