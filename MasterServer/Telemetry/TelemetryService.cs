using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.Database;
using MasterServer.GameLogic.PlayerStats;
using MasterServer.Telemetry.Metrics;
using MasterServer.Users;
using OLAPHypervisor;
using StatsDataSource.Storage;

namespace MasterServer.Telemetry
{
	// Token: 0x020007D1 RID: 2001
	[Service]
	[Singleton]
	internal class TelemetryService : ServiceModule, ITelemetryService
	{
		// Token: 0x060028F3 RID: 10483 RVA: 0x000B18CC File Offset: 0x000AFCCC
		public TelemetryService(ITelemetryDALService telemetryDALService, IUserRepository userRepository, IPlayerStatsService playerStatsService, IServerRepository serverRepository, IDALService dal, IProcessingQueueMetricsTracker processingQueueMetricsTracker)
		{
			this.m_telemetryDALService = telemetryDALService;
			this.m_userRepository = userRepository;
			this.m_playerStatsService = playerStatsService;
			this.m_serverRepository = serverRepository;
			this.m_dal = dal;
			this.m_processingQueueMetricsTracker = processingQueueMetricsTracker;
		}

		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x060028F4 RID: 10484 RVA: 0x000B1909 File Offset: 0x000AFD09
		// (set) Token: 0x060028F5 RID: 10485 RVA: 0x000B1911 File Offset: 0x000AFD11
		public TelemetryMode Mode
		{
			get
			{
				return this.m_mode;
			}
			set
			{
				this.m_mode = value;
			}
		}

		// Token: 0x060028F6 RID: 10486 RVA: 0x000B191C File Offset: 0x000AFD1C
		public override void Start()
		{
			this.m_deferredStream = new DeferredTelemetryStream(this);
			this.m_processTracker = new ProcessTracker(this);
			this.m_serverObserver = new ServerTrackerObserver(this, this.m_serverRepository);
			this.m_userTracker = new UserPresenceTracker(this, this.m_userRepository, this.m_playerStatsService, this.m_serverRepository, this.m_dal);
			this.m_mmTracker = new MatchmakingTracker(this);
			this.m_statsProc = new StatsProcessor(this, this.m_processingQueueMetricsTracker);
		}

		// Token: 0x060028F7 RID: 10487 RVA: 0x000B1998 File Offset: 0x000AFD98
		public override void Stop()
		{
			if (this.m_processTracker != null)
			{
				this.m_processTracker.Dispose();
				this.m_processTracker = null;
			}
			if (this.m_statsProc != null)
			{
				this.m_statsProc.Dispose();
				this.m_statsProc = null;
			}
			if (this.m_userTracker != null)
			{
				this.m_userTracker.Dispose();
				this.m_userTracker = null;
			}
			if (this.m_mmTracker != null)
			{
				this.m_mmTracker.Dispose();
				this.m_mmTracker = null;
			}
			if (this.m_deferredStream != null)
			{
				this.m_deferredStream.Dispose();
				this.m_deferredStream = null;
			}
		}

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x060028F8 RID: 10488 RVA: 0x000B1A36 File Offset: 0x000AFE36
		public StatsProcessor StatsProcessor
		{
			get
			{
				return this.m_statsProc;
			}
		}

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x060028F9 RID: 10489 RVA: 0x000B1A3E File Offset: 0x000AFE3E
		public DeferredTelemetryStream DeferredStream
		{
			get
			{
				return this.m_deferredStream;
			}
		}

		// Token: 0x060028FA RID: 10490 RVA: 0x000B1A46 File Offset: 0x000AFE46
		public bool CheckMode(TelemetryMode mode)
		{
			return this.m_mode != TelemetryMode.Disabled && (this.m_mode & mode) == mode;
		}

		// Token: 0x060028FB RID: 10491 RVA: 0x000B1A61 File Offset: 0x000AFE61
		public void RunAggregation()
		{
			this.m_telemetryDALService.TelemetrySystem.RunAggregation();
		}

		// Token: 0x060028FC RID: 10492 RVA: 0x000B1A73 File Offset: 0x000AFE73
		public void AddMeasure(Measure msr)
		{
			this.AddMeasure(new Measure[]
			{
				msr
			});
		}

		// Token: 0x060028FD RID: 10493 RVA: 0x000B1A8E File Offset: 0x000AFE8E
		public void AddMeasure(IEnumerable<Measure> msrs)
		{
			if (base.State == ServiceState.Started)
			{
				this.m_telemetryDALService.TelemetrySystem.AddMeasure(msrs);
			}
		}

		// Token: 0x060028FE RID: 10494 RVA: 0x000B1AAD File Offset: 0x000AFEAD
		public void AddMeasure(long value, params object[] args)
		{
			if (base.State == ServiceState.Started)
			{
				this.AddMeasure(this.MakeMeasure(value, args));
			}
		}

		// Token: 0x060028FF RID: 10495 RVA: 0x000B1AC9 File Offset: 0x000AFEC9
		public Measure MakeMeasure(long value, params object[] args)
		{
			return TelemetryService.MakeMeasureSt(value, args);
		}

		// Token: 0x06002900 RID: 10496 RVA: 0x000B1AD4 File Offset: 0x000AFED4
		public static Measure MakeMeasureSt(long value, params object[] args)
		{
			if (args.Length % 2 != 0)
			{
				throw new ServiceModuleException("Invalid number of parameters to build dimensions");
			}
			SortedList<string, string> sortedList = new SortedList<string, string>();
			for (int i = 0; i < args.Length; i += 2)
			{
				if (args[i + 1] != null)
				{
					sortedList.Add(args[i].ToString(), args[i + 1].ToString());
				}
			}
			return new Measure
			{
				Dimensions = sortedList,
				Value = value,
				RowCount = 1L
			};
		}

		// Token: 0x06002901 RID: 10497 RVA: 0x000B1B54 File Offset: 0x000AFF54
		public Measure[] MeasureFromUpdates(List<DataUpdate> updates)
		{
			return TelemetryService.MeasureFromUpdatesSt(updates);
		}

		// Token: 0x06002902 RID: 10498 RVA: 0x000B1B5C File Offset: 0x000AFF5C
		internal static Measure[] MeasureFromUpdatesSt(List<DataUpdate> updates)
		{
			Measure[] array = new Measure[updates.Count];
			for (int num = 0; num != updates.Count; num++)
			{
				array[num] = default(Measure);
				array[num].Dimensions = updates[num].Dimensions;
				array[num].Value = (long)double.Parse(updates[num].Value);
				array[num].AggregateOp = EAggOperation.Sum;
				array[num].RowCount = 1L;
			}
			return array;
		}

		// Token: 0x040015D0 RID: 5584
		private ProcessTracker m_processTracker;

		// Token: 0x040015D1 RID: 5585
		private ServerTrackerObserver m_serverObserver;

		// Token: 0x040015D2 RID: 5586
		private UserPresenceTracker m_userTracker;

		// Token: 0x040015D3 RID: 5587
		private MatchmakingTracker m_mmTracker;

		// Token: 0x040015D4 RID: 5588
		private DeferredTelemetryStream m_deferredStream;

		// Token: 0x040015D5 RID: 5589
		private StatsProcessor m_statsProc;

		// Token: 0x040015D6 RID: 5590
		private TelemetryMode m_mode = TelemetryMode.Enabled;

		// Token: 0x040015D7 RID: 5591
		private readonly ITelemetryDALService m_telemetryDALService;

		// Token: 0x040015D8 RID: 5592
		private readonly IUserRepository m_userRepository;

		// Token: 0x040015D9 RID: 5593
		private readonly IPlayerStatsService m_playerStatsService;

		// Token: 0x040015DA RID: 5594
		private readonly IServerRepository m_serverRepository;

		// Token: 0x040015DB RID: 5595
		private readonly IDALService m_dal;

		// Token: 0x040015DC RID: 5596
		private readonly IProcessingQueueMetricsTracker m_processingQueueMetricsTracker;
	}
}
