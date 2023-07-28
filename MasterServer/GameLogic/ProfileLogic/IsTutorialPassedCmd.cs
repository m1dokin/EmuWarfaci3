using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200056D RID: 1389
	[ConsoleCmdAttributes(ArgsSize = 2, CmdName = "is_tutorial_passed", Help = "profile_id, idx")]
	internal class IsTutorialPassedCmd : AbstractCheckProgressionCmd
	{
		// Token: 0x06001E01 RID: 7681 RVA: 0x00079B49 File Offset: 0x00077F49
		public IsTutorialPassedCmd(IProfileProgressionService profileProgressionService) : base(profileProgressionService)
		{
		}

		// Token: 0x06001E02 RID: 7682 RVA: 0x00079B52 File Offset: 0x00077F52
		protected override void LogIsProgressionUnlocked(ProfileProgressionInfo info, byte id)
		{
			Log.Info(string.Format("Tutorial is {0}", (!info.IsTutorialPassed((int)id)) ? "not passed" : "passed"));
		}
	}
}
