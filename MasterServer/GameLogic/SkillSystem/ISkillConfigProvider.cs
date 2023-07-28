using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.SkillSystem
{
	// Token: 0x020000FB RID: 251
	[Contract]
	internal interface ISkillConfigProvider
	{
		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000418 RID: 1048
		bool HasAnyConfigs { get; }

		// Token: 0x06000419 RID: 1049
		bool HasConfig(SkillType skillType);

		// Token: 0x0600041A RID: 1050
		SkillConfig GetSkillConfig(SkillType skillType);

		// Token: 0x0600041B RID: 1051
		IEnumerable<SkillConfig> GetChannelSkillConfigs();
	}
}
