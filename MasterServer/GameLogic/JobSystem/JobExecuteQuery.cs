using System;
using System.Xml;
using MasterServer.Core.Services.Jobs;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.JobSystem
{
	// Token: 0x02000392 RID: 914
	[QueryAttributes(TagName = "job_execute")]
	internal class JobExecuteQuery : BaseQuery
	{
		// Token: 0x06001473 RID: 5235 RVA: 0x00053043 File Offset: 0x00051443
		public JobExecuteQuery(IJobSchedulerService jobScheduler, IOnlineClient onlineClientService)
		{
			this.m_jobScheduler = jobScheduler;
			this.m_routeSender = string.Format("k01.{0}.job", onlineClientService.XmppHost);
		}

		// Token: 0x06001474 RID: 5236 RVA: 0x00053068 File Offset: 0x00051468
		public override int QueryGetResponse(string from, XmlElement request, XmlElement response)
		{
			if (from != this.m_routeSender)
			{
				throw new ApplicationException(string.Format("Message 'job_execute' can't be sent from {0}", from));
			}
			string attribute = request.GetAttribute("job_name");
			this.m_jobScheduler.AddJob(attribute);
			return 0;
		}

		// Token: 0x0400098A RID: 2442
		private readonly string m_routeSender;

		// Token: 0x0400098B RID: 2443
		private readonly IJobSchedulerService m_jobScheduler;
	}
}
