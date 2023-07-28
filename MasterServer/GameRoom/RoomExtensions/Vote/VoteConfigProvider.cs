using System;
using System.Collections.Generic;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.GameLogic.Configs;
using MasterServer.GameRoom.RoomExtensions.Vote.Exceptions;
using Util.Common;

namespace MasterServer.GameRoom.RoomExtensions.Vote
{
	// Token: 0x020004E8 RID: 1256
	[Service]
	[Singleton]
	internal class VoteConfigProvider : ServiceModule, IConfigProvider<VotesConfig>, IConfigProvider
	{
		// Token: 0x06001B05 RID: 6917 RVA: 0x0006E99D File Offset: 0x0006CD9D
		public VoteConfigProvider(IVoteConfigParser voteConfigParser)
		{
			this.m_voteConfigParser = voteConfigParser;
			this.m_config = new VotesConfig();
		}

		// Token: 0x14000067 RID: 103
		// (add) Token: 0x06001B06 RID: 6918 RVA: 0x0006E9B8 File Offset: 0x0006CDB8
		// (remove) Token: 0x06001B07 RID: 6919 RVA: 0x0006E9F0 File Offset: 0x0006CDF0
		public event Action<VotesConfig> Changed;

		// Token: 0x06001B08 RID: 6920 RVA: 0x0006EA26 File Offset: 0x0006CE26
		public override void Init()
		{
			this.m_configSection = this.GetVotesConfigSection();
			this.m_configSection.OnConfigChanged += this.OnConfigChanged;
			this.ParseConfigs();
		}

		// Token: 0x06001B09 RID: 6921 RVA: 0x0006EA51 File Offset: 0x0006CE51
		public override void Stop()
		{
			this.m_configSection.OnConfigChanged -= this.OnConfigChanged;
		}

		// Token: 0x06001B0A RID: 6922 RVA: 0x0006EA6A File Offset: 0x0006CE6A
		public VotesConfig Get()
		{
			return this.m_config;
		}

		// Token: 0x06001B0B RID: 6923 RVA: 0x0006EA72 File Offset: 0x0006CE72
		public int GetHash()
		{
			return this.m_configSection.GetHashCode();
		}

		// Token: 0x06001B0C RID: 6924 RVA: 0x0006EA80 File Offset: 0x0006CE80
		public IEnumerable<XmlElement> GetConfig(XmlDocument doc)
		{
			yield return (XmlElement)this.m_configSection.ToXmlNode(doc);
			yield break;
		}

		// Token: 0x06001B0D RID: 6925 RVA: 0x0006EAAA File Offset: 0x0006CEAA
		private void OnConfigChanged(ConfigEventArgs args)
		{
			this.ParseConfigs();
			this.Changed.SafeInvoke(this.m_config);
		}

		// Token: 0x06001B0E RID: 6926 RVA: 0x0006EAC4 File Offset: 0x0006CEC4
		private ConfigSection GetVotesConfigSection()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("Votes");
			if (section == null)
			{
				throw new NoVotesConfigException("Votes");
			}
			return section;
		}

		// Token: 0x06001B0F RID: 6927 RVA: 0x0006EAF4 File Offset: 0x0006CEF4
		private void ParseConfigs()
		{
			foreach (List<ConfigSection> list in this.m_configSection.GetAllSections().Values)
			{
				ConfigSection configSection = list[0];
				string name = configSection.Name;
				VoteType key;
				if (!Enum.TryParse<VoteType>(name, true, out key))
				{
					throw new InvalidVoteTypeException(name);
				}
				VoteConfig value = this.m_voteConfigParser.Parse(configSection);
				this.m_config[key] = value;
			}
		}

		// Token: 0x04000CE7 RID: 3303
		public const string VotesSection = "Votes";

		// Token: 0x04000CE8 RID: 3304
		private readonly IVoteConfigParser m_voteConfigParser;

		// Token: 0x04000CE9 RID: 3305
		private readonly VotesConfig m_config;

		// Token: 0x04000CEA RID: 3306
		private ConfigSection m_configSection;
	}
}
