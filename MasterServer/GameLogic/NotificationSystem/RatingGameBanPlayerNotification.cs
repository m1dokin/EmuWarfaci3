using System;
using System.Xml;
using MasterServer.DAL.RatingSystem;
using Util.Common;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x02000004 RID: 4
	[Serializable]
	internal class RatingGameBanPlayerNotification : RatingGamePlayerBanInfo
	{
		// Token: 0x06000009 RID: 9 RVA: 0x00004110 File Offset: 0x00002510
		public RatingGameBanPlayerNotification(TimeSpan banTimeout, string msg)
		{
			base.BanTimeout = (ulong)banTimeout.TotalSeconds;
			base.UnbanTime = DateTime.UtcNow.AddSeconds(base.BanTimeout);
			this.Message = msg;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00004153 File Offset: 0x00002553
		// (set) Token: 0x0600000B RID: 11 RVA: 0x0000415B File Offset: 0x0000255B
		public string Message { get; private set; }

		// Token: 0x0600000C RID: 12 RVA: 0x00004164 File Offset: 0x00002564
		public override XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = base.ToXml(factory);
			if (!this.Message.IsNullOrEmpty())
			{
				xmlElement.SetAttribute("ban_message", this.Message);
			}
			return xmlElement;
		}
	}
}
