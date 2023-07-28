using System;
using MasterServer.Core;
using MasterServer.GameLogic.SpecialProfileRewards;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000569 RID: 1385
	[ConsoleCmdAttributes(ArgsSize = 4, CmdName = "unlock_class", Help = "profile_id, idx, silent, event")]
	internal class UnlockClasslCmd : AbstractUnlockProgressionCmd
	{
		// Token: 0x06001DF8 RID: 7672 RVA: 0x00079A5E File Offset: 0x00077E5E
		public UnlockClasslCmd(ISpecialProfileRewardService specialProfileRewardService, ILogService logService, IProfileProgressionService profileProgressionService) : base(specialProfileRewardService, logService, profileProgressionService)
		{
		}

		// Token: 0x06001DF9 RID: 7673 RVA: 0x00079A6C File Offset: 0x00077E6C
		protected override bool UnlockProgression(ulong profileId, byte id, bool silent, ILogGroup logGroup)
		{
			ProfileProgressionInfo progression = this.ProfileProgressionService.GetProgression(profileId);
			ProfileProgressionInfo profileProgressionInfo = this.ProfileProgressionService.UnlockClass(progression, (int)id, silent, logGroup);
			return profileProgressionInfo.IsTutorialUnlocked((int)id);
		}
	}
}
