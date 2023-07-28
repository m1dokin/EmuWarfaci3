using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.GameLogic.InvitationSystem;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003D9 RID: 985
	[NotificationSerializerAttributes(NotificationType = ENotificationType.FriendInviteResult)]
	internal class FriendInviteResultNotificationSerializer : INotificationSerializer
	{
		// Token: 0x0600158F RID: 5519 RVA: 0x0005A454 File Offset: 0x00058854
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			return Utils.GetTypeFromByteArray<SInvitationResult>(data).ToXml(element.OwnerDocument);
		}

		// Token: 0x06001590 RID: 5520 RVA: 0x0005A475 File Offset: 0x00058875
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<SInvitationResult>(data);
		}
	}
}
