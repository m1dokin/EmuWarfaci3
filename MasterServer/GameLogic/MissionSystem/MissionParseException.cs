using System;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003AB RID: 939
	internal class MissionParseException : MissionSystemException
	{
		// Token: 0x060014CB RID: 5323 RVA: 0x000557A4 File Offset: 0x00053BA4
		public MissionParseException(string message) : base(message)
		{
		}

		// Token: 0x060014CC RID: 5324 RVA: 0x000557AD File Offset: 0x00053BAD
		public MissionParseException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
