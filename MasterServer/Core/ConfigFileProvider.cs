using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core.Configs;
using MasterServer.Core.Services.Amqp;
using MasterServer.Core.Services.Configuration;

namespace MasterServer.Core
{
	// Token: 0x02000034 RID: 52
	[Service]
	[Singleton]
	internal class ConfigFileProvider : IConfigFileProvider
	{
		// Token: 0x060000CA RID: 202 RVA: 0x00007964 File Offset: 0x00005D64
		public IEnumerable<ConfigInfo> Get()
		{
			yield return ConfigInfo.ModuleConfiguration;
			yield return MsConfigInfo.OnlineVariablesConfiguration;
			yield return MsConfigInfo.CommonConfiguration;
			yield return MsConfigInfo.ECatalogConfiguration;
			yield return MsConfigInfo.XmppConfiguration;
			yield return MsConfigInfo.DbMasterConfiguration;
			yield return MsConfigInfo.DbSlaveConfiguration;
			yield return MsConfigInfo.RewardsConfiguration;
			yield return MsConfigInfo.QosConfiguration;
			yield return MsConfigInfo.LbConfiguration;
			yield return MsConfigInfo.AnticheatConfiguration;
			yield return MsConfigInfo.AbuseConfiguration;
			yield return MsConfigInfo.ProgressionConfiguration;
			yield return MsConfigInfo.SpecialRewardConfiguration;
			yield return MsConfigInfo.QuickplayConfiguration;
			yield return MsConfigInfo.CustomRulesConfiguration;
			yield return MsConfigInfo.GFaceConfiguration;
			yield return MsConfigInfo.BanRequestsConfiguration;
			yield return MsConfigInfo.RatingCurveConfiguration;
			yield return MsConfigInfo.MissionGenerationConfiguration;
			yield return AmqpConfigInfo.AmqpConfiguration;
			yield return AmqpConfigInfo.AmqpQosConfiguration;
			yield break;
		}
	}
}
