using System;
using System.Runtime.Serialization;

namespace MasterServer.Platform.ProfanityCheck.Exceptions
{
	// Token: 0x020006A2 RID: 1698
	[Serializable]
	internal class ProfanityServiceException : Exception
	{
		// Token: 0x060023A8 RID: 9128 RVA: 0x00095ECE File Offset: 0x000942CE
		public ProfanityServiceException() : this(string.Empty)
		{
		}

		// Token: 0x060023A9 RID: 9129 RVA: 0x00095EDB File Offset: 0x000942DB
		public ProfanityServiceException(string message) : this(message, null)
		{
		}

		// Token: 0x060023AA RID: 9130 RVA: 0x00095EE5 File Offset: 0x000942E5
		public ProfanityServiceException(string message, Exception inner) : base(message, inner)
		{
		}

		// Token: 0x060023AB RID: 9131 RVA: 0x00095EEF File Offset: 0x000942EF
		public ProfanityServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
