using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.PerformanceSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x0200027E RID: 638
	[QueryAttributes(TagName = "clan_list")]
	internal class GetClansListQuery : BaseQuery
	{
		// Token: 0x06000DFB RID: 3579 RVA: 0x000386F0 File Offset: 0x00036AF0
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User user;
			if (!base.GetClientInfo(fromJid, out user))
			{
				return -3;
			}
			IClanPerformanceService service = ServicesManager.GetService<IClanPerformanceService>();
			IDALService service2 = ServicesManager.GetService<IDALService>();
			ClanMember memberInfo = service2.ClanSystem.GetMemberInfo(user.ProfileID);
			ClanPerformanceInfo clanPerformance = service.GetClanPerformance((memberInfo == null) ? 0UL : memberInfo.ClanID);
			response.AppendChild(clanPerformance.ToXml(response.OwnerDocument));
			return 0;
		}
	}
}
