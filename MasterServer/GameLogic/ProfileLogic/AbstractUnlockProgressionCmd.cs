using System;
using MasterServer.Core;
using MasterServer.GameLogic.SpecialProfileRewards;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000567 RID: 1383
	internal abstract class AbstractUnlockProgressionCmd : IConsoleCmd
	{
		// Token: 0x06001DF3 RID: 7667 RVA: 0x00079971 File Offset: 0x00077D71
		protected AbstractUnlockProgressionCmd(ISpecialProfileRewardService specialProfileRewardService, ILogService logService, IProfileProgressionService profileProgressionService)
		{
			this.m_specialProfileRewardService = specialProfileRewardService;
			this.m_logService = logService;
			this.ProfileProgressionService = profileProgressionService;
		}

		// Token: 0x06001DF4 RID: 7668 RVA: 0x00079990 File Offset: 0x00077D90
		public void ExecuteCmd(string[] args)
		{
			ulong profileId = ulong.Parse(args[1]);
			byte id = byte.Parse(args[2]);
			bool silent = args.Length <= 3 || int.Parse(args[3]) == 1;
			string text = (args.Length <= 4) ? string.Empty : args[4];
			bool flag = this.UnlockProgression(profileId, id, silent, this.m_logService.Event);
			if (!string.IsNullOrEmpty(text) && flag)
			{
				this.m_specialProfileRewardService.ProcessEvent(text, profileId, this.m_logService.Event);
			}
		}

		// Token: 0x06001DF5 RID: 7669
		protected abstract bool UnlockProgression(ulong profileId, byte id, bool silent, ILogGroup logGroup);

		// Token: 0x04000E7F RID: 3711
		protected readonly IProfileProgressionService ProfileProgressionService;

		// Token: 0x04000E80 RID: 3712
		private readonly ILogService m_logService;

		// Token: 0x04000E81 RID: 3713
		private readonly ISpecialProfileRewardService m_specialProfileRewardService;
	}
}
