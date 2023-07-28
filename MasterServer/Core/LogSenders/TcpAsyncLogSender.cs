using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MasterServer.Core.Configuration;

namespace MasterServer.Core.LogSenders
{
	// Token: 0x02000132 RID: 306
	internal class TcpAsyncLogSender : TcpLogSender
	{
		// Token: 0x06000507 RID: 1287 RVA: 0x00015BC0 File Offset: 0x00013FC0
		public TcpAsyncLogSender(ConfigSection logServerConfigSection) : base(logServerConfigSection)
		{
			this.m_sockets = new List<TcpLogSender.SocketObject>(this.m_proxies.Count);
			foreach (IPEndPoint endPoint in this.m_proxies)
			{
				this.Connect(new TcpLogSender.SocketObject(endPoint));
			}
			Log.Info("Tcp async log sender has been created.");
		}

		// Token: 0x06000508 RID: 1288 RVA: 0x00015C48 File Offset: 0x00014048
		private void Connect(TcpLogSender.SocketObject socket)
		{
			try
			{
				Log.Info<IPEndPoint>("Try to connect to {0} for sending logs.", socket.EndPoint);
				socket.Socket.BeginConnect(socket.EndPoint, new AsyncCallback(this.OnConnected), socket);
			}
			catch (Exception e)
			{
				Log.Warning<TcpLogSender.SocketObject>("Can't connect to {0} for sending logs.", socket);
				Log.Warning(e);
				Thread.Sleep(5000);
				ThreadPool.QueueUserWorkItem(delegate(object state)
				{
					this.Connect(socket);
				});
			}
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x00015CFC File Offset: 0x000140FC
		private void OnConnected(IAsyncResult asyncResult)
		{
			TcpLogSender.SocketObject socketObject = (TcpLogSender.SocketObject)asyncResult.AsyncState;
			try
			{
				socketObject.Socket.EndConnect(asyncResult);
				Log.Info<IPEndPoint>("Connect to {0} for sending logs has been established.", socketObject.EndPoint);
				socketObject.SentDataSize = 0;
			}
			catch (Exception e)
			{
				Log.Warning<IPEndPoint>("Can't connect to {0} for sending logs.", socketObject.EndPoint);
				Log.Warning(e);
				Thread.Sleep(5000);
				this.Connect(socketObject);
				return;
			}
			try
			{
				object sockets = this.m_sockets;
				lock (sockets)
				{
					socketObject.Socket.BeginSend(this.m_buffer, 0, this.m_bufferPosition, SocketFlags.None, new AsyncCallback(this.OnSent), socketObject);
					this.m_sockets.Add(socketObject);
				}
			}
			catch (Exception e2)
			{
				Log.Warning<IPEndPoint>("Can't send data to {0}.", socketObject.EndPoint);
				Log.Warning(e2);
				Log.Warning("Disconnect current socket.");
				try
				{
					socketObject.Socket.Disconnect(true);
				}
				catch
				{
				}
				object sockets2 = this.m_sockets;
				lock (sockets2)
				{
					this.m_sockets.Remove(socketObject);
				}
				this.Connect(socketObject);
			}
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x00015E7C File Offset: 0x0001427C
		private void OnSent(IAsyncResult asyncResult)
		{
			TcpLogSender.SocketObject socketObject = (TcpLogSender.SocketObject)asyncResult.AsyncState;
			try
			{
				SocketError socketError;
				int num = socketObject.Socket.EndSend(asyncResult, out socketError);
				if (socketError != SocketError.Success)
				{
					throw new Exception("Socket has been disconnected.");
				}
				if (num >= this.m_bufferPosition)
				{
					object sockets = this.m_sockets;
					lock (sockets)
					{
						this.m_sentSocketCount++;
						this.CheckAllSocketsSentData();
					}
				}
				else
				{
					socketObject.SentDataSize += num;
					socketObject.Socket.BeginSend(this.m_buffer, socketObject.SentDataSize, this.m_bufferPosition - socketObject.SentDataSize, SocketFlags.None, new AsyncCallback(this.OnSent), socketObject);
				}
			}
			catch (Exception e)
			{
				Log.Warning<IPEndPoint>("Can't send data to {0}.", socketObject.EndPoint);
				Log.Warning(e);
				Log.Warning("Disconnect socket.");
				try
				{
					socketObject.Socket.Disconnect(true);
				}
				catch
				{
				}
				object sockets2 = this.m_sockets;
				lock (sockets2)
				{
					this.m_sockets.Remove(socketObject);
					this.CheckAllSocketsSentData();
				}
				this.Connect(socketObject);
			}
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x00015FFC File Offset: 0x000143FC
		private bool CheckAllSocketsSentData()
		{
			bool flag = this.m_sockets.Count != 0 && this.m_sockets.Count == this.m_sentSocketCount;
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
					Thread.Sleep(100);
				}
				for (int i = 0; i < this.m_sockets.Count; i++)
				{
					TcpLogSender.SocketObject socketObject = this.m_sockets[i];
					try
					{
						socketObject.Socket.BeginSend(this.m_buffer, 0, this.m_bufferPosition, SocketFlags.None, new AsyncCallback(this.OnSent), socketObject);
					}
					catch (Exception e)
					{
						Log.Warning<IPEndPoint>("Can't send data to {0}.", socketObject.EndPoint);
						Log.Warning(e);
						Log.Warning("Disconnect socket.");
						try
						{
							socketObject.Socket.Disconnect(true);
						}
						catch
						{
						}
						this.m_sockets.RemoveAt(i--);
						this.Connect(socketObject);
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x04000216 RID: 534
		private int m_sentSocketCount;

		// Token: 0x04000217 RID: 535
		private readonly List<TcpLogSender.SocketObject> m_sockets;
	}
}
