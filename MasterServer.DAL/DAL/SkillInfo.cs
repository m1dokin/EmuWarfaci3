using System;

namespace MasterServer.DAL
{
	// Token: 0x02000096 RID: 150
	[Serializable]
	public struct SkillInfo
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060001C1 RID: 449 RVA: 0x000057DE File Offset: 0x00003BDE
		public bool IsEmptySkill
		{
			get
			{
				return this.Equals(SkillInfo.NULL_SKILL_INFO);
			}
		}

		// Token: 0x04000183 RID: 387
		public string Type;

		// Token: 0x04000184 RID: 388
		public double Points;

		// Token: 0x04000185 RID: 389
		public double CurveCoef;

		// Token: 0x04000186 RID: 390
		private static SkillInfo NULL_SKILL_INFO = default(SkillInfo);
	}
}
