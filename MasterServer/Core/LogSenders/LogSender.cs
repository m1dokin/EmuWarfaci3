using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using MasterServer.Core.Configuration;

namespace MasterServer.Core.LogSenders
{
	// Token: 0x02000123 RID: 291
	internal abstract class LogSender : ILogSender, IDisposable
	{
		// Token: 0x060004B9 RID: 1209 RVA: 0x00014058 File Offset: 0x00012458
		protected LogSender(ConfigSection logServerConfigSection)
		{
			foreach (ConfigSection configSection in logServerConfigSection.GetSections("server"))
			{
				string text = configSection.Get("host");
				if (!string.IsNullOrEmpty(text))
				{
					int port = int.Parse(configSection.Get("port"));
					IPAddress address;
					if (!IPAddress.TryParse(text, out address))
					{
						IPAddress[] hostAddresses = Dns.GetHostAddresses(text);
						address = hostAddresses[0];
					}
					this.m_proxies.Add(new IPEndPoint(address, port));
				}
			}
		}

		// Token: 0x060004BA RID: 1210
		public abstract ILocker Lock();

		// Token: 0x060004BB RID: 1211
		public abstract MemoryStream GetStream(Encoding encoding);

		// Token: 0x060004BC RID: 1212
		public abstract void Flush(MemoryStream stream);

		// Token: 0x060004BD RID: 1213 RVA: 0x00014120 File Offset: 0x00012520
		public virtual void Dispose()
		{
		}

		// Token: 0x04000200 RID: 512
		protected readonly List<IPEndPoint> m_proxies = new List<IPEndPoint>();
	}
}
