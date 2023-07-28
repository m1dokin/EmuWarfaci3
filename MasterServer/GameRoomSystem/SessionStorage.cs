using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.Core.Timers;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200061F RID: 1567
	[Service]
	[Singleton]
	internal class SessionStorage : ServiceModule, ISessionStorageDebug, ISessionStorage
	{
		// Token: 0x060021A6 RID: 8614 RVA: 0x0008A110 File Offset: 0x00088510
		public SessionStorage(IGameRoomManager roomManager, IServerRepository serverRepository)
		{
			this.m_gameRoomManager = roomManager;
			this.m_serverRepository = serverRepository;
		}

		// Token: 0x060021A7 RID: 8615 RVA: 0x0008A134 File Offset: 0x00088534
		public override void Init()
		{
			this.m_gameRoomManager.SessionStarted += this.RoomSessionStarted;
			this.m_gameRoomManager.SessionEnded += this.RoomSessionEnded;
			this.m_sweep_timer = new SafeTimer(new TimerCallback(this.doSweep), null, SessionStorage.SWEEP_TIMEOUT, SessionStorage.SWEEP_TIMEOUT);
		}

		// Token: 0x060021A8 RID: 8616 RVA: 0x0008A191 File Offset: 0x00088591
		public override void Stop()
		{
			this.m_sweep_timer.Dispose();
		}

		// Token: 0x060021A9 RID: 8617 RVA: 0x0008A1A0 File Offset: 0x000885A0
		public void AddData(string session_id, ESessionData type, object data)
		{
			if (string.IsNullOrEmpty(session_id))
			{
				Log.Warning<ESessionData>("Trying to add data {0} for unexisting session", type);
				return;
			}
			object sessions = this.m_sessions;
			lock (sessions)
			{
				SessionStorage.SessionData sessionObj = this.GetSessionObj(session_id);
				sessionObj.Data[type] = new SessionStorage.SessionDataChunk(data);
				Log.Verbose(Log.Group.SessionStorage, "Session data added {0}:{1}", new object[]
				{
					session_id,
					type
				});
			}
		}

		// Token: 0x060021AA RID: 8618 RVA: 0x0008A230 File Offset: 0x00088630
		private SessionStorage.SessionData GetSessionObj(string session_id)
		{
			SessionStorage.SessionData sessionData;
			if (!this.m_sessions.TryGetValue(session_id, out sessionData))
			{
				Log.Verbose(Log.Group.SessionStorage, "Session data created {0}", new object[]
				{
					session_id
				});
				sessionData = new SessionStorage.SessionData();
				this.m_sessions.Add(session_id, sessionData);
			}
			sessionData.LastTouched = DateTime.UtcNow;
			return sessionData;
		}

		// Token: 0x060021AB RID: 8619 RVA: 0x0008A288 File Offset: 0x00088688
		public void RemoveData(string session_id, ESessionData type)
		{
			if (string.IsNullOrEmpty(session_id))
			{
				Log.Warning<ESessionData>("Trying to remove data {0} for unexisting session", type);
				return;
			}
			object sessions = this.m_sessions;
			lock (sessions)
			{
				SessionStorage.SessionData sessionData = null;
				if (this.m_sessions.TryGetValue(session_id, out sessionData))
				{
					SessionStorage.SessionDataChunk sessionDataChunk = null;
					if (sessionData.Data.TryGetValue(type, out sessionDataChunk))
					{
						sessionData.Data.Remove(type);
						Log.Verbose(Log.Group.SessionStorage, "Session data removed {0}:{1}", new object[]
						{
							session_id,
							type
						});
						if (sessionDataChunk.Data is IDisposable)
						{
							((IDisposable)sessionDataChunk.Data).Dispose();
						}
					}
				}
			}
		}

		// Token: 0x060021AC RID: 8620 RVA: 0x0008A358 File Offset: 0x00088758
		public void RemoveAllData(string sessionId)
		{
			IEnumerator enumerator = Enum.GetValues(typeof(ESessionData)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					ESessionData type = (ESessionData)obj;
					this.RemoveData(sessionId, type);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		// Token: 0x060021AD RID: 8621 RVA: 0x0008A3C8 File Offset: 0x000887C8
		public T GetData<T>(string session_id, ESessionData type)
		{
			object sessions = this.m_sessions;
			T result;
			lock (sessions)
			{
				SessionStorage.SessionData sessionData;
				SessionStorage.SessionDataChunk sessionDataChunk;
				if (this.m_sessions.TryGetValue(session_id, out sessionData) && sessionData.Data.TryGetValue(type, out sessionDataChunk))
				{
					result = (T)((object)sessionDataChunk.Data);
				}
				else
				{
					result = default(T);
				}
			}
			return result;
		}

		// Token: 0x060021AE RID: 8622 RVA: 0x0008A44C File Offset: 0x0008884C
		public bool ValidateSession(string server_jid, string session_id)
		{
			string serverID = this.m_serverRepository.GetServerID(server_jid);
			if (string.IsNullOrEmpty(serverID))
			{
				return false;
			}
			object sessions = this.m_sessions;
			bool result;
			lock (sessions)
			{
				SessionStorage.SessionData sessionData;
				if (!this.m_sessions.TryGetValue(session_id, out sessionData))
				{
					result = false;
				}
				else if (sessionData.Finished && DateTime.UtcNow - sessionData.LastTouched >= SessionStorage.SESSION_VALID_TTL)
				{
					result = false;
				}
				else
				{
					result = (string.Compare(sessionData.ServerID, serverID) == 0);
				}
			}
			return result;
		}

		// Token: 0x060021AF RID: 8623 RVA: 0x0008A504 File Offset: 0x00088904
		private void RoomSessionStarted(IGameRoom room, string session_id)
		{
			object sessions = this.m_sessions;
			lock (sessions)
			{
				SessionStorage.SessionData sessionObj = this.GetSessionObj(session_id);
				ServerExtension extension = room.GetExtension<ServerExtension>();
				sessionObj.ServerID = extension.ServerID;
				Log.Verbose(Log.Group.SessionStorage, "Session data server set {0}:{1}", new object[]
				{
					session_id,
					sessionObj.ServerID
				});
			}
		}

		// Token: 0x060021B0 RID: 8624 RVA: 0x0008A580 File Offset: 0x00088980
		private void RoomSessionEnded(IGameRoom room, string session_id, bool abnormal)
		{
			object sessions = this.m_sessions;
			lock (sessions)
			{
				SessionStorage.SessionData sessionData;
				if (this.m_sessions.TryGetValue(session_id, out sessionData))
				{
					if (!abnormal)
					{
						sessionData.LastTouched = DateTime.UtcNow;
						sessionData.Finished = true;
						Log.Verbose(Log.Group.SessionStorage, "Session data marked finished {0}", new object[]
						{
							session_id
						});
					}
					else
					{
						Log.Warning<string, string>("[SessionStorage] Session {0} terminated abnormally. SessionStorage state: {1}", session_id, this.DataKeysStr(sessionData.Data.Keys));
					}
				}
			}
		}

		// Token: 0x060021B1 RID: 8625 RVA: 0x0008A624 File Offset: 0x00088A24
		private void doSweep(object dummy)
		{
			object sessions = this.m_sessions;
			lock (sessions)
			{
				List<string> list = new List<string>();
				DateTime utcNow = DateTime.UtcNow;
				foreach (KeyValuePair<string, SessionStorage.SessionData> keyValuePair in this.m_sessions)
				{
					if (utcNow - keyValuePair.Value.LastTouched >= SessionStorage.SESSION_SANITY_TTL)
					{
						Log.Warning<string, DateTime, string>("Disposing data of session '{0}' by sanity timeout exceeded (last touch {1} UTC). Stale data: {2}", keyValuePair.Key, keyValuePair.Value.LastTouched, this.DataKeysStr(keyValuePair.Value.Data.Keys));
						list.Add(keyValuePair.Key);
						Log.Verbose(Log.Group.SessionStorage, "Session data deleted {0} by SANITY timeout", new object[]
						{
							keyValuePair.Key
						});
					}
					else if (keyValuePair.Value.Finished && utcNow - keyValuePair.Value.LastTouched >= SessionStorage.SESSION_FINISHED_TTL)
					{
						Log.Warning<string, DateTime, string>("Disposing data of session '{0}' by finished session timeout (last touch {1} UTC). Stale data: {2}", keyValuePair.Key, keyValuePair.Value.LastTouched, this.DataKeysStr(keyValuePair.Value.Data.Keys));
						list.Add(keyValuePair.Key);
						Log.Verbose(Log.Group.SessionStorage, "Session data deleted {0} by FINISHED timeout", new object[]
						{
							keyValuePair.Key
						});
					}
					else if (keyValuePair.Value.Finished && utcNow - keyValuePair.Value.LastTouched >= SessionStorage.SESSION_VALID_TTL && keyValuePair.Value.Data.Count == 0)
					{
						list.Add(keyValuePair.Key);
						Log.Verbose(Log.Group.SessionStorage, "Session data deleted {0} normaly", new object[]
						{
							keyValuePair.Key
						});
					}
				}
				foreach (string key in list)
				{
					SessionStorage.SessionData sessionData = this.m_sessions[key];
					this.m_sessions.Remove(key);
					sessionData.Dispose();
				}
			}
		}

		// Token: 0x060021B2 RID: 8626 RVA: 0x0008A8D0 File Offset: 0x00088CD0
		private string DataKeysStr(IEnumerable<ESessionData> keys)
		{
			return string.Join(", ", (from x in keys
			select x.ToString()).ToArray<string>());
		}

		// Token: 0x060021B3 RID: 8627 RVA: 0x0008A904 File Offset: 0x00088D04
		public void DbgDumpStorage()
		{
			StringBuilder stringBuilder = new StringBuilder();
			object sessions = this.m_sessions;
			lock (sessions)
			{
				foreach (KeyValuePair<string, SessionStorage.SessionData> keyValuePair in this.m_sessions)
				{
					string key = keyValuePair.Key;
					SessionStorage.SessionData value = keyValuePair.Value;
					stringBuilder.AppendFormat("'{0}' {1}: {2}\n", key, (!value.Finished) ? "in_progress" : "finished", string.Join(",", (from k in value.Data.Keys
					select k.ToString()).ToArray<string>()));
				}
			}
			Log.Info<string>("{0}", stringBuilder.ToString());
		}

		// Token: 0x04001053 RID: 4179
		private static readonly TimeSpan SESSION_VALID_TTL = TimeSpan.FromMinutes(1.0);

		// Token: 0x04001054 RID: 4180
		private static readonly TimeSpan SESSION_FINISHED_TTL = TimeSpan.FromMinutes(5.0);

		// Token: 0x04001055 RID: 4181
		private static readonly TimeSpan SESSION_SANITY_TTL = TimeSpan.FromHours(3.0);

		// Token: 0x04001056 RID: 4182
		private static readonly TimeSpan SWEEP_TIMEOUT = TimeSpan.FromSeconds(30.0);

		// Token: 0x04001057 RID: 4183
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04001058 RID: 4184
		private readonly IServerRepository m_serverRepository;

		// Token: 0x04001059 RID: 4185
		private SafeTimer m_sweep_timer;

		// Token: 0x0400105A RID: 4186
		private Dictionary<string, SessionStorage.SessionData> m_sessions = new Dictionary<string, SessionStorage.SessionData>();

		// Token: 0x02000620 RID: 1568
		private class SessionDataChunk
		{
			// Token: 0x060021B7 RID: 8631 RVA: 0x0008AA8B File Offset: 0x00088E8B
			public SessionDataChunk(object data)
			{
				this.Data = data;
			}

			// Token: 0x17000365 RID: 869
			// (get) Token: 0x060021B8 RID: 8632 RVA: 0x0008AA9A File Offset: 0x00088E9A
			// (set) Token: 0x060021B9 RID: 8633 RVA: 0x0008AAA2 File Offset: 0x00088EA2
			public object Data { get; private set; }
		}

		// Token: 0x02000621 RID: 1569
		private class SessionData : IDisposable
		{
			// Token: 0x060021BB RID: 8635 RVA: 0x0008AAC0 File Offset: 0x00088EC0
			public void Dispose()
			{
				foreach (SessionStorage.SessionDataChunk sessionDataChunk in this.Data.Values)
				{
					try
					{
						if (sessionDataChunk is IDisposable)
						{
							((IDisposable)sessionDataChunk).Dispose();
						}
					}
					catch (Exception e)
					{
						Log.Error("Error while disposing the expired session data");
						Log.Error(e);
					}
				}
			}

			// Token: 0x0400105E RID: 4190
			public string ServerID;

			// Token: 0x0400105F RID: 4191
			public Dictionary<ESessionData, SessionStorage.SessionDataChunk> Data = new Dictionary<ESessionData, SessionStorage.SessionDataChunk>();

			// Token: 0x04001060 RID: 4192
			public DateTime LastTouched;

			// Token: 0x04001061 RID: 4193
			public bool Finished;
		}
	}
}
