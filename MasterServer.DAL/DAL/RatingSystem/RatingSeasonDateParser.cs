using System;
using System.Globalization;

namespace MasterServer.DAL.RatingSystem
{
	// Token: 0x0200008F RID: 143
	public static class RatingSeasonDateParser
	{
		// Token: 0x060001AF RID: 431 RVA: 0x00005654 File Offset: 0x00003A54
		public static DateTime Parse(string dateString)
		{
			return DateTime.ParseExact(dateString, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
		}

		// Token: 0x0400017B RID: 379
		public const string DateFormat = "yyyy-MM-ddTHH:mm";
	}
}
