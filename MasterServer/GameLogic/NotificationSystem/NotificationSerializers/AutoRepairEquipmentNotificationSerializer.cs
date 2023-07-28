using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.ElectronicCatalog;

namespace MasterServer.GameLogic.NotificationSystem.NotificationSerializers
{
	// Token: 0x020003D4 RID: 980
	[NotificationSerializerAttributes(NotificationType = ENotificationType.AutoRepairEquipment)]
	internal class AutoRepairEquipmentNotificationSerializer : INotificationSerializer
	{
		// Token: 0x06001580 RID: 5504 RVA: 0x0005A34C File Offset: 0x0005874C
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			RepairEquipmentOperationResult typeFromByteArray = Utils.GetTypeFromByteArray<RepairEquipmentOperationResult>(data);
			return typeFromByteArray.ToXml(element.OwnerDocument);
		}

		// Token: 0x06001581 RID: 5505 RVA: 0x0005A36C File Offset: 0x0005876C
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<RepairEquipmentOperationResult>(data);
		}
	}
}
