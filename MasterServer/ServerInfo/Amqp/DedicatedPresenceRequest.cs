using System;
using DedicatedPoolServer.Model;
using Network;
using Network.Interfaces;

namespace MasterServer.ServerInfo.Amqp
{
	// Token: 0x020006B6 RID: 1718
	[Domain("dps.presence")]
	public class DedicatedPresenceRequest : DedicatedPresenceModel, IRemoteRequest, IRemoteMessage
	{
		// Token: 0x06002408 RID: 9224 RVA: 0x00096F0F File Offset: 0x0009530F
		public DedicatedPresenceRequest()
		{
			this.Metadata = new RequestMetadata();
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x06002409 RID: 9225 RVA: 0x00096F22 File Offset: 0x00095322
		// (set) Token: 0x0600240A RID: 9226 RVA: 0x00096F2A File Offset: 0x0009532A
		public Uri Url { get; set; }

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x0600240B RID: 9227 RVA: 0x00096F33 File Offset: 0x00095333
		// (set) Token: 0x0600240C RID: 9228 RVA: 0x00096F3B File Offset: 0x0009533B
		public RequestMetadata Metadata { get; set; }

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x0600240D RID: 9229 RVA: 0x00096F44 File Offset: 0x00095344
		public bool IsRepeatable
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x0600240E RID: 9230 RVA: 0x00096F4B File Offset: 0x0009534B
		public void Repeat()
		{
			throw new NotImplementedException();
		}
	}
}
