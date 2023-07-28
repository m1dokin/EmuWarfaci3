using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.Achievements
{
	// Token: 0x020000AF RID: 175
	[ConsoleCmdAttributes(CmdName = "reset_achievements", Help = "reset all achievement progression")]
	internal class ResetProfileAchievementsCmd : ConsoleCommand<ResetProfileAchievementsCmdParams>
	{
		// Token: 0x060002D0 RID: 720 RVA: 0x0000DD45 File Offset: 0x0000C145
		public ResetProfileAchievementsCmd(IDebugAchievementSystem achievementSystem)
		{
			this.m_achievementSystem = achievementSystem;
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0000DD54 File Offset: 0x0000C154
		protected override void Execute(ResetProfileAchievementsCmdParams param)
		{
			this.m_achievementSystem.DeleteProfileAchievements(param.ProfileId);
			Log.Info<ulong>("Achievement was removed for profile {0}", param.ProfileId);
		}

		// Token: 0x04000132 RID: 306
		private readonly IDebugAchievementSystem m_achievementSystem;
	}
}
