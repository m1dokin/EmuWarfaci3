using System;
using System.Xml;

namespace MasterServer.GameLogic.InvitationSystem
{
	// Token: 0x02000311 RID: 785
	[Serializable]
	public struct SInvitationClanData
	{
		// Token: 0x06001201 RID: 4609 RVA: 0x000475C8 File Offset: 0x000459C8
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("invitation");
			xmlElement.SetAttribute("clan_id", this.clan_id.ToString());
			xmlElement.AppendChild(this.Initiator.ToXml(factory));
			return xmlElement;
		}

		// Token: 0x06001202 RID: 4610 RVA: 0x00047614 File Offset: 0x00045A14
		public override string ToString()
		{
			return string.Format("initiator: {0}, target: {1}, clanId: {2}, clan: {3}", new object[]
			{
				this.Initiator.Nickname,
				this.reciever_name,
				this.clan_id,
				this.Initiator.ClanName
			});
		}

		// Token: 0x04000828 RID: 2088
		public ulong clan_id;

		// Token: 0x04000829 RID: 2089
		public ulong target_id;

		// Token: 0x0400082A RID: 2090
		public string reciever_name;

		// Token: 0x0400082B RID: 2091
		internal CommonInitiatorData Initiator;
	}
}
