using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x02000391 RID: 913
	[Job("graceful_auto_shutdown")]
	internal class GracefulAutoShutdownJob : Job
	{
		// Token: 0x0600146F RID: 5231 RVA: 0x00052F08 File Offset: 0x00051308
		public GracefulAutoShutdownJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, IJobMetricsTracker metricsService, IApplicationService appService, IUserRepository userRepository) : base(jobConfigSection, jobScheduler, metricsService)
		{
			this.m_appService = appService;
			this.m_userRepository = userRepository;
			foreach (string v in jobConfigSection.Get("channel").Split(new char[]
			{
				','
			}))
			{
				this.m_channels.Add(ReflectionUtils.EnumParse<Resources.ChannelType>(v));
			}
			jobConfigSection.Get("online_less", out this.m_online);
			int num;
			jobConfigSection.Get("timeout_sec", out num);
			this.m_timeout = TimeSpan.FromSeconds((double)num);
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x00052FAC File Offset: 0x000513AC
		protected override JobResult ExecuteImpl()
		{
			if (this.m_channels.Any((Resources.ChannelType e) => e == Resources.Channel) && this.m_userRepository.GetOnlineUsersCount() < this.m_online)
			{
				this.m_appService.ScheduleShutdown(this.m_timeout);
			}
			return JobResult.Finished;
		}

		// Token: 0x06001471 RID: 5233 RVA: 0x0005300E File Offset: 0x0005140E
		protected override void OnConfigChaged(ConfigEventArgs args)
		{
			base.OnConfigChaged(args);
			if (string.Equals(args.Name, "online_less", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_online = args.iValue;
			}
		}

		// Token: 0x04000983 RID: 2435
		public const string GRACEFUL_AUTO_SHUTDOWN_JOB_NAME = "graceful_auto_shutdown";

		// Token: 0x04000984 RID: 2436
		private readonly TimeSpan m_timeout;

		// Token: 0x04000985 RID: 2437
		private readonly List<Resources.ChannelType> m_channels = new List<Resources.ChannelType>();

		// Token: 0x04000986 RID: 2438
		private readonly IApplicationService m_appService;

		// Token: 0x04000987 RID: 2439
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000988 RID: 2440
		private int m_online;
	}
}
