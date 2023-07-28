using System;
using HK2Net;
using HK2Net.Kernel;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200062F RID: 1583
	[Service]
	[Singleton]
	internal class SessionSummaryService : ServiceModule, ISessionSummaryService
	{
		// Token: 0x060021FE RID: 8702 RVA: 0x0008D690 File Offset: 0x0008BA90
		public SessionSummaryService(IContainer container, ISessionStorage sessionStorage, IGameRoomManager gameRoomManager)
		{
			this.m_container = container;
			this.m_sessionStorage = sessionStorage;
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x14000092 RID: 146
		// (add) Token: 0x060021FF RID: 8703 RVA: 0x0008D6B8 File Offset: 0x0008BAB8
		// (remove) Token: 0x06002200 RID: 8704 RVA: 0x0008D6F0 File Offset: 0x0008BAF0
		public event Action<SessionSummary> SessionSummaryFinalized;

		// Token: 0x06002201 RID: 8705 RVA: 0x0008D728 File Offset: 0x0008BB28
		public override void Init()
		{
			base.Init();
			this.m_gameRoomManager.SessionStarted += this.OnSessionStarted;
			this.m_gameRoomManager.SessionEnded += this.OnSessionEnded;
			this.m_sourceRewards = this.m_container.Create<SessionSummaryRewardsSource>();
			this.m_sourceTelem = this.m_container.Create<SessionSummaryTelemetrySource>();
			this.m_reporterLog = this.m_container.Create<SessionSummaryLogReporter>();
			this.m_reporterTelem = this.m_container.Create<SessionSummaryTelemetryReporter>();
		}

		// Token: 0x06002202 RID: 8706 RVA: 0x0008D7B0 File Offset: 0x0008BBB0
		public override void Stop()
		{
			base.Stop();
			this.SessionSummaryFinalized = null;
			if (this.m_sourceRewards != null)
			{
				this.m_sourceRewards.Dispose();
			}
			this.m_sourceRewards = null;
			if (this.m_sourceTelem != null)
			{
				this.m_sourceTelem.Dispose();
			}
			this.m_sourceTelem = null;
			this.m_reporterLog = null;
			this.m_reporterTelem = null;
			this.m_gameRoomManager.SessionStarted -= this.OnSessionStarted;
			this.m_gameRoomManager.SessionEnded -= this.OnSessionEnded;
		}

		// Token: 0x06002203 RID: 8707 RVA: 0x0008D840 File Offset: 0x0008BC40
		public void Contribute(string sessionId, string hint, Action<SessionSummary> contribution)
		{
			SessionSummary data = this.m_sessionStorage.GetData<SessionSummary>(sessionId, ESessionData.SessionSummary);
			if (data == null)
			{
				Log.Warning<string, string>("Ignoring '{0}' contribution for session {1}", hint, sessionId);
				return;
			}
			object obj = data;
			lock (obj)
			{
				try
				{
					contribution(data);
					this.FinalizeSummary(data);
				}
				catch (Exception e)
				{
					Log.Error<string>("Error in '{0}' contribution, discarding data", hint);
					Log.Error(e);
					this.m_sessionStorage.RemoveData(sessionId, ESessionData.SessionSummary);
				}
			}
		}

		// Token: 0x06002204 RID: 8708 RVA: 0x0008D8DC File Offset: 0x0008BCDC
		private void FinalizeSummary(SessionSummary data)
		{
			if (!data.TelemetryContributed || !data.RewardsContributed || data.EndTime == DateTime.MinValue)
			{
				return;
			}
			this.m_sessionStorage.RemoveData(data.SessionId, ESessionData.SessionSummary);
			try
			{
				if (this.SessionSummaryFinalized != null)
				{
					this.SessionSummaryFinalized(data);
				}
			}
			catch (Exception e)
			{
				Log.Error("Error while notifying of session summary");
				Log.Error(e);
			}
			object logLock = this.m_logLock;
			lock (logLock)
			{
				string sessionLogFileName = SessionSummaryService.GetSessionLogFileName();
				SessionsSummaryXMLSerializer.AppendSessionLog(sessionLogFileName, data);
				SessionsSummaryXMLSerializer.MoveOldLogsToHistory(sessionLogFileName);
			}
		}

		// Token: 0x06002205 RID: 8709 RVA: 0x0008D9A8 File Offset: 0x0008BDA8
		private static string GetSessionLogFileName()
		{
			string arg = DateTime.Now.ToString("yy-MM-dd");
			return string.Format("session_log_{0}_{1}.xml", arg, Resources.ServerName);
		}

		// Token: 0x06002206 RID: 8710 RVA: 0x0008D9D8 File Offset: 0x0008BDD8
		private void OnSessionStarted(IGameRoom room, string sessionId)
		{
			MissionExtension extension = room.GetExtension<MissionExtension>();
			MissionContext mission = extension.Mission;
			SessionSummary data = new SessionSummary(sessionId, room.ID, room.Type, room.RoomName, mission);
			this.m_sessionStorage.AddData(sessionId, ESessionData.SessionSummary, data);
		}

		// Token: 0x06002207 RID: 8711 RVA: 0x0008DA1C File Offset: 0x0008BE1C
		private void OnSessionEnded(IGameRoom room, string sessionId, bool abnormal)
		{
			SessionSummary data = this.m_sessionStorage.GetData<SessionSummary>(sessionId, ESessionData.SessionSummary);
			if (data == null)
			{
				Log.Warning<string>("Ignoring room summary finalization for session {0}", sessionId);
				return;
			}
			bool autobalance = room.Autobalance;
			object obj = data;
			lock (obj)
			{
				data.IsAutobalanced = autobalance;
				data.EndTime = DateTime.Now;
				this.FinalizeSummary(data);
			}
		}

		// Token: 0x040010B9 RID: 4281
		private readonly IContainer m_container;

		// Token: 0x040010BA RID: 4282
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x040010BB RID: 4283
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x040010BC RID: 4284
		private SessionSummaryRewardsSource m_sourceRewards;

		// Token: 0x040010BD RID: 4285
		private SessionSummaryTelemetrySource m_sourceTelem;

		// Token: 0x040010BE RID: 4286
		private SessionSummaryLogReporter m_reporterLog;

		// Token: 0x040010BF RID: 4287
		private SessionSummaryTelemetryReporter m_reporterTelem;

		// Token: 0x040010C0 RID: 4288
		private readonly object m_logLock = new object();
	}
}
