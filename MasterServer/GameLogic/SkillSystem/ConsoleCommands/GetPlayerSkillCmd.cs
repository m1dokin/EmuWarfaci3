using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.SkillSystem.ConsoleCommands
{
	// Token: 0x0200042C RID: 1068
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "get_player_skill", Help = "Gets player skill by type for given profile id")]
	internal class GetPlayerSkillCmd : ConsoleCommand<GetPlayerSkillCmdParams>
	{
		// Token: 0x060016E6 RID: 5862 RVA: 0x0005FC2B File Offset: 0x0005E02B
		public GetPlayerSkillCmd(ISkillService skillService)
		{
			this.m_skillService = skillService;
		}

		// Token: 0x060016E7 RID: 5863 RVA: 0x0005FC3C File Offset: 0x0005E03C
		protected override void Execute(GetPlayerSkillCmdParams param)
		{
			Skill skill = this.m_skillService.GetSkill(param.ProfileId, param.SkillType);
			Log.Info(string.Format("Profile {0} has {1} skill = {2}", param.ProfileId, param.SkillType, skill.Value.ToString("F4")));
		}

		// Token: 0x04000B0F RID: 2831
		private readonly ISkillService m_skillService;
	}
}
