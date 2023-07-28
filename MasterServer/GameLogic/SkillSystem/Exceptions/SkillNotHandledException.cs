using System;

namespace MasterServer.GameLogic.SkillSystem.Exceptions
{
	// Token: 0x020000F9 RID: 249
	internal class SkillNotHandledException : SkillServiceException
	{
		// Token: 0x06000414 RID: 1044 RVA: 0x00011CA9 File Offset: 0x000100A9
		public SkillNotHandledException(SkillType skillType) : base(string.Format("Skill of type {0} is not handled by SkillService", skillType))
		{
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x00011CC1 File Offset: 0x000100C1
		public SkillNotHandledException(string message) : base(message)
		{
		}
	}
}
