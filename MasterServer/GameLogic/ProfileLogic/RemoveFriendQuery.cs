using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000551 RID: 1361
	[QueryAttributes(TagName = "remove_friend")]
	internal class RemoveFriendQuery : BaseQuery
	{
		// Token: 0x06001D50 RID: 7504 RVA: 0x00076848 File Offset: 0x00074C48
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User initiator;
			if (!base.GetClientInfo(fromJid, out initiator))
			{
				return -3;
			}
			string attribute = request.GetAttribute("target");
			response.SetAttribute("target", attribute);
			IFriendsService service = ServicesManager.GetService<IFriendsService>();
			service.RemoveFriend(initiator, attribute);
			return 0;
		}
	}
}
