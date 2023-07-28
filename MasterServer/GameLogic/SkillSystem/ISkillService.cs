using System;
using HK2Net;

namespace MasterServer.GameLogic.SkillSystem
{
	// Token: 0x0200043C RID: 1084
	[Contract]
	internal interface ISkillService
	{
		// Token: 0x1400004B RID: 75
		// (add) Token: 0x06001729 RID: 5929
		// (remove) Token: 0x0600172A RID: 5930
		event Action<ulong, Skill> SkillChanged;

		// Token: 0x0600172B RID: 5931
		Skill GetSkill(ulong profileId, SkillType skillType);

		// Token: 0x0600172C RID: 5932
		double GetMaxChannelSkillByType(SkillType skillType);
	}
}
