using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.GameLogic.InvitationSystem;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003D5 RID: 981
	[NotificationSerializerAttributes(NotificationType = ENotificationType.ClanInvite)]
	internal class ClanInviteNotificationSerializer : INotificationSerializer
	{
		// Token: 0x06001583 RID: 5507 RVA: 0x0005A37C File Offset: 0x0005877C
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			return Utils.GetTypeFromByteArray<SInvitationClanData>(data).ToXml(element.OwnerDocument);
		}

		// Token: 0x06001584 RID: 5508 RVA: 0x0005A39D File Offset: 0x0005879D
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<SInvitationClanData>(data);
		}
	}
}
