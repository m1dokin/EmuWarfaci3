using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.GameLogic.ContractSystem;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003D7 RID: 983
	[NotificationSerializerAttributes(NotificationType = ENotificationType.Contract)]
	internal class ContractNotificationSerializer : INotificationSerializer
	{
		// Token: 0x06001589 RID: 5513 RVA: 0x0005A3EC File Offset: 0x000587EC
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			ContractNotification typeFromByteArray = Utils.GetTypeFromByteArray<ContractNotification>(data);
			return typeFromByteArray.ToXml(element.OwnerDocument);
		}

		// Token: 0x0600158A RID: 5514 RVA: 0x0005A40C File Offset: 0x0005880C
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<ContractNotification>(data);
		}
	}
}
