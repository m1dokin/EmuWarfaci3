using System;

namespace MasterServer.GameLogic.SkillSystem.Exceptions
{
	// Token: 0x02000430 RID: 1072
	internal class SkillServiceException : ApplicationException
	{
		// Token: 0x060016F1 RID: 5873 RVA: 0x00011C96 File Offset: 0x00010096
		protected SkillServiceException(string message) : base(message)
		{
		}

		// Token: 0x060016F2 RID: 5874 RVA: 0x00011C9F File Offset: 0x0001009F
		protected SkillServiceException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
