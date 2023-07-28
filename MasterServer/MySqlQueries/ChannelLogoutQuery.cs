using System;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.MySqlQueries
{
	// Token: 0x02000681 RID: 1665
	[QueryAttributes(TagName = "channel_logout")]
	internal class ChannelLogoutQuery : BaseQuery
	{
		// Token: 0x0600230E RID: 8974 RVA: 0x00093498 File Offset: 0x00091898
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User userByOnlineId = base.UserRepository.GetUserByOnlineId(fromJid);
			if (userByOnlineId != null)
			{
				base.UserRepository.UserLogout(userByOnlineId, ELogoutType.ChannelSwitch);
			}
			return 0;
		}
	}
}
