using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000566 RID: 1382
	[ConsoleCmdAttributes(ArgsSize = 2, CmdName = "is_mission_unlocked", Help = "profile_id, mission_type")]
	internal class IsMissionUnlockedCmd : IConsoleCmd
	{
		// Token: 0x06001DF1 RID: 7665 RVA: 0x000798E1 File Offset: 0x00077CE1
		public IsMissionUnlockedCmd(IProfileProgressionService profileProgressionService)
		{
			this.m_profileProgressionService = profileProgressionService;
		}

		// Token: 0x06001DF2 RID: 7666 RVA: 0x000798F0 File Offset: 0x00077CF0
		public void ExecuteCmd(string[] args)
		{
			if (args.Length < 3)
			{
				Log.Error("Invalid number of arguments");
				return;
			}
			string text = args[2];
			ulong profileId;
			if (!ulong.TryParse(args[1], out profileId) || string.IsNullOrEmpty(text))
			{
				Log.Error("Invalid arguments");
				return;
			}
			ProfileProgressionInfo progression = this.m_profileProgressionService.GetProgression(profileId);
			Log.Info("Mission is " + ((!progression.IsMissionTypeUnlocked(text)) ? "locked" : "unlocked"));
		}

		// Token: 0x04000E7E RID: 3710
		private readonly IProfileProgressionService m_profileProgressionService;
	}
}
