using System;
using System.Xml;
using MasterServer.Users;

namespace MasterServer.GameLogic.InvitationSystem
{
	// Token: 0x02000313 RID: 787
	[Serializable]
	public struct SInvitationResult
	{
		// Token: 0x06001205 RID: 4613 RVA: 0x000476C0 File Offset: 0x00045AC0
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("invite_result");
			xmlElement.SetAttribute("profile_id", this.ProfileId.ToString());
			xmlElement.SetAttribute("jid", this.OnlineID);
			xmlElement.SetAttribute("nickname", this.Nickname);
			XmlElement xmlElement2 = xmlElement;
			string name = "status";
			int status = (int)this.Status;
			xmlElement2.SetAttribute(name, status.ToString());
			xmlElement.SetAttribute("location", this.Location);
			xmlElement.SetAttribute("experience", this.Experience.ToString());
			XmlElement xmlElement3 = xmlElement;
			string name2 = "result";
			int result = (int)this.Result;
			xmlElement3.SetAttribute(name2, result.ToString());
			xmlElement.SetAttribute("invite_date", this.InvitationDate.ToString());
			return xmlElement;
		}

		// Token: 0x06001206 RID: 4614 RVA: 0x0004779F File Offset: 0x00045B9F
		public override string ToString()
		{
			return string.Format("Nickname: {0}, Result: {1}", this.Nickname, this.Result.ToString());
		}

		// Token: 0x04000830 RID: 2096
		public ulong ProfileId;

		// Token: 0x04000831 RID: 2097
		public string OnlineID;

		// Token: 0x04000832 RID: 2098
		public string Nickname;

		// Token: 0x04000833 RID: 2099
		public UserStatus Status;

		// Token: 0x04000834 RID: 2100
		public string Location;

		// Token: 0x04000835 RID: 2101
		public ulong Experience;

		// Token: 0x04000836 RID: 2102
		public ulong InvitationDate;

		// Token: 0x04000837 RID: 2103
		public EInviteStatus Result;
	}
}
