using System;
using System.Xml;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003CF RID: 975
	[Serializable]
	internal class ItemUnlockedNotification
	{
		// Token: 0x0600156E RID: 5486 RVA: 0x0005A14B File Offset: 0x0005854B
		public ItemUnlockedNotification(string itemName)
		{
			this.m_itemName = itemName;
		}

		// Token: 0x0600156F RID: 5487 RVA: 0x0005A15C File Offset: 0x0005855C
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("item_unlocked");
			xmlElement.SetAttribute("item_name", this.m_itemName);
			return xmlElement;
		}

		// Token: 0x04000A4D RID: 2637
		private readonly string m_itemName;
	}
}
