using System;
using System.Text;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Core.Configuration;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.ProfanityCheck;
using MasterServer.Telemetry.Metrics;

namespace MasterServer.Platform.ProfanityCheck
{
	// Token: 0x020006AD RID: 1709
	[Service]
	[Singleton]
	internal class XmppProfanityCheckService : ProfanityCheckService
	{
		// Token: 0x060023EC RID: 9196 RVA: 0x00096B8E File Offset: 0x00094F8E
		public XmppProfanityCheckService(IProfanityMetricsTracker profanityMetricsTracker, IQueryManager queryManager, IOnlineClient onlineClient) : base(profanityMetricsTracker)
		{
			this.m_queryManager = queryManager;
			this.m_onlineClient = onlineClient;
		}

		// Token: 0x060023ED RID: 9197 RVA: 0x00096BA5 File Offset: 0x00094FA5
		protected override void ReadConfigImpl(ConfigSection configSection)
		{
		}

		// Token: 0x060023EE RID: 9198 RVA: 0x00096BA8 File Offset: 0x00094FA8
		protected override ProfanityCheckResult CheckImpl(ProfanityCheckService.CheckType checkType, ulong userId, string userNickname, string str)
		{
			string receiver = string.Format("k01.{0}.profanity", this.m_onlineClient.XmppHost);
			Task<object> task = this.m_queryManager.RequestAsync("masterserver_profanity_check", receiver, new object[]
			{
				str
			});
			task.Wait();
			return (ProfanityCheckResult)task.Result;
		}

		// Token: 0x060023EF RID: 9199 RVA: 0x00096BFC File Offset: 0x00094FFC
		protected override ProfanityCheckResult FilterImpl(ProfanityCheckService.CheckType checkType, ulong userId, StringBuilder builder)
		{
			string receiver = string.Format("k01.{0}.profanity", this.m_onlineClient.XmppHost);
			Task<object> task = this.m_queryManager.RequestAsync("masterserver_profanity_filter", receiver, new object[]
			{
				builder.ToString()
			});
			task.Wait();
			ProfanityFilterResult profanityFilterResult = (ProfanityFilterResult)task.Result;
			if (profanityFilterResult.Result != ProfanityCheckResult.Succeeded)
			{
				builder.Replace(builder.ToString(), profanityFilterResult.Filtered);
			}
			return profanityFilterResult.Result;
		}

		// Token: 0x04001201 RID: 4609
		private readonly IQueryManager m_queryManager;

		// Token: 0x04001202 RID: 4610
		private readonly IOnlineClient m_onlineClient;
	}
}
