using System;
using MasterServer.DAL;
using MasterServer.GameLogic.SkillSystem;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000213 RID: 531
	internal interface ISkillSystemClient
	{
		// Token: 0x06000B88 RID: 2952
		SkillInfo GetSkill(ulong profileId, SkillType skillType);

		// Token: 0x06000B89 RID: 2953
		SkillInfo AddSkill(ulong profileId, SkillType skillType, double value, double curveCoef);

		// Token: 0x06000B8A RID: 2954
		void DeleteSkill(ulong profileId, SkillType skillType);
	}
}
