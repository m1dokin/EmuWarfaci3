using System;
using System.Globalization;
using System.Xml;
using Util.Common;

namespace MasterServer.DAL.RatingSystem
{
	// Token: 0x0200008A RID: 138
	[Serializable]
	public class RatingGamePlayerBanInfo
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x000054A6 File Offset: 0x000038A6
		// (set) Token: 0x060001A3 RID: 419 RVA: 0x000054AE File Offset: 0x000038AE
		public ulong BanTimeout { get; set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060001A4 RID: 420 RVA: 0x000054B7 File Offset: 0x000038B7
		// (set) Token: 0x060001A5 RID: 421 RVA: 0x000054BF File Offset: 0x000038BF
		public DateTime UnbanTime { get; set; }

		// Token: 0x060001A6 RID: 422 RVA: 0x000054C8 File Offset: 0x000038C8
		public virtual XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("RatingGameBan");
			xmlElement.SetAttribute("ban_timeout", this.BanTimeout.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("unban_time", TimeUtils.LocalTimeToUTCTimestamp(this.UnbanTime).ToString(CultureInfo.InvariantCulture));
			return xmlElement;
		}
	}
}
