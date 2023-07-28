using System;
using Network.Interfaces;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020000AC RID: 172
	public class GiCommandResponse : IRemoteResponse, IRemoteMessage, IDisposable
	{
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060002C3 RID: 707 RVA: 0x0000DCB4 File Offset: 0x0000C0B4
		// (set) Token: 0x060002C4 RID: 708 RVA: 0x0000DCBC File Offset: 0x0000C0BC
		public string Text { get; set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060002C5 RID: 709 RVA: 0x0000DCC5 File Offset: 0x0000C0C5
		public Uri Url
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0000DCC8 File Offset: 0x0000C0C8
		public void Dispose()
		{
		}
	}
}
