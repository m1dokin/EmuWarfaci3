using System;
using MasterServer.DAL.Utils;
using Util.Common;

namespace MasterServer.DAL
{
	// Token: 0x02000079 RID: 121
	[Serializable]
	public class TouchProfileResult
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000156 RID: 342 RVA: 0x00004C9E File Offset: 0x0000309E
		public DateTime PreviousLastSeenTime
		{
			get
			{
				return TimeUtils.UTCTimestampToLocalTime(this.PreviousLastSeenTimeUTC);
			}
		}

		// Token: 0x04000140 RID: 320
		public ETouchProfileResult Status;

		// Token: 0x04000141 RID: 321
		public DBVersion DataVersion;

		// Token: 0x04000142 RID: 322
		public ulong PreviousLastSeenTimeUTC;
	}
}
