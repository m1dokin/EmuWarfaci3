using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.SkillSystem.ConsoleCommands
{
	// Token: 0x02000427 RID: 1063
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "add_player_skill_points", Help = "Adds player skill points value by type for given profile id (only for DB-stored skills)")]
	internal class AddPlayerSkillPointsCmd : ConsoleCommand<AddPlayerSkillPointsCmdParams>
	{
		// Token: 0x060016D1 RID: 5841 RVA: 0x0005FACB File Offset: 0x0005DECB
		public AddPlayerSkillPointsCmd(IDebugSkillService skillService)
		{
			this.m_skillService = skillService;
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x0005FADC File Offset: 0x0005DEDC
		protected override void Execute(AddPlayerSkillPointsCmdParams param)
		{
			this.m_skillService.AddSkillPoints(param.ProfileId, param.Type, param.Value);
			double skillPoints = this.m_skillService.GetSkillPoints(param.ProfileId, param.Type);
			Log.Info(string.Format("Profile {0} {1} skill points were successfully updated to {2}", param.ProfileId, param.Type, skillPoints.ToString("F4")));
		}

		// Token: 0x04000B06 RID: 2822
		private readonly IDebugSkillService m_skillService;
	}
}
