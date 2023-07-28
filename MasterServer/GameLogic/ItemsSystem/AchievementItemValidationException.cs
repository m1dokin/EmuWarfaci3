using System;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000328 RID: 808
	public class AchievementItemValidationException : ApplicationException
	{
		// Token: 0x0600124A RID: 4682 RVA: 0x0004904C File Offset: 0x0004744C
		public AchievementItemValidationException(string formatMessage, params object[] formatParams) : base(string.Format(formatMessage, formatParams))
		{
		}
	}
}
