using System;
using System.Collections.Generic;
using System.IO;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.GameRoomSystem;
using StatsDataSource.Storage;

namespace MasterServer.Telemetry
{
	// Token: 0x02000730 RID: 1840
	[Service]
	[Singleton]
	internal class TelemetryStreamService : ServiceModule, ITelemetryStreamService, IDebugTelemetryStreamService
	{
		// Token: 0x06002615 RID: 9749 RVA: 0x000A10C8 File Offset: 0x0009F4C8
		public TelemetryStreamService(ITelemetryService telemetryService, ISessionStorage sessionStorage, IGameRoomManager gameRoomManager)
		{
			this.m_telemetryService = telemetryService;
			this.m_sessionStorage = sessionStorage;
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x140000A2 RID: 162
		// (add) Token: 0x06002616 RID: 9750 RVA: 0x000A10E8 File Offset: 0x0009F4E8
		// (remove) Token: 0x06002617 RID: 9751 RVA: 0x000A1120 File Offset: 0x0009F520
		public event OnSessionTelemetryDeleg OnSessionTelemetry;

		// Token: 0x06002618 RID: 9752 RVA: 0x000A1156 File Offset: 0x0009F556
		public override void Init()
		{
			base.Init();
			this.m_gameRoomManager.SessionStarted += this.OnSessionStarted;
		}

		// Token: 0x06002619 RID: 9753 RVA: 0x000A1175 File Offset: 0x0009F575
		public override void Stop()
		{
			base.Stop();
			this.m_gameRoomManager.SessionStarted -= this.OnSessionStarted;
		}

		// Token: 0x0600261A RID: 9754 RVA: 0x000A1194 File Offset: 0x0009F594
		public bool Process(TelemetryStreamService.StreamPacket packet)
		{
			Log.Verbose(Log.Group.SessionTelemetry, "Session '{0}' incoming telemetry packet {1}", new object[]
			{
				packet.SessionID,
				packet.PacketID
			});
			TelemetryStreamService.SessionData data = this.m_sessionStorage.GetData<TelemetryStreamService.SessionData>(packet.SessionID, ESessionData.Telemetry);
			if (data == null)
			{
				Log.Warning<string>("Discarding telemetry packet for invalid session '{0}'", packet.SessionID);
				return false;
			}
			if (packet.PacketID != data.CurrentPacket + 1)
			{
				Log.Warning<int, int, string>("Out of sequence packet id {0} (expected {1}) in session {2}, discarding data", packet.PacketID, data.CurrentPacket + 1, packet.SessionID);
				data.Discard();
				this.m_sessionStorage.RemoveData(packet.SessionID, ESessionData.Telemetry);
				return false;
			}
			try
			{
				data.AddPacket(packet);
				if (packet.IsFinal)
				{
					Log.Verbose(Log.Group.SessionTelemetry, "Session '{0}' finalizing telemetry on packet {1}", new object[]
					{
						packet.SessionID,
						packet.PacketID
					});
					this.SessionDone(data);
					this.m_sessionStorage.RemoveData(packet.SessionID, ESessionData.Telemetry);
				}
			}
			catch
			{
				Log.Warning<int, string>("Failed to apply packet {0} to session '{1}', discarding data", packet.PacketID, packet.SessionID);
				data.Discard();
				this.m_sessionStorage.RemoveData(packet.SessionID, ESessionData.Telemetry);
				return false;
			}
			return true;
		}

		// Token: 0x0600261B RID: 9755 RVA: 0x000A12E4 File Offset: 0x0009F6E4
		private void SessionDone(TelemetryStreamService.SessionData sessionData)
		{
			this.m_telemetryService.StatsProcessor.QueueStreamProcessing(sessionData.StreamFile, sessionData.SessionFile);
			sessionData.StreamFile = null;
			List<DataUpdate> telemetry = sessionData.Repository.DataProcessor.FinalizeAccumulation();
			this.FireOnSessionTelemetryEvent(sessionData, telemetry);
		}

		// Token: 0x0600261C RID: 9756 RVA: 0x000A1330 File Offset: 0x0009F730
		private void FireOnSessionTelemetryEvent(TelemetryStreamService.SessionData sessionData, List<DataUpdate> telemetry)
		{
			try
			{
				this.OnSessionTelemetry(sessionData, telemetry);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x0600261D RID: 9757 RVA: 0x000A136C File Offset: 0x0009F76C
		private void OnSessionStarted(IGameRoom room, string sessionId)
		{
			string streamFile = this.GetStreamFile(room.ID, sessionId);
			string sessionFile = this.GetSessionFile(Path.GetFileNameWithoutExtension(streamFile));
			TelemetryStreamService.SessionData data = new TelemetryStreamService.SessionData(this.m_telemetryService.StatsProcessor.GetRegistry(), sessionId, streamFile, sessionFile);
			this.m_sessionStorage.AddData(sessionId, ESessionData.Telemetry, data);
			Log.Verbose(Log.Group.SessionTelemetry, "Starting room {0} telemetry session '{1}'", new object[]
			{
				room.ID,
				sessionId
			});
		}

		// Token: 0x0600261E RID: 9758 RVA: 0x000A13E0 File Offset: 0x0009F7E0
		private string GetStreamFile(ulong roomId, string sessionId)
		{
			string path = string.Format("{0}_{1}_{2}.tlm", Resources.ServerName, roomId, sessionId);
			return Path.Combine(Resources.TelemStreamDir, path);
		}

		// Token: 0x0600261F RID: 9759 RVA: 0x000A1410 File Offset: 0x0009F810
		private string GetSessionFile(string sessionId)
		{
			return string.Format("Game_{0}_{1}.xml", DateTime.Now.ToString("yy-MM-dd-HHmmss"), sessionId);
		}

		// Token: 0x06002620 RID: 9760 RVA: 0x000A143A File Offset: 0x0009F83A
		public void SimulateTelemetryEvent(TelemetryStreamService.SessionData sessionData, List<DataUpdate> telemetry)
		{
			this.FireOnSessionTelemetryEvent(sessionData, telemetry);
		}

		// Token: 0x0400138C RID: 5004
		private readonly ITelemetryService m_telemetryService;

		// Token: 0x0400138D RID: 5005
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x0400138E RID: 5006
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x02000731 RID: 1841
		public class StreamPacket
		{
			// Token: 0x0400138F RID: 5007
			public string SessionID;

			// Token: 0x04001390 RID: 5008
			public int PacketID;

			// Token: 0x04001391 RID: 5009
			public bool IsFinal;

			// Token: 0x04001392 RID: 5010
			public string Content;
		}

		// Token: 0x02000732 RID: 1842
		public class SessionData : IDisposable
		{
			// Token: 0x06002622 RID: 9762 RVA: 0x000A144C File Offset: 0x0009F84C
			public SessionData(StatsRegistry registry, string sessionId, string streamFile, string sessionFile)
			{
				this.SessionID = sessionId;
				this.Repository = new StatsRepository(registry);
				this.CurrentPacket = -1;
				this.StreamFile = streamFile;
				this.SessionFile = sessionFile;
			}

			// Token: 0x06002623 RID: 9763 RVA: 0x000A147D File Offset: 0x0009F87D
			public void Dispose()
			{
				if (!string.IsNullOrEmpty(this.StreamFile) && this.CurrentPacket >= 0)
				{
					Log.Warning<string>("Discarding telemetry data for session '{0}'", this.SessionID);
					this.Discard();
				}
			}

			// Token: 0x06002624 RID: 9764 RVA: 0x000A14B4 File Offset: 0x0009F8B4
			public void AddPacket(TelemetryStreamService.StreamPacket packet)
			{
				if (packet.PacketID != this.CurrentPacket + 1)
				{
					throw new Exception(string.Format("Out of sequence telemetry packet {0}, expecting {1}", packet.PacketID, this.CurrentPacket + 1));
				}
				try
				{
					using (StringReader stringReader = new StringReader(packet.Content))
					{
						using (TelemetryStreamParser telemetryStreamParser = new TelemetryStreamParser(this.Repository, stringReader))
						{
							telemetryStreamParser.Parse();
						}
					}
					this.CurrentPacket++;
				}
				catch (Exception ex)
				{
					Log.Error<string, string, string>("Failed to parse telemetry packet: {0}\n{1}\nContent: {2}", ex.Message, ex.StackTrace, packet.Content);
					throw;
				}
				this.Repository.DataProcessor.DropProcessedEvents();
				File.AppendAllText(this.StreamFile, packet.Content);
			}

			// Token: 0x06002625 RID: 9765 RVA: 0x000A15BC File Offset: 0x0009F9BC
			public void Discard()
			{
				if (File.Exists(this.StreamFile))
				{
					File.Delete(this.StreamFile);
					this.StreamFile = null;
				}
			}

			// Token: 0x04001393 RID: 5011
			public string SessionID;

			// Token: 0x04001394 RID: 5012
			public StatsRepository Repository;

			// Token: 0x04001395 RID: 5013
			public int CurrentPacket;

			// Token: 0x04001396 RID: 5014
			public string StreamFile;

			// Token: 0x04001397 RID: 5015
			public string SessionFile;
		}
	}
}
