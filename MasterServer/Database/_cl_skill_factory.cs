using System;
using MasterServer.GameLogic.SkillSystem;

namespace MasterServer.Database
{
	// Token: 0x020001E4 RID: 484
	public struct _cl_skill_factory
	{
		// Token: 0x06000967 RID: 2407 RVA: 0x00023820 File Offset: 0x00021C20
		public _cl_skill_factory(string name)
		{
			this.m_name = name;
		}

		// Token: 0x1700013B RID: 315
		public cache_domain this[SkillType skillType]
		{
			get
			{
				return new cache_domain(this.m_name + ".skill_" + skillType);
			}
		}

		// Token: 0x0400054A RID: 1354
		private string m_name;
	}
}
