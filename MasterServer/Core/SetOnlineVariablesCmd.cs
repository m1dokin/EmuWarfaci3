using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Core.Configuration;

namespace MasterServer.Core
{
	// Token: 0x0200015A RID: 346
	[ConsoleCmdAttributes(CmdName = "set_online_variables", ArgsSize = 4)]
	internal class SetOnlineVariablesCmd : IConsoleCmd
	{
		// Token: 0x06000612 RID: 1554 RVA: 0x00018554 File Offset: 0x00016954
		public void ExecuteCmd(string[] args)
		{
			string text = args[1];
			string value = args[2];
			string value2 = (args.Length <= 3) ? string.Empty : args[3];
			string groupName = (args.Length <= 4) ? string.Empty : args[4];
			string destination = (args.Length <= 5) ? string.Empty : args[5];
			List<ConfigSection> source;
			if (!string.IsNullOrEmpty(groupName) && Resources.OnlineVariablesSettings.TryGetSections("Group", out source))
			{
				ConfigSection configSection = source.SingleOrDefault((ConfigSection cs) => cs.Get("name") == groupName);
				if (configSection != null)
				{
					List<ConfigSection> sections = configSection.GetSections("Variable");
					ConfigSection onlineVariable = SetOnlineVariablesCmd.GetOnlineVariable(sections, text, destination);
					if (onlineVariable != null)
					{
						if (!string.IsNullOrEmpty(value2))
						{
							onlineVariable.Set("probability", value2);
						}
						onlineVariable.Set("value", value);
					}
				}
			}
			else
			{
				List<ConfigSection> sections2 = Resources.OnlineVariablesSettings.GetSections("Variable");
				ConfigSection onlineVariable2 = SetOnlineVariablesCmd.GetOnlineVariable(sections2, text, destination);
				if (onlineVariable2 != null)
				{
					if (!string.IsNullOrEmpty(value2))
					{
						onlineVariable2.Set("probability", value2);
					}
					string text2 = onlineVariable2.Get("value");
					if (OnlineVariable.IsLinkedVariable(text2))
					{
						Log.Warning<string, string>("Changing linked to config online variable {0}, was linked to {1}", text, text2);
					}
					onlineVariable2.Set("value", value);
				}
			}
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x000186BC File Offset: 0x00016ABC
		private static ConfigSection GetOnlineVariable(IEnumerable<ConfigSection> variables, string name, string destination)
		{
			return variables.FirstOrDefault((ConfigSection x) => x.Get("name") == name && (string.IsNullOrEmpty(destination) || x.Get("destination") == destination));
		}
	}
}
