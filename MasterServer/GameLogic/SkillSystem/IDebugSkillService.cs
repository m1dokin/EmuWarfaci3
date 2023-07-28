using System;
using HK2Net;

namespace MasterServer.GameLogic.SkillSystem
{
	// Token: 0x0200043B RID: 1083
	[Contract]
	internal interface IDebugSkillService
	{
		// Token: 0x06001725 RID: 5925
		void AddSkillPoints(ulong profileId, SkillType skillType, double skillToAdd);

		// Token: 0x06001726 RID: 5926
		void SetSkillPoints(ulong profileId, SkillType skillType, double skillPoints);

		// Token: 0x06001727 RID: 5927
		double GetSkillPoints(ulong profileId, SkillType skillType);

		// Token: 0x06001728 RID: 5928
		void DeleteSkillPoints(ulong profileId, SkillType skillType);
	}
}
