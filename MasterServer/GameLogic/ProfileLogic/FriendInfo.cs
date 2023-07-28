using System;
using System.Globalization;
using System.Xml;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000552 RID: 1362
	public struct FriendInfo
	{
		// Token: 0x06001D51 RID: 7505 RVA: 0x0007688D File Offset: 0x00074C8D
		public FriendInfo(ProfileInfo pi, ulong exp)
		{
			this.Profile = pi;
			this.Experience = exp;
		}

		// Token: 0x06001D52 RID: 7506 RVA: 0x000768A0 File Offset: 0x00074CA0
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("friend");
			xmlElement.SetAttribute("jid", this.Profile.OnlineID ?? string.Empty);
			xmlElement.SetAttribute("profile_id", this.Profile.ProfileID.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("nickname", this.Profile.Nickname ?? string.Empty);
			XmlElement xmlElement2 = xmlElement;
			string name = "status";
			int status = (int)this.Profile.Status;
			xmlElement2.SetAttribute(name, status.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("experience", this.Experience.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("location", this.Profile.Location);
			return xmlElement;
		}

		// Token: 0x04000DFE RID: 3582
		public ProfileInfo Profile;

		// Token: 0x04000DFF RID: 3583
		public ulong Experience;
	}
}
