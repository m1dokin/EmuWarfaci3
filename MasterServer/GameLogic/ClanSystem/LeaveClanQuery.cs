using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x0200027F RID: 639
	[QueryAttributes(TagName = "clan_leave")]
	internal class LeaveClanQuery : BaseQuery
	{
		// Token: 0x06000DFD RID: 3581 RVA: 0x00038764 File Offset: 0x00036B64
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User user;
			if (!base.GetClientInfo(fromJid, out user))
			{
				return -3;
			}
			IClanService service = ServicesManager.GetService<IClanService>();
			if (!service.LeaveClan(user.ProfileID))
			{
				return -1;
			}
			return 0;
		}
	}
}
