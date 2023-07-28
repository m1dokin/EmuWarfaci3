using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterServer.Core;

namespace MasterServer.GameLogic.Achievements
{
	// Token: 0x02000255 RID: 597
	[ConsoleCmdAttributes(CmdName = "achievements_list", ArgsSize = 1, Help = "Lists achievement status of specified profile")]
	internal class AchievementsListCmd : IConsoleCmd
	{
		// Token: 0x06000D2B RID: 3371 RVA: 0x00033EE4 File Offset: 0x000322E4
		public AchievementsListCmd(IAchievementSystem achievementSystem)
		{
			this.m_achievementSystem = achievementSystem;
		}

		// Token: 0x06000D2C RID: 3372 RVA: 0x00033EF4 File Offset: 0x000322F4
		public void ExecuteCmd(string[] args)
		{
			ulong profileId = ulong.Parse(args[1]);
			StringBuilder stringBuilder = new StringBuilder("\nAchievements:\n");
			var source = from a in this.m_achievementSystem.GetCurrentProfileAchievements(profileId).Values
			join d in this.m_achievementSystem.GetAllAchievementDescs().Values on a.achievementId equals d.Id into descriptions
			select new
			{
				a,
				descriptions
			} into <>__TranspIdent1
			from ad in <>__TranspIdent1.descriptions.DefaultIfEmpty<AchievementDescription>()
			select new
			{
				Id = <>__TranspIdent1.a.achievementId,
				Progress = <>__TranspIdent1.a.progress,
				Amount = ((ad == null) ? null : new uint?(ad.Amount))
			};
			source.Aggregate(stringBuilder, (StringBuilder sb, a) => sb.AppendFormat("  {0}: {1}\n", a.Id, (a.Amount == null) ? "Description for this achievement was not found in the config (achievements_data.xml)" : string.Format("{0}/{1}", a.Progress, a.Amount.Value)));
			Log.Info<string>("{0}", stringBuilder.ToString());
		}

		// Token: 0x0400060A RID: 1546
		private readonly IAchievementSystem m_achievementSystem;
	}
}
