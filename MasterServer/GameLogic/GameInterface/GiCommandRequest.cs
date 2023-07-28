using System;
using Network;
using Network.Interfaces;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020000AB RID: 171
	[Domain("gi")]
	public class GiCommandRequest : IRemoteRequest, IRemoteMessage
	{
		// Token: 0x060002B7 RID: 695 RVA: 0x0000DC47 File Offset: 0x0000C047
		public GiCommandRequest()
		{
			this.Metadata = new RequestMetadata();
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060002B8 RID: 696 RVA: 0x0000DC5A File Offset: 0x0000C05A
		// (set) Token: 0x060002B9 RID: 697 RVA: 0x0000DC62 File Offset: 0x0000C062
		public string Text { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060002BA RID: 698 RVA: 0x0000DC6B File Offset: 0x0000C06B
		// (set) Token: 0x060002BB RID: 699 RVA: 0x0000DC73 File Offset: 0x0000C073
		public string Domain { get; set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060002BC RID: 700 RVA: 0x0000DC7C File Offset: 0x0000C07C
		// (set) Token: 0x060002BD RID: 701 RVA: 0x0000DC84 File Offset: 0x0000C084
		public Uri Url { get; set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060002BE RID: 702 RVA: 0x0000DC8D File Offset: 0x0000C08D
		// (set) Token: 0x060002BF RID: 703 RVA: 0x0000DC95 File Offset: 0x0000C095
		public RequestMetadata Metadata { get; set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060002C0 RID: 704 RVA: 0x0000DC9E File Offset: 0x0000C09E
		public bool IsRepeatable
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x0000DCA5 File Offset: 0x0000C0A5
		public void Repeat()
		{
			throw new NotImplementedException();
		}
	}
}
