using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Configuration;

namespace MasterServer.Core
{
	// Token: 0x02000109 RID: 265
	[ConsoleCmdAttributes(CmdName = "set", ArgsSize = 3)]
	public class SetConfigValueCmd : IConsoleCmd
	{
		// Token: 0x0600044E RID: 1102 RVA: 0x000128B3 File Offset: 0x00010CB3
		public SetConfigValueCmd(IConfigurationService configurationService, IConfigFileProvider configFileProvider)
		{
			this.m_configurationService = configurationService;
			this.m_configFileProvider = configFileProvider;
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x000128CC File Offset: 0x00010CCC
		public void ExecuteCmd(string[] args)
		{
			IEnumerable<ConfigInfo> source = this.m_configFileProvider.Get();
			if (args.Length == 1)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("\nAvailable config values:");
				IEnumerable<Config> enumerable = from info in source
				select this.m_configurationService.GetConfig(info);
				foreach (Config config in enumerable)
				{
					stringBuilder.AppendLine(string.Format("\t[{0}]", config.Name));
				}
				Log.Info(stringBuilder.ToString());
			}
			else
			{
				int num = args[1].IndexOf('.');
				int num2 = args[1].LastIndexOf('.');
				string cfg = args[1].Substring(0, num);
				string text = string.Empty;
				if (num2 != num)
				{
					text = args[1].Substring(num + 1, num2 - num - 1);
				}
				string text2 = args[1].Substring(num2 + 1);
				ConfigInfo configInfo = source.FirstOrDefault((ConfigInfo info) => info.Name == cfg);
				if (configInfo == null)
				{
					throw new InvalidOperationException(string.Format("ConfigInfo named '{0}' was not found", cfg));
				}
				Config config2 = this.m_configurationService.GetConfig(configInfo);
				ConfigSection configSection;
				if (!string.IsNullOrEmpty(text))
				{
					configSection = config2.GetSection(text);
					if (configSection == null)
					{
						Log.Info<string>("Section '{0}' not found", text);
						return;
					}
				}
				else
				{
					configSection = config2;
				}
				if (text2 == "*")
				{
					foreach (KeyValuePair<string, ConfigValue> keyValuePair in configSection.GetAllValues())
					{
						Log.Info<string, string>("{0} = {1}", keyValuePair.Key, keyValuePair.Value.GetString());
					}
				}
				else
				{
					if (args.Length == 3)
					{
						configSection.Set(text2, args[2]);
					}
					Log.Info<string, string>("{0} = {1}", args[1], configSection.Get(text2));
				}
			}
		}

		// Token: 0x040001C6 RID: 454
		private readonly IConfigurationService m_configurationService;

		// Token: 0x040001C7 RID: 455
		private readonly IConfigFileProvider m_configFileProvider;
	}
}
