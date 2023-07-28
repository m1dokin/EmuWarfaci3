using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Users;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x02000280 RID: 640
	[QueryAttributes(TagName = "clan_set_member_role")]
	internal class SetClanRoleQuery : BaseQuery
	{
		// Token: 0x06000DFE RID: 3582 RVA: 0x0003879C File Offset: 0x00036B9C
		public SetClanRoleQuery(IClanService clanService)
		{
			this.m_clanService = clanService;
		}

		// Token: 0x06000DFF RID: 3583 RVA: 0x000387AC File Offset: 0x00036BAC
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "SetClanRoleQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					ulong target_id = ulong.Parse(request.GetAttribute("profile_id"));
					EClanRole role = (EClanRole)ulong.Parse(request.GetAttribute("role"));
					if (!this.m_clanService.SetClanRole(user.ProfileID, target_id, role))
					{
						result = -1;
					}
					else
					{
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x0400066E RID: 1646
		private readonly IClanService m_clanService;
	}
}
