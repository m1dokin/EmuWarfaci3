using System;
using System.IO;
using System.Text;
using MasterServer.Core.Configuration;
using MasterServer.Core.LogSenders;

namespace MasterServer.Core
{
	// Token: 0x02000144 RID: 324
	internal class LogProxySync : ILogSync, IDisposable
	{
		// Token: 0x0600059E RID: 1438 RVA: 0x00016BAC File Offset: 0x00014FAC
		public LogProxySync()
		{
			ConfigSection section = Resources.CommonSettings.GetSection("log_server");
			string text = section.Get("type");
			string text2 = text.ToLower();
			if (text2 != null)
			{
				if (!(text2 == "udp"))
				{
					if (!(text2 == "tcp"))
					{
						goto IL_76;
					}
					this.m_logSender = new TcpSyncLogSender(section);
				}
				else
				{
					this.m_logSender = new UdpLogSender(section);
				}
				return;
			}
			IL_76:
			throw new ArgumentException(string.Format("Invalid log server type '{0}'", text));
		}

		// Token: 0x0600059F RID: 1439 RVA: 0x00016C40 File Offset: 0x00015040
		public void WriteToLog(int group, string data)
		{
			Encoding utf = Encoding.UTF8;
			using (this.m_logSender.Lock())
			{
				using (MemoryStream stream = this.m_logSender.GetStream(utf))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(stream))
					{
						binaryWriter.Write(0);
						binaryWriter.Write(0);
						binaryWriter.Write(Resources.RealmId);
						byte[] bytes = utf.GetBytes(Resources.ServerName);
						binaryWriter.Write(bytes.Length);
						binaryWriter.Write(bytes);
						binaryWriter.Write(group);
						byte[] bytes2 = utf.GetBytes(data);
						binaryWriter.Write(bytes2.Length);
						binaryWriter.Write(bytes2);
						int num = (int)stream.Position;
						binaryWriter.Seek(0, SeekOrigin.Begin);
						binaryWriter.Write(num);
						binaryWriter.Seek(num, SeekOrigin.Begin);
						this.m_logSender.Flush(stream);
					}
				}
			}
		}

		// Token: 0x060005A0 RID: 1440 RVA: 0x00016D5C File Offset: 0x0001515C
		public void EndGroup(int group)
		{
			Encoding utf = Encoding.UTF8;
			using (this.m_logSender.Lock())
			{
				using (MemoryStream stream = this.m_logSender.GetStream(utf))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(stream))
					{
						binaryWriter.Write(0);
						binaryWriter.Write(1);
						binaryWriter.Write(Resources.RealmId);
						byte[] bytes = utf.GetBytes(Resources.ServerName);
						binaryWriter.Write(bytes.Length);
						binaryWriter.Write(bytes);
						binaryWriter.Write(group);
						int num = (int)stream.Position;
						binaryWriter.Seek(0, SeekOrigin.Begin);
						binaryWriter.Write(num);
						binaryWriter.Seek(num, SeekOrigin.Begin);
						this.m_logSender.Flush(stream);
					}
				}
			}
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x00016E5C File Offset: 0x0001525C
		public void Dispose()
		{
			if (this.m_logSender != null)
			{
				this.m_logSender.Dispose();
				this.m_logSender = null;
			}
		}

		// Token: 0x040003AD RID: 941
		private ILogSender m_logSender;
	}
}
