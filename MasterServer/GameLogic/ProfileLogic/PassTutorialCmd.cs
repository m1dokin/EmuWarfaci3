using System;
using MasterServer.Core;
using MasterServer.GameLogic.SpecialProfileRewards;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200056A RID: 1386
	[ConsoleCmdAttributes(ArgsSize = 4, CmdName = "pass_tutorial", Help = "profile_id, idx, silent, event")]
	internal class PassTutorialCmd : AbstractUnlockProgressionCmd
	{
		// Token: 0x06001DFA RID: 7674 RVA: 0x00079A9E File Offset: 0x00077E9E
		public PassTutorialCmd(ISpecialProfileRewardService specialProfileRewardService, ILogService logService, IProfileProgressionService profileProgressionService) : base(specialProfileRewardService, logService, profileProgressionService)
		{
		}

		// Token: 0x06001DFB RID: 7675 RVA: 0x00079AAC File Offset: 0x00077EAC
		protected override bool UnlockProgression(ulong profileId, byte id, bool silent, ILogGroup logGroup)
		{
			ProfileProgressionInfo profileProgressionInfo = this.ProfileProgressionService.PassTutorial(profileId, (int)id, silent, logGroup);
			return profileProgressionInfo.IsTutorialUnlocked((int)id);
		}
	}
}
