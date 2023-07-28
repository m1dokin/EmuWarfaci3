using System;
using MasterServer.Core;
using MasterServer.GameLogic.SpecialProfileRewards;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000568 RID: 1384
	[ConsoleCmdAttributes(ArgsSize = 4, CmdName = "unlock_tutorial", Help = "profile_id, idx, silent, event")]
	internal class UnlockTutorialCmd : AbstractUnlockProgressionCmd
	{
		// Token: 0x06001DF6 RID: 7670 RVA: 0x00079A1F File Offset: 0x00077E1F
		public UnlockTutorialCmd(ISpecialProfileRewardService specialProfileRewardService, ILogService logService, IProfileProgressionService profileProgressionService) : base(specialProfileRewardService, logService, profileProgressionService)
		{
		}

		// Token: 0x06001DF7 RID: 7671 RVA: 0x00079A2C File Offset: 0x00077E2C
		protected override bool UnlockProgression(ulong profileId, byte id, bool silent, ILogGroup logGroup)
		{
			ProfileProgressionInfo progression = this.ProfileProgressionService.GetProgression(profileId);
			ProfileProgressionInfo profileProgressionInfo = this.ProfileProgressionService.UnlockTutorial(progression, (int)id, silent, logGroup);
			return profileProgressionInfo.IsTutorialUnlocked((int)id);
		}
	}
}
