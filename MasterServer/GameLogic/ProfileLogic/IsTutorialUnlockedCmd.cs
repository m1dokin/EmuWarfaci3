using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200056C RID: 1388
	[ConsoleCmdAttributes(ArgsSize = 2, CmdName = "is_tutorial_unlocked", Help = "profile_id, idx")]
	internal class IsTutorialUnlockedCmd : AbstractCheckProgressionCmd
	{
		// Token: 0x06001DFF RID: 7679 RVA: 0x00079B14 File Offset: 0x00077F14
		public IsTutorialUnlockedCmd(IProfileProgressionService profileProgressionService) : base(profileProgressionService)
		{
		}

		// Token: 0x06001E00 RID: 7680 RVA: 0x00079B1D File Offset: 0x00077F1D
		protected override void LogIsProgressionUnlocked(ProfileProgressionInfo info, byte id)
		{
			Log.Info(string.Format("Tutorial is {0}", (!info.IsTutorialUnlocked((int)id)) ? "locked" : "unlocked"));
		}
	}
}
