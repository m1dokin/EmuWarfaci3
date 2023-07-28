using System;
using System.Xml;

namespace MasterServer.GameLogic.InvitationSystem
{
	// Token: 0x02000312 RID: 786
	[Serializable]
	public struct SInvitationFriendData
	{
		// Token: 0x06001203 RID: 4611 RVA: 0x00047664 File Offset: 0x00045A64
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("invitation");
			xmlElement.SetAttribute("target", this.RecieverName);
			xmlElement.AppendChild(this.Initiator.ToXml(factory));
			return xmlElement;
		}

		// Token: 0x06001204 RID: 4612 RVA: 0x000476A2 File Offset: 0x00045AA2
		public override string ToString()
		{
			return string.Format("initiator: {0}, target: {1}", this.Initiator.Nickname, this.RecieverName);
		}

		// Token: 0x0400082C RID: 2092
		public ulong TargetId;

		// Token: 0x0400082D RID: 2093
		public ulong TargetUserId;

		// Token: 0x0400082E RID: 2094
		public string RecieverName;

		// Token: 0x0400082F RID: 2095
		internal CommonInitiatorData Initiator;
	}
}
