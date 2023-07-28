using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.DAL.RatingSystem;

namespace MasterServer.GameLogic.NotificationSystem.NotificationSerializers
{
	// Token: 0x020000B7 RID: 183
	[NotificationSerializerAttributes(NotificationType = ENotificationType.RatingGameBan)]
	internal class RatingGameBanPlayerNotificationSerializer : INotificationSerializer
	{
		// Token: 0x060002F1 RID: 753 RVA: 0x0000E004 File Offset: 0x0000C404
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			RatingGameBanPlayerNotification typeFromByteArray = Utils.GetTypeFromByteArray<RatingGameBanPlayerNotification>(data);
			return typeFromByteArray.ToXml(element.OwnerDocument);
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x0000E024 File Offset: 0x0000C424
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<RatingGamePlayerBanInfo>(data);
		}
	}
}
