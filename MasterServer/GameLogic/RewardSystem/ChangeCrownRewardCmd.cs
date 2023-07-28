using System;
using MasterServer.Core;
using MasterServer.Core.Configuration;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005A4 RID: 1444
	[ConsoleCmdAttributes(CmdName = "change_crown_reward", ArgsSize = 5, Help = "Change reward config for star system")]
	internal class ChangeCrownRewardCmd : IConsoleCmd
	{
		// Token: 0x06001F0A RID: 7946 RVA: 0x0007E1B4 File Offset: 0x0007C5B4
		public void ExecuteCmd(string[] args)
		{
			ConfigSection section = Resources.Rewards.GetSection("CrownRewards");
			if (args.Length >= 5)
			{
				string strB = args[1];
				string value = args[2];
				string value2 = args[3];
				string value3 = args[4];
				foreach (ConfigSection configSection in section.GetSections("Reward"))
				{
					string strA = configSection.Get("type");
					if (string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase) == 0)
					{
						configSection.Set("bronze", value);
						configSection.Set("silver", value2);
						configSection.Set("gold", value3);
						break;
					}
				}
			}
			Log.Info(section.ToString());
		}
	}
}
