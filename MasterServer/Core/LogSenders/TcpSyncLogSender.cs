using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MasterServer.Core.Configuration;

namespace MasterServer.Core.LogSenders
{
	// Token: 0x02000136 RID: 310
	internal class TcpSyncLogSender : TcpLogSender
	{
		// Token: 0x06000513 RID: 1299 RVA: 0x000161B8 File Offset: 0x000145B8
		public TcpSyncLogSender(ConfigSection logServerConfigSection) : base(logServerConfigSection)
		{
			foreach (IPEndPoint parameter in this.m_proxies)
			{
				new Thread(new ParameterizedThreadStart(this.Loop)).Start(parameter);
			}
			Log.Info("Tcp sync log sender has been created.");
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x00016240 File Offset: 0x00014640
		public override void Dispose()
		{
			this.m_shutdownRequested = true;
			object sentSync = this.m_sentSync;
			lock (sentSync)
			{
				Monitor.PulseAll(this.m_sentSync);
			}
		}

		// Token: 0x06000515 RID: 1301 RVA: 0x00016290 File Offset: 0x00014690
		private void Loop(object parameter)
		{
			IPEndPoint endPoint = (IPEndPoint)parameter;
			while (!this.m_shutdownRequested)
			{
				Socket socket = this.Connect(endPoint);
				if (socket != null)
				{
					object sentSync = this.m_sentSync;
					lock (sentSync)
					{
						this.m_sendingSocketCount++;
					}
					this.Send(socket);
				}
			}
		}

		// Token: 0x06000516 RID: 1302 RVA: 0x00016308 File Offset: 0x00014708
		private Socket Connect(IPEndPoint endPoint)
		{
			while (!this.m_shutdownRequested)
			{
				try
				{
					Log.Info<IPEndPoint>("Try to connect to {0} for sending logs.", endPoint);
					Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
					socket.Connect(endPoint);
					if (socket.Connected)
					{
						Log.Info<IPEndPoint>("Connect to {0} for sending logs has been established.", endPoint);
						return socket;
					}
				}
				catch (Exception e)
				{
					Log.Warning<IPEndPoint>("Can't connect to {0} for sending logs.", endPoint);
					Log.Warning(e);
					Thread.Sleep(5000);
				}
			}
			return null;
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x000163A4 File Offset: 0x000147A4
		private void Send(Socket socket)
		{
			string p = (!socket.Connected) ? "<unknown>" : socket.RemoteEndPoint.ToString();
			int num = 0;
			while (!this.m_shutdownRequested)
			{
				try
				{
					num += socket.Send(this.m_buffer, num, this.m_bufferPosition - num, SocketFlags.None);
					if (num >= this.m_bufferPosition)
					{
						object sentSync = this.m_sentSync;
						bool flag2;
						lock (sentSync)
						{
							this.m_sentSocketCount++;
							if (this.CheckAllSocketsSentData(out flag2))
							{
								Monitor.PulseAll(this.m_sentSync);
							}
							else
							{
								Monitor.Wait(this.m_sentSync);
							}
							num = 0;
						}
						if (flag2)
						{
							Thread.Sleep(100);
						}
					}
				}
				catch (Exception e)
				{
					Log.Warning<string>("Can't send data to {0}.", p);
					Log.Warning(e);
					Log.Warning("Disconnect socket.");
					try
					{
						socket.Disconnect(true);
					}
					catch
					{
					}
					object sentSync2 = this.m_sentSync;
					lock (sentSync2)
					{
						this.m_sendingSocketCount--;
						bool flag4;
						if (this.CheckAllSocketsSentData(out flag4))
						{
							Monitor.PulseAll(this.m_sentSync);
						}
					}
					break;
				}
			}
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x0001652C File Offset: 0x0001492C
		private bool CheckAllSocketsSentData(out bool emptyBuffer)
		{
			emptyBuffer = false;
			bool flag = this.m_sendingSocketCount > 0 && this.m_sentSocketCount == this.m_sendingSocketCount;
			if (flag)
			{
				object sync = this.m_sync;
				lock (sync)
				{
					byte[] buffer = this.m_buffer;
					this.m_buffer = this.m_backBuffer;
					this.m_backBuffer = buffer;
					this.m_bufferPosition = this.m_backBufferPosition;
					this.m_backBufferPosition = 0;
					this.m_sentSocketCount = 0;
					int enlargeBufferSizeCount = this.m_enlargeBufferSizeCount;
					this.m_enlargeBufferSizeCount = this.m_enlargeBackBufferSizeCount;
					this.m_enlargeBackBufferSizeCount = enlargeBufferSizeCount;
				}
				if (this.m_bufferPosition == 0)
				{
					emptyBuffer = true;
				}
				return true;
			}
			return false;
		}

		// Token: 0x04000229 RID: 553
		private readonly object m_sentSync = new object();

		// Token: 0x0400022A RID: 554
		private int m_sendingSocketCount;

		// Token: 0x0400022B RID: 555
		private int m_sentSocketCount;

		// Token: 0x0400022C RID: 556
		private bool m_shutdownRequested;
	}
}
