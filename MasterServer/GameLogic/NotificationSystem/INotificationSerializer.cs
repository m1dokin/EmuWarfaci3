using System;
using System.Xml;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003DB RID: 987
	public interface INotificationSerializer
	{
		// Token: 0x06001592 RID: 5522
		XmlElement Serialize(byte[] data, XmlElement element);

		// Token: 0x06001593 RID: 5523
		object Deserialize(byte[] data);
	}
}
