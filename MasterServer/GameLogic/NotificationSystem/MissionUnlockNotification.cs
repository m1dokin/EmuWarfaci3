using System;
using System.Xml;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020000B8 RID: 184
	[Serializable]
	public class MissionUnlockNotification
	{
		// Token: 0x060002F3 RID: 755 RVA: 0x0000E02C File Offset: 0x0000C42C
		public MissionUnlockNotification(string unlockedMission, bool silent)
		{
			this.m_unlockedMission = unlockedMission;
			this.m_silent = silent;
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x0000E044 File Offset: 0x0000C444
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("mission_unlocked_message");
			xmlElement.SetAttribute("silent", ((!this.m_silent) ? 0 : 1).ToString());
			xmlElement.SetAttribute("unlocked_mission", this.m_unlockedMission);
			return xmlElement;
		}

		// Token: 0x04000143 RID: 323
		private readonly string m_unlockedMission;

		// Token: 0x04000144 RID: 324
		private readonly bool m_silent;
	}
}
