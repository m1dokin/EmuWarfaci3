using System;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem.Exceptions
{
	// Token: 0x0200007E RID: 126
	public class UnknownGameModeException : FirstWinOfDayByModeSystemException
	{
		// Token: 0x060001DC RID: 476 RVA: 0x0000B46D File Offset: 0x0000986D
		public UnknownGameModeException(string mode) : base(UnknownGameModeException.GetExceptionMessage(mode))
		{
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000B47B File Offset: 0x0000987B
		private static string GetExceptionMessage(string mode)
		{
			return string.Format("Unknown game mode '{0}' in GameModeFirstWinOfDayBonus config in rewards_configuration.xml", mode);
		}
	}
}
