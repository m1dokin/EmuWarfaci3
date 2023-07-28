using System;
using System.Xml;
using MasterServer.Common;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003E0 RID: 992
	[NotificationSerializerAttributes(NotificationType = ENotificationType.Message)]
	internal class MessageNotificationSerializer : INotificationSerializer
	{
		// Token: 0x060015A1 RID: 5537 RVA: 0x0000E11C File Offset: 0x0000C51C
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			string typeFromByteArray = Utils.GetTypeFromByteArray<string>(data);
			XmlElement xmlElement = element.OwnerDocument.CreateElement("message");
			xmlElement.SetAttribute("data", typeFromByteArray);
			return xmlElement;
		}

		// Token: 0x060015A2 RID: 5538 RVA: 0x0000E14E File Offset: 0x0000C54E
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<string>(data);
		}
	}
}
