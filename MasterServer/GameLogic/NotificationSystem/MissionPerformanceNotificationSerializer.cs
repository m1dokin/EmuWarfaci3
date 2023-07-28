using System;
using System.Xml;
using MasterServer.Common;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003E1 RID: 993
	[NotificationSerializerAttributes(NotificationType = ENotificationType.MissionPerformance)]
	internal class MissionPerformanceNotificationSerializer : INotificationSerializer
	{
		// Token: 0x060015A4 RID: 5540 RVA: 0x0005A59C File Offset: 0x0005899C
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			string typeFromByteArray = Utils.GetTypeFromByteArray<string>(data);
			XmlElement xmlElement = element.OwnerDocument.CreateElement("mission_performance");
			xmlElement.SetAttribute("data", typeFromByteArray);
			return xmlElement;
		}

		// Token: 0x060015A5 RID: 5541 RVA: 0x0005A5CE File Offset: 0x000589CE
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<string>(data);
		}
	}
}
