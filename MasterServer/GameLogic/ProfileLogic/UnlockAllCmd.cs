using System;
using System.Collections;
using MasterServer.Core;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000564 RID: 1380
	[ConsoleCmdAttributes(ArgsSize = 1, CmdName = "unlock_all_progression", Help = "profile_id")]
	internal class UnlockAllCmd : IConsoleCmd
	{
		// Token: 0x06001DED RID: 7661 RVA: 0x0007955D File Offset: 0x0007795D
		public UnlockAllCmd(IProfileProgressionService profileProgressionService, ILogService logService)
		{
			this.m_profileProgressionService = profileProgressionService;
			this.m_logService = logService;
		}

		// Token: 0x06001DEE RID: 7662 RVA: 0x00079574 File Offset: 0x00077974
		public void ExecuteCmd(string[] args)
		{
			ulong profileId = ulong.Parse(args[1]);
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				ProfileProgressionInfo progression = this.m_profileProgressionService.GetProgression(profileId);
				IEnumerator enumerator = Enum.GetValues(typeof(ProfileProgressionInfo.MissionType)).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						ProfileProgressionInfo.MissionType unlockedMissionType = (ProfileProgressionInfo.MissionType)obj;
						this.m_profileProgressionService.UnlockMission(progression, unlockedMissionType, true, logGroup);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				IEnumerator enumerator2 = Enum.GetValues(typeof(ProfileProgressionInfo.PlayerClass)).GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						object obj2 = enumerator2.Current;
						ProfileProgressionInfo.PlayerClass classId = (ProfileProgressionInfo.PlayerClass)obj2;
						this.m_profileProgressionService.UnlockClass(progression, classId, true, logGroup);
					}
				}
				finally
				{
					IDisposable disposable2;
					if ((disposable2 = (enumerator2 as IDisposable)) != null)
					{
						disposable2.Dispose();
					}
				}
				IEnumerator enumerator3 = Enum.GetValues(typeof(ProfileProgressionInfo.Tutorial)).GetEnumerator();
				try
				{
					while (enumerator3.MoveNext())
					{
						object obj3 = enumerator3.Current;
						ProfileProgressionInfo.Tutorial tutorialId = (ProfileProgressionInfo.Tutorial)obj3;
						this.m_profileProgressionService.UnlockTutorial(progression, tutorialId, true, logGroup);
					}
				}
				finally
				{
					IDisposable disposable3;
					if ((disposable3 = (enumerator3 as IDisposable)) != null)
					{
						disposable3.Dispose();
					}
				}
			}
		}

		// Token: 0x04000E7A RID: 3706
		private readonly IProfileProgressionService m_profileProgressionService;

		// Token: 0x04000E7B RID: 3707
		private readonly ILogService m_logService;
	}
}
