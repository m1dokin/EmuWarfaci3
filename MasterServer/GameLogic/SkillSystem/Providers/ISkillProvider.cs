using System;
using HK2Net;

namespace MasterServer.GameLogic.SkillSystem.Providers
{
	// Token: 0x02000431 RID: 1073
	[Contract]
	internal interface ISkillProvider
	{
		// Token: 0x1700021A RID: 538
		// (get) Token: 0x060016F3 RID: 5875
		SkillType SkillType { get; }

		// Token: 0x060016F4 RID: 5876
		Skill GetSkill(ulong profileId);

		// Token: 0x060016F5 RID: 5877
		double GetMaxSkillValue();
	}
}
