using System;
using System.Collections.Generic;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000565 RID: 1381
	[ConsoleCmdAttributes(ArgsSize = 2, CmdName = "unlock_mission", Help = "profile_id, mission_type")]
	internal class UnlockMissionCmd : IConsoleCmd
	{
		// Token: 0x06001DEF RID: 7663 RVA: 0x00079740 File Offset: 0x00077B40
		public UnlockMissionCmd(IProfileProgressionService profileProgressionService, ILogService logService)
		{
			this.m_profileProgressionService = profileProgressionService;
			this.m_logService = logService;
		}

		// Token: 0x06001DF0 RID: 7664 RVA: 0x00079758 File Offset: 0x00077B58
		public void ExecuteCmd(string[] args)
		{
			if (args.Length < 3)
			{
				Log.Error("Invalid number of arguments");
				return;
			}
			string requestedMissionType = args[2];
			ulong profileId;
			if (!ulong.TryParse(args[1], out profileId) || string.IsNullOrEmpty(requestedMissionType))
			{
				Log.Error("Invalid arguments");
				return;
			}
			ProfileProgressionInfo progression = this.m_profileProgressionService.GetProgression(profileId);
			List<ConfigSection> sections = Resources.ProfileProgressionConfig.GetSections("mission_unlock");
			ConfigSection configSection = sections.Find((ConfigSection sec) => sec.Get("unlock_type").Contains(requestedMissionType));
			ProfileProgressionInfo.MissionType missionType = Utils.ParseEnum<ProfileProgressionInfo.MissionType>(requestedMissionType);
			int num;
			configSection.TryGet("max_value", out num, 0);
			if (num > 1)
			{
				MissionUnlockBranch missionUnlockBranch = ProfileProgressionService.GetMissionUnlockBranch(missionType);
				int missionPassCounter = ProfileProgressionService.GetMissionPassCounter(progression, missionUnlockBranch);
				if (missionPassCounter < num)
				{
					this.m_profileProgressionService.IncrementMissionPassCounter(profileId, num - missionPassCounter, num, missionUnlockBranch);
				}
			}
			string unlockMission = requestedMissionType;
			while (!string.IsNullOrEmpty(unlockMission))
			{
				configSection = sections.Find((ConfigSection sec) => sec.Get("unlock_type").Contains(unlockMission));
				missionType = Utils.ParseEnum<ProfileProgressionInfo.MissionType>(unlockMission);
				unlockMission = configSection.Get("type");
				this.m_profileProgressionService.UnlockMission(progression, missionType, true, this.m_logService.Event);
			}
		}

		// Token: 0x04000E7C RID: 3708
		private readonly IProfileProgressionService m_profileProgressionService;

		// Token: 0x04000E7D RID: 3709
		private readonly ILogService m_logService;
	}
}
