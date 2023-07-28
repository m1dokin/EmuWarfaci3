using System;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x02000281 RID: 641
	[QueryAttributes(TagName = "clan_kick")]
	internal class KickFromClanQuery : BaseQuery
	{
		// Token: 0x06000E00 RID: 3584 RVA: 0x00038844 File Offset: 0x00036C44
		public KickFromClanQuery(IClanService clanService)
		{
			this.m_clanService = clanService;
		}

		// Token: 0x06000E01 RID: 3585 RVA: 0x00038854 File Offset: 0x00036C54
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User user;
			if (!base.GetClientInfo(fromJid, out user))
			{
				return -3;
			}
			ulong target_id = ulong.Parse(request.GetAttribute("profile_id"));
			if (!this.m_clanService.KickFromClan(user.ProfileID, target_id))
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x0400066F RID: 1647
		private readonly IClanService m_clanService;
	}
}
