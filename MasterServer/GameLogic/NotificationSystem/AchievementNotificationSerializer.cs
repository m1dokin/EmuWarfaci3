using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.GameLogic.Achievements;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003D2 RID: 978
	[NotificationSerializerAttributes(NotificationType = ENotificationType.Achievement)]
	internal class AchievementNotificationSerializer : INotificationSerializer
	{
		// Token: 0x0600157A RID: 5498 RVA: 0x0005A2E4 File Offset: 0x000586E4
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			return Utils.GetTypeFromByteArray<AchievementUpdateChunk>(data).ToXml(element.OwnerDocument);
		}

		// Token: 0x0600157B RID: 5499 RVA: 0x0005A305 File Offset: 0x00058705
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<AchievementUpdateChunk>(data);
		}
	}
}
