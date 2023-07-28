using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MasterServer.Core.Configuration;

namespace MasterServer.Core.LogSenders
{
	// Token: 0x02000133 RID: 307
	internal abstract class TcpLogSender : LogSender
	{
		// Token: 0x0600050C RID: 1292 RVA: 0x000159F4 File Offset: 0x00013DF4
		public TcpLogSender(ConfigSection logServerConfigSection) : base(logServerConfigSection)
		{
		}

		// Token: 0x0600050D RID: 1293 RVA: 0x00015A28 File Offset: 0x00013E28
		public override ILocker Lock()
		{
			return new TcpLogSender.Locker(this);
		}

		// Token: 0x0600050E RID: 1294 RVA: 0x00015A30 File Offset: 0x00013E30
		public override MemoryStream GetStream(Encoding encoding)
		{
			int num = this.m_backBuffer.Length - this.m_backBufferPosition;
			if ((float)num < (float)this.m_backBuffer.Length * 0.05f)
			{
				if (this.m_enlargeBackBufferSizeCount == 3)
				{
					Log.Error("Max enlarge buffer size count reached. Some log information will be erased.");
					Array.Resize<byte>(ref this.m_backBuffer, 1048576);
					this.m_backBufferPosition = 0;
					this.m_enlargeBackBufferSizeCount = 0;
				}
				else
				{
					string format = string.Format("Tcp log sender's buffer is critically filled. Enlarge it to {0}.", this.m_backBuffer.Length * 2);
					if (this.m_enlargeBackBufferSizeCount < 2)
					{
						Log.Info(format);
					}
					else
					{
						Log.Warning(format);
					}
					Array.Resize<byte>(ref this.m_backBuffer, this.m_backBuffer.Length * 2);
					this.m_enlargeBackBufferSizeCount++;
				}
			}
			this.m_stream = new MemoryStream(this.m_backBuffer, this.m_backBufferPosition, this.m_backBuffer.Length - this.m_backBufferPosition);
			return this.m_stream;
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x00015B22 File Offset: 0x00013F22
		public override void Flush(MemoryStream stream)
		{
			if (stream != this.m_stream)
			{
				throw new InvalidOperationException("Incorrect stream object.");
			}
			this.m_backBufferPosition += (int)this.m_stream.Position;
			this.m_stream = null;
		}

		// Token: 0x04000218 RID: 536
		protected readonly object m_sync = new object();

		// Token: 0x04000219 RID: 537
		protected const int RECONNECT_TIMEOUT = 5000;

		// Token: 0x0400021A RID: 538
		protected const int BUFFER_SIZE = 1048576;

		// Token: 0x0400021B RID: 539
		private const float BUFFER_CRITICAL_SIZE = 0.05f;

		// Token: 0x0400021C RID: 540
		private const int ENLARGE_BUFFER_SIZE_COUNT_MAX = 3;

		// Token: 0x0400021D RID: 541
		private const int ENLARGE_BUFFER_SIZE_COEF = 2;

		// Token: 0x0400021E RID: 542
		protected int m_bufferPosition;

		// Token: 0x0400021F RID: 543
		protected byte[] m_buffer = new byte[1048576];

		// Token: 0x04000220 RID: 544
		protected int m_enlargeBufferSizeCount;

		// Token: 0x04000221 RID: 545
		protected int m_backBufferPosition;

		// Token: 0x04000222 RID: 546
		protected byte[] m_backBuffer = new byte[1048576];

		// Token: 0x04000223 RID: 547
		protected int m_enlargeBackBufferSizeCount;

		// Token: 0x04000224 RID: 548
		private MemoryStream m_stream;

		// Token: 0x02000134 RID: 308
		private class Locker : ILocker, IDisposable
		{
			// Token: 0x06000510 RID: 1296 RVA: 0x00015B5B File Offset: 0x00013F5B
			public Locker(TcpLogSender sender)
			{
				this.m_sender = sender;
				Monitor.Enter(sender.m_sync);
			}

			// Token: 0x06000511 RID: 1297 RVA: 0x00015B75 File Offset: 0x00013F75
			public void Dispose()
			{
				Monitor.Exit(this.m_sender.m_sync);
			}

			// Token: 0x04000225 RID: 549
			private readonly TcpLogSender m_sender;
		}

		// Token: 0x02000135 RID: 309
		protected class SocketObject
		{
			// Token: 0x06000512 RID: 1298 RVA: 0x00015B87 File Offset: 0x00013F87
			public SocketObject(IPEndPoint endPoint)
			{
				this.EndPoint = endPoint;
				this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				this.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
				this.SentDataSize = 0;
			}

			// Token: 0x04000226 RID: 550
			public readonly IPEndPoint EndPoint;

			// Token: 0x04000227 RID: 551
			public readonly Socket Socket;

			// Token: 0x04000228 RID: 552
			public int SentDataSize;
		}
	}
}
