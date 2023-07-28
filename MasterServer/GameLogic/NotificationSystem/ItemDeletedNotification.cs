using System;
using System.Xml;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020000B5 RID: 181
	[Serializable]
	internal class ItemDeletedNotification
	{
		// Token: 0x060002EB RID: 747 RVA: 0x0000DF7E File Offset: 0x0000C37E
		public ItemDeletedNotification(ulong profileItemID)
		{
			this.m_profileItemID = profileItemID;
		}

		// Token: 0x060002EC RID: 748 RVA: 0x0000DF90 File Offset: 0x0000C390
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("item_deleted");
			xmlElement.SetAttribute("profile_item_id", this.m_profileItemID.ToString());
			return xmlElement;
		}

		// Token: 0x04000142 RID: 322
		private readonly ulong m_profileItemID;
	}
}
