using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x02000284 RID: 644
	[QueryAttributes(TagName = "clan_remove")]
	internal class RemoveClanQuery : BaseQuery
	{
		// Token: 0x06000E07 RID: 3591 RVA: 0x00038960 File Offset: 0x00036D60
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User user;
			if (!base.GetClientInfo(fromJid, out user))
			{
				return -3;
			}
			IClanService service = ServicesManager.GetService<IClanService>();
			if (!service.RemoveClan(user.ProfileID))
			{
				return -1;
			}
			return 0;
		}
	}
}
