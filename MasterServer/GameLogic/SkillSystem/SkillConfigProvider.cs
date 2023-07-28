using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.GameLogic.SkillSystem.Exceptions;

namespace MasterServer.GameLogic.SkillSystem
{
	// Token: 0x020000FC RID: 252
	[Service]
	[Singleton]
	internal class SkillConfigProvider : ServiceModule, ISkillConfigProvider
	{
		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600041D RID: 1053 RVA: 0x00011CF9 File Offset: 0x000100F9
		public bool HasAnyConfigs
		{
			get
			{
				return this.m_skillTypeConfigs.Any<KeyValuePair<SkillType, Dictionary<Resources.ChannelType, SkillConfig>>>();
			}
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x00011D06 File Offset: 0x00010106
		public bool HasConfig(SkillType skillType)
		{
			return this.m_skillTypeConfigs.ContainsKey(skillType);
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x00011D14 File Offset: 0x00010114
		public override void Init()
		{
			ConfigSection skillConfigSection = this.GetSkillConfigSection();
			this.m_skillTypeConfigs = this.ParseSkillConfigSection(skillConfigSection);
			foreach (ConfigSection configSection in skillConfigSection.GetSections("Skill"))
			{
				foreach (List<ConfigSection> list in configSection.GetAllSections().Values)
				{
					foreach (ConfigSection configSection2 in list)
					{
						configSection2.OnConfigChanged += this.OnConfigChanged;
					}
				}
			}
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x00011E20 File Offset: 0x00010220
		public override void Stop()
		{
			ConfigSection skillConfigSection = this.GetSkillConfigSection();
			foreach (ConfigSection configSection in skillConfigSection.GetSections("Skill"))
			{
				foreach (List<ConfigSection> list in configSection.GetAllSections().Values)
				{
					foreach (ConfigSection configSection2 in list)
					{
						configSection2.OnConfigChanged -= this.OnConfigChanged;
					}
				}
			}
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x00011F20 File Offset: 0x00010320
		public SkillConfig GetSkillConfig(SkillType skillType)
		{
			Dictionary<Resources.ChannelType, SkillConfig> dictionary;
			SkillConfig result;
			if (this.m_skillTypeConfigs.TryGetValue(skillType, out dictionary) && dictionary.TryGetValue(Resources.Channel, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x00011F58 File Offset: 0x00010358
		public IEnumerable<SkillConfig> GetChannelSkillConfigs()
		{
			List<SkillConfig> list = new List<SkillConfig>();
			Resources.ChannelType channelType = Resources.Channel;
			foreach (Dictionary<Resources.ChannelType, SkillConfig> source in this.m_skillTypeConfigs.Values)
			{
				list.AddRange(from k in source
				where k.Key == channelType
				select k into v
				select v.Value);
			}
			return list;
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x00012004 File Offset: 0x00010404
		private Dictionary<SkillType, Dictionary<Resources.ChannelType, SkillConfig>> ParseSkillConfigSection(ConfigSection skillConfigSection)
		{
			Dictionary<SkillType, Dictionary<Resources.ChannelType, SkillConfig>> dictionary = new Dictionary<SkillType, Dictionary<Resources.ChannelType, SkillConfig>>();
			SkillConfigValidator skillConfigValidator = new SkillConfigValidator();
			foreach (ConfigSection configSection in skillConfigSection.GetSections("Skill"))
			{
				SkillType skillType = this.ParseSkillType(configSection.Get("type"));
				Dictionary<Resources.ChannelType, SkillConfig> dictionary2;
				if (!dictionary.TryGetValue(skillType, out dictionary2))
				{
					dictionary2 = new Dictionary<Resources.ChannelType, SkillConfig>();
					dictionary.Add(skillType, dictionary2);
				}
				foreach (List<ConfigSection> source in configSection.GetAllSections().Values)
				{
					ConfigSection configSection2 = source.First<ConfigSection>();
					SkillConfig skillConfig = new SkillConfig(skillType, configSection2);
					skillConfigValidator.Validate(skillConfig);
					Resources.ChannelType key = Utils.ParseEnum<Resources.ChannelType>(configSection2.Name);
					dictionary2.Add(key, skillConfig);
				}
			}
			this.AtLeastOneChannelShouldBeConfiguredValidation(dictionary);
			this.CurrentChannelConfigValidation(dictionary);
			return dictionary;
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x0001212C File Offset: 0x0001052C
		private SkillType ParseSkillType(string skillTypeString)
		{
			SkillType skillType;
			try
			{
				skillType = Utils.ParseEnum<SkillType>(skillTypeString);
			}
			catch (Exception innerException)
			{
				throw new SkillTypeParseException(skillTypeString, innerException);
			}
			if (skillType == SkillType.None)
			{
				throw new SkillTypeParseException(skillTypeString);
			}
			return skillType;
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x00012170 File Offset: 0x00010570
		private void CurrentChannelConfigValidation(Dictionary<SkillType, Dictionary<Resources.ChannelType, SkillConfig>> skillTypeConfigs)
		{
			SkillType skillTypeByChannelType = SkillTypeHelper.GetSkillTypeByChannelType(Resources.Channel);
			Dictionary<Resources.ChannelType, SkillConfig> dictionary;
			if (skillTypeConfigs.TryGetValue(skillTypeByChannelType, out dictionary) && !dictionary.ContainsKey(Resources.Channel))
			{
				throw new ChannelSkillConfigNotFoundException(skillTypeByChannelType, Resources.ChannelName);
			}
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x000121B4 File Offset: 0x000105B4
		private void AtLeastOneChannelShouldBeConfiguredValidation(Dictionary<SkillType, Dictionary<Resources.ChannelType, SkillConfig>> skillTypeConfigs)
		{
			foreach (Dictionary<Resources.ChannelType, SkillConfig> source in skillTypeConfigs.Values)
			{
				if (!source.Any<KeyValuePair<Resources.ChannelType, SkillConfig>>())
				{
					throw new InvalidSkillConfigException("At least one channel should be configured for skill type");
				}
			}
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x00012220 File Offset: 0x00010620
		private void OnConfigChanged(ConfigEventArgs args)
		{
			ConfigSection skillConfigSection = this.GetSkillConfigSection();
			Dictionary<SkillType, Dictionary<Resources.ChannelType, SkillConfig>> value = this.ParseSkillConfigSection(skillConfigSection);
			Interlocked.Exchange<Dictionary<SkillType, Dictionary<Resources.ChannelType, SkillConfig>>>(ref this.m_skillTypeConfigs, value);
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x00012249 File Offset: 0x00010649
		private ConfigSection GetSkillConfigSection()
		{
			return Resources.ModuleSettings.GetSection("Skills");
		}

		// Token: 0x040001B8 RID: 440
		private Dictionary<SkillType, Dictionary<Resources.ChannelType, SkillConfig>> m_skillTypeConfigs;
	}
}
