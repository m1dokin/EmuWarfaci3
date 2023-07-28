using System;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem.Exceptions
{
	// Token: 0x02000080 RID: 128
	public class FirstWinOfDayByModeSystemException : Exception
	{
		// Token: 0x060001E0 RID: 480 RVA: 0x0000B433 File Offset: 0x00009833
		public FirstWinOfDayByModeSystemException(string message) : base(message)
		{
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000B43C File Offset: 0x0000983C
		public FirstWinOfDayByModeSystemException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
