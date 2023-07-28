using System;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000192 RID: 402
	internal class QueryException : Exception
	{
		// Token: 0x06000761 RID: 1889 RVA: 0x0001C329 File Offset: 0x0001A729
		public QueryException(EOnlineError err, string message) : this(err, message, null)
		{
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x0001C334 File Offset: 0x0001A734
		public QueryException(EOnlineError err, string message, Exception inner) : base(string.Format("Query error: {0}", message), inner)
		{
			this.OnlineError = err;
			this.ErrorDescription = message;
		}

		// Token: 0x04000475 RID: 1141
		public readonly EOnlineError OnlineError;

		// Token: 0x04000476 RID: 1142
		public readonly string ErrorDescription;
	}
}
