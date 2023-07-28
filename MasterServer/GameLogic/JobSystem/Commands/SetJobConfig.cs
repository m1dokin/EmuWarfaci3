using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Core;
using MasterServer.Core.Configuration;

namespace MasterServer.GameLogic.JobSystem.Commands
{
	// Token: 0x02000062 RID: 98
	[ConsoleCmdAttributes(CmdName = "set_job_config", ArgsSize = 3)]
	internal class SetJobConfig : IConsoleCmd
	{
		// Token: 0x06000176 RID: 374 RVA: 0x0000A13C File Offset: 0x0000853C
		public void ExecuteCmd(string[] args)
		{
			string jobName = args[1].ToLower();
			string text = args[2].ToLower();
			string text2 = args[3];
			ConfigSection section = Resources.ModuleSettings.GetSection("Jobs");
			ConfigSection configSection = (from cs in section.GetAllSections()
			where string.Compare(cs.Key, "job", StringComparison.OrdinalIgnoreCase) == 0
			select cs.Value).SelectMany((List<ConfigSection> cs) => cs).FirstOrDefault((ConfigSection cs) => string.Compare(cs.Get("name"), jobName, StringComparison.OrdinalIgnoreCase) == 0);
			if (configSection == null)
			{
				Log.Warning<string>("Can't find job with name '{0}'", jobName);
				return;
			}
			if (configSection.Set(text, text2))
			{
				Log.Info<string, string, string>("{0}'s config '{1}' has been set to '{2}'", jobName, text, text2);
			}
		}
	}
}
