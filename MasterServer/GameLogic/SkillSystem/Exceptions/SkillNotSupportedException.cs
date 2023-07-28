using System;

namespace MasterServer.GameLogic.SkillSystem.Exceptions
{
	// Token: 0x0200042F RID: 1071
	internal class SkillNotSupportedException : SkillServiceException
	{
		// Token: 0x060016EF RID: 5871 RVA: 0x0005FD28 File Offset: 0x0005E128
		public SkillNotSupportedException(SkillType skillType) : base(string.Format("Skill type {0} is not supported by SkillService", skillType))
		{
		}

		// Token: 0x060016F0 RID: 5872 RVA: 0x0005FD40 File Offset: 0x0005E140
		public SkillNotSupportedException(string message) : base(message)
		{
		}
	}
}
