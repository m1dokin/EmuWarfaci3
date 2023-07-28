using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MasterServer.Core.Configuration;

namespace MasterServer.Core.LogSenders
{
	// Token: 0x02000137 RID: 311
	internal class UdpLogSender : LogSender
	{
		// Token: 0x06000519 RID: 1305 RVA: 0x000165F4 File Offset: 0x000149F4
		public UdpLogSender(ConfigSection logServerConfigSection) : base(logServerConfigSection)
		{
			this.m_client = new UdpClient();
			Log.Info("Udp log sender has been created.");
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x00016612 File Offset: 0x00014A12
		public override ILocker Lock()
		{
			return new UdpLogSender.NoLocker();
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x00016619 File Offset: 0x00014A19
		public override MemoryStream GetStream(Encoding encoding)
		{
			return new MemoryStream();
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x00016620 File Offset: 0x00014A20
		public override void Flush(MemoryStream stream)
		{
			byte[] buffer = stream.GetBuffer();
			foreach (IPEndPoint endPoint in this.m_proxies)
			{
				this.m_client.Send(buffer, buffer.Length, endPoint);
			}
		}

		// Token: 0x0400022D RID: 557
		private readonly UdpClient m_client;

		// Token: 0x02000138 RID: 312
		private class NoLocker : ILocker, IDisposable
		{
			// Token: 0x0600051E RID: 1310 RVA: 0x00016698 File Offset: 0x00014A98
			public void Dispose()
			{
			}
		}
	}
}
