using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Database;

namespace MasterServer.Users
{
	// Token: 0x020007D8 RID: 2008
	[QueryAttributes(TagName = "get_last_seen_date")]
	internal class GetLastSeenDateQuery : BaseQuery
	{
		// Token: 0x0600290F RID: 10511 RVA: 0x000B1FDC File Offset: 0x000B03DC
		public GetLastSeenDateQuery(IDALService dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x06002910 RID: 10512 RVA: 0x000B1FEC File Offset: 0x000B03EC
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "GetLastSeenDateQuery"))
			{
				ulong profileId = ulong.Parse(request.GetAttribute("profile_id"));
				ulong lastSeenDate = this.m_dal.ProfileSystem.GetLastSeenDate(profileId);
				response.SetAttribute("profile_id", profileId.ToString());
				response.SetAttribute("last_seen", lastSeenDate.ToString());
				result = 0;
			}
			return result;
		}

		// Token: 0x040015E5 RID: 5605
		private readonly IDALService m_dal;
	}
}
