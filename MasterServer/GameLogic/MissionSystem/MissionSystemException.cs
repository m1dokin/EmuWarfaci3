using System;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003AC RID: 940
	internal class MissionSystemException : Exception
	{
		// Token: 0x060014CD RID: 5325 RVA: 0x00055791 File Offset: 0x00053B91
		public MissionSystemException(string message) : base(message)
		{
		}

		// Token: 0x060014CE RID: 5326 RVA: 0x0005579A File Offset: 0x00053B9A
		public MissionSystemException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
