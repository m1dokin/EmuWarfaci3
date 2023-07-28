using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.GameLogic.InvitationSystem;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003D8 RID: 984
	[NotificationSerializerAttributes(NotificationType = ENotificationType.FriendInvite)]
	internal class FriendInviteNotificationSerializer : INotificationSerializer
	{
		// Token: 0x0600158C RID: 5516 RVA: 0x0005A41C File Offset: 0x0005881C
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			return Utils.GetTypeFromByteArray<SInvitationFriendData>(data).ToXml(element.OwnerDocument);
		}

		// Token: 0x0600158D RID: 5517 RVA: 0x0005A43D File Offset: 0x0005883D
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<SInvitationFriendData>(data);
		}
	}
}
