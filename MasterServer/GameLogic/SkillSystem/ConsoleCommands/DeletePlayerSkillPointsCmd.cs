using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.SkillSystem.ConsoleCommands
{
	// Token: 0x02000429 RID: 1065
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "delete_player_skill_points", Help = "Delete player skill points value by type for given profile id")]
	internal class DeletePlayerSkillPointsCmd : ConsoleCommand<DeletePlayerSkillPointsCmdParams>
	{
		// Token: 0x060016DA RID: 5850 RVA: 0x0005FB8A File Offset: 0x0005DF8A
		public DeletePlayerSkillPointsCmd(IDebugSkillService skillService)
		{
			this.m_skillService = skillService;
		}

		// Token: 0x060016DB RID: 5851 RVA: 0x0005FB99 File Offset: 0x0005DF99
		protected override void Execute(DeletePlayerSkillPointsCmdParams param)
		{
			this.m_skillService.DeleteSkillPoints(param.ProfileId, param.Type);
			Log.Info(string.Format("Profile {0} {1} skill was deleted", param.ProfileId, param.Type));
		}

		// Token: 0x04000B0A RID: 2826
		private readonly IDebugSkillService m_skillService;
	}
}
