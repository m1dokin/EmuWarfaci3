using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.GameLogic.InvitationSystem;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003D6 RID: 982
	[NotificationSerializerAttributes(NotificationType = ENotificationType.ClanInviteResult)]
	internal class ClanInviteResultNotificationSerializer : INotificationSerializer
	{
		// Token: 0x06001586 RID: 5510 RVA: 0x0005A3B4 File Offset: 0x000587B4
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			return Utils.GetTypeFromByteArray<SInvitationResult>(data).ToXml(element.OwnerDocument);
		}

		// Token: 0x06001587 RID: 5511 RVA: 0x0005A3D5 File Offset: 0x000587D5
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<SInvitationResult>(data);
		}
	}
}
