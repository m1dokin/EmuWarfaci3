using System;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Users;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x0200027C RID: 636
	[QueryAttributes(TagName = "clan_create")]
	internal class CreateClanQuery : BaseQuery
	{
		// Token: 0x06000DF6 RID: 3574 RVA: 0x000384C4 File Offset: 0x000368C4
		public CreateClanQuery(IClanService clanService)
		{
			this.m_clanService = clanService;
		}

		// Token: 0x06000DF7 RID: 3575 RVA: 0x000384D4 File Offset: 0x000368D4
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User user;
			if (!base.GetClientInfo(fromJid, out user))
			{
				return -3;
			}
			string attribute = request.GetAttribute("clan_name");
			string attribute2 = request.GetAttribute("description");
			ulong clan_id = 0UL;
			EClanCreationStatus eclanCreationStatus = this.m_clanService.CreateClan(user.ProfileID, ref clan_id, attribute, attribute2);
			if (eclanCreationStatus != EClanCreationStatus.Created)
			{
				return (int)eclanCreationStatus;
			}
			ClanInfo clanInfo = this.m_clanService.GetClanInfo(clan_id);
			XmlElement xmlElement = clanInfo.ToXml(response.OwnerDocument, true);
			ProfileInfo pi = new ProfileInfo(user);
			ClanMember memberInfo = this.m_clanService.GetMemberInfo(user.ProfileID);
			ClanMemberInfo clanMemberInfo = new ClanMemberInfo(pi, memberInfo);
			xmlElement.AppendChild(clanMemberInfo.ToXml(response.OwnerDocument));
			response.AppendChild(xmlElement);
			return 0;
		}

		// Token: 0x0400066D RID: 1645
		private readonly IClanService m_clanService;
	}
}
