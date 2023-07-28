using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200056B RID: 1387
	internal abstract class AbstractCheckProgressionCmd : IConsoleCmd
	{
		// Token: 0x06001DFC RID: 7676 RVA: 0x00079AD1 File Offset: 0x00077ED1
		protected AbstractCheckProgressionCmd(IProfileProgressionService profileProgressionService)
		{
			this.m_profileProgressionService = profileProgressionService;
		}

		// Token: 0x06001DFD RID: 7677 RVA: 0x00079AE0 File Offset: 0x00077EE0
		public void ExecuteCmd(string[] args)
		{
			ulong profileId = ulong.Parse(args[1]);
			byte id = byte.Parse(args[2]);
			ProfileProgressionInfo progression = this.m_profileProgressionService.GetProgression(profileId);
			this.LogIsProgressionUnlocked(progression, id);
		}

		// Token: 0x06001DFE RID: 7678
		protected abstract void LogIsProgressionUnlocked(ProfileProgressionInfo info, byte id);

		// Token: 0x04000E82 RID: 3714
		private readonly IProfileProgressionService m_profileProgressionService;
	}
}
