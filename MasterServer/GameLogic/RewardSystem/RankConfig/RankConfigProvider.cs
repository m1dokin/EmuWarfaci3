using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Configuration;
using MasterServer.GameLogic.RewardSystem.RankConfig.Deserializers;
using Util.Common;

namespace MasterServer.GameLogic.RewardSystem.RankConfig
{
	// Token: 0x020000D6 RID: 214
	[Service]
	[Singleton]
	public class RankConfigProvider : ServiceModule, IConfigProvider<ChannelRankConfig>
	{
		// Token: 0x06000375 RID: 885 RVA: 0x0000FAC8 File Offset: 0x0000DEC8
		public RankConfigProvider(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
			IEnumerable<ConfigSection> rankRestrictions = this.GetRankRestrictions();
			foreach (ConfigSection configSection in rankRestrictions)
			{
				configSection.OnConfigChanged += this.OnConfigChanged;
			}
			this.m_rankConfig = this.GetConfig();
		}

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x06000376 RID: 886 RVA: 0x0000FB48 File Offset: 0x0000DF48
		// (remove) Token: 0x06000377 RID: 887 RVA: 0x0000FB80 File Offset: 0x0000DF80
		public event Action<ChannelRankConfig> Changed;

		// Token: 0x06000378 RID: 888 RVA: 0x0000FBB8 File Offset: 0x0000DFB8
		public override void Stop()
		{
			IEnumerable<ConfigSection> rankRestrictions = this.GetRankRestrictions();
			foreach (ConfigSection configSection in rankRestrictions)
			{
				configSection.OnConfigChanged -= this.OnConfigChanged;
			}
			base.Stop();
		}

		// Token: 0x06000379 RID: 889 RVA: 0x0000FC24 File Offset: 0x0000E024
		public ChannelRankConfig Get()
		{
			return this.m_rankConfig;
		}

		// Token: 0x0600037A RID: 890 RVA: 0x0000FC2C File Offset: 0x0000E02C
		private ChannelRankConfig GetConfig()
		{
			XmlDocument experienceCurveXml = this.GetExperienceCurveXml();
			IEnumerable<ConfigSection> rankRestrictions = this.GetRankRestrictions();
			ConfigSection newbieProtectionRankClustering = this.GetNewbieProtectionRankClustering();
			ExperienceCurveDeserializer experienceCurveDeserializer = new ExperienceCurveDeserializer();
			RankRestrictionsDeserializer rankRestrictionsDeserializer = new RankRestrictionsDeserializer();
			NewbieProtectionRankClusteringDeserializer newbieProtectionRankClusteringDeserializer = new NewbieProtectionRankClusteringDeserializer();
			List<ulong> list = experienceCurveDeserializer.Deserialize(experienceCurveXml);
			uint globalMaxRank = (uint)list.Count<ulong>();
			Dictionary<Resources.ChannelType, ChannelRankRestriction> dictionary = rankRestrictionsDeserializer.Deserialize(rankRestrictions, globalMaxRank);
			NewbieProtectionRankClustering newbieProtectionRankClustering2 = newbieProtectionRankClusteringDeserializer.Deserialize(newbieProtectionRankClustering);
			RankRestrictionsValidator rankRestrictionsValidator = new RankRestrictionsValidator();
			rankRestrictionsValidator.Validate(dictionary, globalMaxRank);
			ChannelRankConfigBuilder channelRankConfigBuilder = new ChannelRankConfigBuilder();
			channelRankConfigBuilder.SetExpCurve(list);
			channelRankConfigBuilder.SetGlobalMaxRank(globalMaxRank);
			channelRankConfigBuilder.SetNewbieProtectionRankClustering(newbieProtectionRankClustering2);
			channelRankConfigBuilder.SetRankRestrictions(dictionary);
			return channelRankConfigBuilder.BuildForChannel(Resources.Channel);
		}

		// Token: 0x0600037B RID: 891 RVA: 0x0000FCD8 File Offset: 0x0000E0D8
		private void OnConfigChanged(ConfigEventArgs args)
		{
			ChannelRankConfig config = this.GetConfig();
			Interlocked.Exchange<ChannelRankConfig>(ref this.m_rankConfig, config);
			this.Changed.SafeInvokeEach(config);
		}

		// Token: 0x0600037C RID: 892 RVA: 0x0000FD08 File Offset: 0x0000E108
		private XmlDocument GetExperienceCurveXml()
		{
			string filename = Path.Combine(Resources.GetResourcesDirectory(), "exp_curve.xml");
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(filename);
			return xmlDocument;
		}

		// Token: 0x0600037D RID: 893 RVA: 0x0000FD34 File Offset: 0x0000E134
		private IEnumerable<ConfigSection> GetRankRestrictions()
		{
			Config config = this.m_configurationService.GetConfig(ConfigInfo.ModuleConfiguration);
			ConfigSection section = config.GetSection("ChannelRestrictions");
			if (section == null)
			{
				return Enumerable.Empty<ConfigSection>();
			}
			Dictionary<string, List<ConfigSection>> allSections = section.GetAllSections();
			return allSections.SelectMany((KeyValuePair<string, List<ConfigSection>> x) => x.Value);
		}

		// Token: 0x0600037E RID: 894 RVA: 0x0000FD98 File Offset: 0x0000E198
		private ConfigSection GetNewbieProtectionRankClustering()
		{
			Config config = this.m_configurationService.GetConfig(ConfigInfo.ModuleConfiguration);
			return config.GetSection("NewbieProtection").GetSection("RankClustering");
		}

		// Token: 0x04000175 RID: 373
		private readonly IConfigurationService m_configurationService;

		// Token: 0x04000176 RID: 374
		private ChannelRankConfig m_rankConfig;
	}
}
