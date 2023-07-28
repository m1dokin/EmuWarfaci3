using System;
using System.Xml;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003CC RID: 972
	[Serializable]
	public class NewRankNotification
	{
		// Token: 0x06001567 RID: 5479 RVA: 0x0005A09F File Offset: 0x0005849F
		public NewRankNotification(int oldRank, int newRank)
		{
			this.m_oldRank = oldRank;
			this.m_newRank = newRank;
		}

		// Token: 0x06001568 RID: 5480 RVA: 0x0005A0B8 File Offset: 0x000584B8
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("new_rank_reached");
			xmlElement.SetAttribute("old_rank", this.m_oldRank.ToString());
			xmlElement.SetAttribute("new_rank", this.m_newRank.ToString());
			return xmlElement;
		}

		// Token: 0x04000A49 RID: 2633
		private readonly int m_oldRank;

		// Token: 0x04000A4A RID: 2634
		private readonly int m_newRank;
	}
}
