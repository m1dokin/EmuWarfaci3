using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x02000282 RID: 642
	[QueryAttributes(TagName = "set_clan_info")]
	internal class SetClanInfoQuery : BaseQuery
	{
		// Token: 0x06000E02 RID: 3586 RVA: 0x0003889D File Offset: 0x00036C9D
		public SetClanInfoQuery(IClanService clanService)
		{
			this.m_clanService = clanService;
		}

		// Token: 0x06000E03 RID: 3587 RVA: 0x000388AC File Offset: 0x00036CAC
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "SetClanInfoQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					string attribute = request.GetAttribute("description");
					result = (this.m_clanService.SetClanInfo(user.ProfileID, attribute) ? 0 : -1);
				}
			}
			return result;
		}

		// Token: 0x04000670 RID: 1648
		private readonly IClanService m_clanService;
	}
}
