using System;
using System.Xml;
using MasterServer.DAL;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.NotificationSystem;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x0200023D RID: 573
	internal class RandomBoxPurchaseHandler : PurchaseHandler
	{
		// Token: 0x06000C52 RID: 3154 RVA: 0x00030554 File Offset: 0x0002E954
		private RandomBoxPurchaseHandler(ulong profileId, XmlElement node, XmlElement userData = null) : base(profileId, node)
		{
			this.m_node = node;
			if (userData != null)
			{
				XmlNode newChild = this.m_node.OwnerDocument.ImportNode(userData, true);
				this.m_node.AppendChild(newChild);
			}
		}

		// Token: 0x06000C53 RID: 3155 RVA: 0x00030598 File Offset: 0x0002E998
		public static RandomBoxPurchaseHandler Create(ulong profileId, XmlElement userData = null)
		{
			XmlElement node = RandomBoxPurchaseHandler.CreateNode();
			return new RandomBoxPurchaseHandler(profileId, node, userData);
		}

		// Token: 0x06000C54 RID: 3156 RVA: 0x000305B4 File Offset: 0x0002E9B4
		public override SNotification CreateNotification(OfferItem givenItem, string message = "", bool notify = true)
		{
			this.m_node.SetAttribute("name", givenItem.Item.Name);
			this.m_node.SetAttribute("notify", (!notify) ? "0" : "1");
			return NotificationFactory.CreateNotification<string>(ENotificationType.RandomBoxGiven, this.m_node.OuterXml, TimeSpan.FromDays(1.0), EConfirmationType.Confirmation, message);
		}

		// Token: 0x06000C55 RID: 3157 RVA: 0x00030629 File Offset: 0x0002EA29
		private static XmlElement CreateNode()
		{
			return new XmlDocument().CreateElement("give_random_box");
		}

		// Token: 0x040005B3 RID: 1459
		private readonly XmlElement m_node;
	}
}
