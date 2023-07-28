using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.SkillSystem.ConsoleCommands
{
	// Token: 0x0200042D RID: 1069
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "get_player_skill_points", Help = "Gets player skill points value by type for given profile id (only for DB-stored skills)")]
	internal class GetPlayerSkillPointsCmd : ConsoleCommand<GetPlayerSkillPointsCmdParams>
	{
		// Token: 0x060016E8 RID: 5864 RVA: 0x0005FC99 File Offset: 0x0005E099
		public GetPlayerSkillPointsCmd(IDebugSkillService skillService)
		{
			this.m_skillService = skillService;
		}

		// Token: 0x060016E9 RID: 5865 RVA: 0x0005FCA8 File Offset: 0x0005E0A8
		protected override void Execute(GetPlayerSkillPointsCmdParams param)
		{
			double skillPoints = this.m_skillService.GetSkillPoints(param.ProfileId, param.Type);
			Log.Info(string.Format("Profile {0} has {1} skill points = {2}", param.ProfileId, param.Type, skillPoints.ToString("F4")));
		}

		// Token: 0x04000B10 RID: 2832
		private readonly IDebugSkillService m_skillService;
	}
}
