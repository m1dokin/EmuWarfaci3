using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200056E RID: 1390
	[ConsoleCmdAttributes(ArgsSize = 2, CmdName = "is_class_unlocked", Help = "profile_id, idx")]
	internal class IsClassUnlockedCmd : AbstractCheckProgressionCmd
	{
		// Token: 0x06001E03 RID: 7683 RVA: 0x00079B7E File Offset: 0x00077F7E
		public IsClassUnlockedCmd(IProfileProgressionService profileProgressionService) : base(profileProgressionService)
		{
		}

		// Token: 0x06001E04 RID: 7684 RVA: 0x00079B87 File Offset: 0x00077F87
		protected override void LogIsProgressionUnlocked(ProfileProgressionInfo info, byte id)
		{
			Log.Info(string.Format("Class is {0}", (!info.IsClassUnlocked((int)id)) ? "locked" : "unlocked"));
		}
	}
}
