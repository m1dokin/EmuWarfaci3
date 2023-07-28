using System;
using System.Xml;
using MasterServer.DAL;
using MasterServer.Users;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x02000275 RID: 629
	internal class ClanMemberInfo
	{
		// Token: 0x06000D8F RID: 3471 RVA: 0x000368CC File Offset: 0x00034CCC
		public ClanMemberInfo(ProfileInfo pi, ClanMember cm)
		{
			this.m_profileInfo = pi;
			this.m_clanMember = cm;
		}

		// Token: 0x06000D90 RID: 3472 RVA: 0x000368E4 File Offset: 0x00034CE4
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("clan_member_info");
			xmlElement.SetAttribute("nickname", this.m_clanMember.Nickname);
			xmlElement.SetAttribute("profile_id", this.m_profileInfo.ProfileID.ToString());
			xmlElement.SetAttribute("experience", this.m_clanMember.Expirience.ToString());
			xmlElement.SetAttribute("clan_points", this.m_clanMember.ClanPoints.ToString());
			xmlElement.SetAttribute("invite_date", this.m_clanMember.InviteDate.ToString());
			XmlElement xmlElement2 = xmlElement;
			string name = "clan_role";
			int clanRole = (int)this.m_clanMember.ClanRole;
			xmlElement2.SetAttribute(name, clanRole.ToString());
			xmlElement.SetAttribute("jid", this.m_profileInfo.OnlineID ?? string.Empty);
			XmlElement xmlElement3 = xmlElement;
			string name2 = "status";
			int status = (int)this.m_profileInfo.Status;
			xmlElement3.SetAttribute(name2, status.ToString());
			return xmlElement;
		}

		// Token: 0x04000654 RID: 1620
		private readonly ClanMember m_clanMember;

		// Token: 0x04000655 RID: 1621
		private readonly ProfileInfo m_profileInfo;
	}
}
