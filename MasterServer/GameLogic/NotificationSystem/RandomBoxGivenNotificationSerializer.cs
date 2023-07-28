using System;
using System.Xml;
using MasterServer.Common;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003E4 RID: 996
	[NotificationSerializerAttributes(NotificationType = ENotificationType.RandomBoxGiven)]
	internal class RandomBoxGivenNotificationSerializer : INotificationSerializer
	{
		// Token: 0x060015AD RID: 5549 RVA: 0x0005A64C File Offset: 0x00058A4C
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			string typeFromByteArray = Utils.GetTypeFromByteArray<string>(data);
			element.OwnerDocument.LoadXml(typeFromByteArray);
			return element.OwnerDocument.DocumentElement;
		}

		// Token: 0x060015AE RID: 5550 RVA: 0x0005A677 File Offset: 0x00058A77
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<string>(data);
		}
	}
}
