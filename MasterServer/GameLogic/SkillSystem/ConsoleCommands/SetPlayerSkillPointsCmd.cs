using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.SkillSystem.ConsoleCommands
{
	// Token: 0x020000F7 RID: 247
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "set_player_skill_points", Help = "Sets player skill points value by type for given profile id (only for DB-stored skills)")]
	internal class SetPlayerSkillPointsCmd : ConsoleCommand<SetPlayerSkillPointsCmdParams>
	{
		// Token: 0x0600040B RID: 1035 RVA: 0x00011BD6 File Offset: 0x0000FFD6
		public SetPlayerSkillPointsCmd(IDebugSkillService skillService)
		{
			this.m_skillService = skillService;
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x00011BE8 File Offset: 0x0000FFE8
		protected override void Execute(SetPlayerSkillPointsCmdParams param)
		{
			this.m_skillService.SetSkillPoints(param.ProfileId, param.Type, param.Value);
			double skillPoints = this.m_skillService.GetSkillPoints(param.ProfileId, param.Type);
			Log.Info(string.Format("Profile {0} {1} skill points were successfully updated to {2}", param.ProfileId, param.Type, skillPoints.ToString("F4")));
		}

		// Token: 0x040001B4 RID: 436
		private readonly IDebugSkillService m_skillService;
	}
}
