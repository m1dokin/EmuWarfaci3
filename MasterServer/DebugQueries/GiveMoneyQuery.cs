using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.Users;

namespace MasterServer.DebugQueries
{
	// Token: 0x02000224 RID: 548
	[DebugQuery]
	[QueryAttributes(TagName = "debug_give_money")]
	internal class GiveMoneyQuery : BaseQuery
	{
		// Token: 0x06000BCC RID: 3020 RVA: 0x0002CDDC File Offset: 0x0002B1DC
		public GiveMoneyQuery(ICatalogService catalogService)
		{
			this.m_catalogService = catalogService;
		}

		// Token: 0x06000BCD RID: 3021 RVA: 0x0002CDEC File Offset: 0x0002B1EC
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "GiveMoneyQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					string attribute = request.GetAttribute("money");
					string attribute2 = request.GetAttribute("currency");
					uint num = uint.Parse(attribute);
					Currency type = (Currency)uint.Parse(attribute2);
					this.m_catalogService.SetMoney(user.UserID, type, (ulong)num);
					response.SetAttribute("money", attribute);
					response.SetAttribute("currency", attribute2);
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x0400057C RID: 1404
		private readonly ICatalogService m_catalogService;
	}
}
