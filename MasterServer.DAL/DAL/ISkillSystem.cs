using System;

namespace MasterServer.DAL
{
	// Token: 0x02000095 RID: 149
	public interface ISkillSystem
	{
		// Token: 0x060001BE RID: 446
		DALResult<SkillInfo> GetSkill(ulong profileId, string skillType);

		// Token: 0x060001BF RID: 447
		DALResult<SkillInfo> AddSkill(ulong profileId, string skillType, double value, double curveCoef);

		// Token: 0x060001C0 RID: 448
		DALResultVoid DeleteSkill(ulong profileId, string skillType);
	}
}
