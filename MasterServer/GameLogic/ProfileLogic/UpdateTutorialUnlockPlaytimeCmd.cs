using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000571 RID: 1393
	[ConsoleCmdAttributes(ArgsSize = 2, CmdName = "update_tutorial_unlock_playtime", Help = "index, minutes")]
	internal class UpdateTutorialUnlockPlaytimeCmd : IConsoleCmd
	{
		// Token: 0x06001E09 RID: 7689 RVA: 0x00079BF8 File Offset: 0x00077FF8
		public UpdateTutorialUnlockPlaytimeCmd(IProfileProgressionDebug profileProgressionDebug)
		{
			this.m_profileProgressionDebug = profileProgressionDebug;
		}

		// Token: 0x06001E0A RID: 7690 RVA: 0x00079C08 File Offset: 0x00078008
		public void ExecuteCmd(string[] args)
		{
			int index = int.Parse(args[1]);
			TimeSpan time = TimeSpan.FromMinutes((double)int.Parse(args[2]));
			this.m_profileProgressionDebug.UpdateTutorialUnlockPlaytime(index, time);
		}

		// Token: 0x04000E85 RID: 3717
		private readonly IProfileProgressionDebug m_profileProgressionDebug;
	}
}
