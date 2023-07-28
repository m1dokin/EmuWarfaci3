using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MasterServer.Core.Configuration;
using MasterServer.Core.Timers;
using MasterServer.CryOnlineNET;
using MasterServer.Users;
using Ninject;

namespace MasterServer.Core
{
	// Token: 0x02000112 RID: 274
	internal class GracefulShutdownContext : IDisposable
	{
		// Token: 0x06000463 RID: 1123 RVA: 0x00013350 File Offset: 0x00011750
		public GracefulShutdownContext(IQueryManager queryManager, IUserRepository userRepository, [Named("NonReentrant")] ITimerFactory timerFactory, TimeSpan timeout)
		{
			this.m_queryManager = queryManager;
			this.m_userRepository = userRepository;
			this.m_timerFactory = timerFactory;
			this.SetupShutdownConfig(timeout);
			Log.Info("[ApplicationService] Dropping max_online_users to 0");
			Resources.QoSSettings.GetSection("limits").Set("max_online_users", 0);
			this.ScheduleShutdownTimer();
			string a = Resources.CommonSettings.GetSection("shutdown").Get("autorotate");
			if (a != "0")
			{
				this.ScheduleAutorotateTimer();
			}
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x000133EC File Offset: 0x000117EC
		private void ScheduleAutorotateTimer()
		{
			if (this.m_autorotateTimer == null)
			{
				this.m_autorotatedClients = new HashSet<ulong>();
				this.m_autorotateTimer = this.m_timerFactory.CreateTimer(new TimerCallback(this.AutorotateTick), null, GracefulShutdownContext.AUTOROTATE_TICK, GracefulShutdownContext.AUTOROTATE_TICK);
			}
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x0001342C File Offset: 0x0001182C
		private void AutorotateTick(object obj)
		{
			List<string> list = new List<string>();
			foreach (UserInfo.User user in this.m_userRepository.GetUsersWithoutTouch((UserInfo.User u) => !this.m_autorotatedClients.Contains(u.ProfileID)))
			{
				Jid fakeJid = DebugCommandsJidHelper.GetFakeJid(user.ProfileID);
				if (fakeJid.ToString() != user.OnlineID)
				{
					list.Add(user.OnlineID);
				}
				this.m_autorotatedClients.Add(user.ProfileID);
				if (list.Count >= 25)
				{
					break;
				}
			}
			if (list.Count > 0)
			{
				this.m_queryManager.BroadcastRequest("autorotate", "k01.", list, new object[0]);
			}
			else
			{
				this.m_autorotatedClients = null;
				this.m_autorotateTimer.Dispose();
				this.m_autorotateTimer = null;
			}
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x00013530 File Offset: 0x00011930
		private void ScheduleShutdownTimer()
		{
			if (this.m_shutdownTracker == null)
			{
				this.m_shutdownTracker = this.m_timerFactory.CreateTimer(new TimerCallback(this.ShutdownTick), null, GracefulShutdownContext.SHUTDOWN_TICK, GracefulShutdownContext.SHUTDOWN_TICK);
			}
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x00013568 File Offset: 0x00011968
		private void ShutdownTick(object obj)
		{
			if (this.m_userRepository.GetOnlineUsersCount() == 0)
			{
				Log.Info("No online players left. Closing MasterServer...");
				ConsoleCmdManager.ExecuteCmd("quit");
			}
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x0001359C File Offset: 0x0001199C
		private void SetupShutdownConfig(TimeSpan timeout)
		{
			DateTime dateTime = DateTime.UtcNow + timeout;
			ConfigSection section = Resources.CommonSettings.GetSection("shutdown");
			if (section == null)
			{
				return;
			}
			IEnumerable<ConfigSection> enumerable = (from s in section.GetAllSections()
			where s.Key.EndsWith("phase", StringComparison.OrdinalIgnoreCase)
			select s.Value).SelectMany((List<ConfigSection> s) => s);
			foreach (ConfigSection configSection in enumerable)
			{
				string text;
				if (configSection.Get("queries", out text))
				{
					string[] queries = text.Split(new char[]
					{
						','
					}, StringSplitOptions.RemoveEmptyEntries);
					string text2;
					int num;
					if (timeout == TimeSpan.Zero || !configSection.Get("exec_before_shutdown_in", out text2) || string.IsNullOrEmpty(text2) || !int.TryParse(text2.TrimEnd(new char[]
					{
						'%'
					}), out num))
					{
						this.m_queryManager.UpdateQueryBlockingFlags(queries, true, EOnlineError.eOnlineError_ParseError);
					}
					else
					{
						DateTime d = (!text2.EndsWith("%")) ? ((timeout.TotalSeconds <= (double)num) ? dateTime : dateTime.Subtract(TimeSpan.FromSeconds((double)num))) : dateTime.Subtract(TimeSpan.FromSeconds(timeout.TotalSeconds * (double)num / 100.0));
						this.m_shutdownPhaseTimers.Add(this.m_timerFactory.CreateTimer(delegate(object _)
						{
							this.m_queryManager.UpdateQueryBlockingFlags(queries, true, EOnlineError.eOnlineError_ParseError);
						}, null, d - DateTime.UtcNow, TimeSpan.FromMilliseconds(-1.0)));
					}
				}
			}
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x000137CC File Offset: 0x00011BCC
		public void Dispose()
		{
			if (this.m_shutdownTracker != null)
			{
				this.m_shutdownTracker.Dispose();
			}
			if (this.m_autorotateTimer != null)
			{
				this.m_autorotateTimer.Dispose();
			}
			if (this.m_shutdownPhaseTimers == null || !this.m_shutdownPhaseTimers.Any<ITimer>())
			{
				return;
			}
			foreach (ITimer timer in this.m_shutdownPhaseTimers)
			{
				timer.Dispose();
			}
		}

		// Token: 0x040001E1 RID: 481
		public const string ShutdownConfigSectionName = "shutdown";

		// Token: 0x040001E2 RID: 482
		public const string BlockingPhaseConfigSectionEnding = "phase";

		// Token: 0x040001E3 RID: 483
		public const string ExecBeforeShutdownInAttributeName = "exec_before_shutdown_in";

		// Token: 0x040001E4 RID: 484
		public const string QueriesAttributeName = "queries";

		// Token: 0x040001E5 RID: 485
		public const string AutorotateAttributeName = "autorotate";

		// Token: 0x040001E6 RID: 486
		private const int AUTOROTATE_BATCH = 25;

		// Token: 0x040001E7 RID: 487
		private static readonly TimeSpan SHUTDOWN_TICK = TimeSpan.FromSeconds(1.0);

		// Token: 0x040001E8 RID: 488
		private static readonly TimeSpan AUTOROTATE_TICK = TimeSpan.FromSeconds(1.0);

		// Token: 0x040001E9 RID: 489
		private readonly IQueryManager m_queryManager;

		// Token: 0x040001EA RID: 490
		private readonly IUserRepository m_userRepository;

		// Token: 0x040001EB RID: 491
		private readonly ITimerFactory m_timerFactory;

		// Token: 0x040001EC RID: 492
		private HashSet<ulong> m_autorotatedClients;

		// Token: 0x040001ED RID: 493
		private ITimer m_shutdownTracker;

		// Token: 0x040001EE RID: 494
		private ITimer m_autorotateTimer;

		// Token: 0x040001EF RID: 495
		private readonly IList<ITimer> m_shutdownPhaseTimers = new List<ITimer>(2);
	}
}
