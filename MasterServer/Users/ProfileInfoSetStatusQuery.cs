using System;
using System.Xml;
using MasterServer.CryOnlineNET;
using Util.Common;

namespace MasterServer.Users
{
	// Token: 0x020007F4 RID: 2036
	[QueryAttributes(TagName = "profile_info_set_status")]
	internal class ProfileInfoSetStatusQuery : BaseQuery
	{
		// Token: 0x060029DD RID: 10717 RVA: 0x000B4718 File Offset: 0x000B2B18
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			ProfileInfo profileInfo = (ProfileInfo)queryParams[0];
			request.SetAttribute("nickname", profileInfo.Nickname);
			request.SetAttribute("online_id", profileInfo.OnlineID);
			request.SetAttribute("profile_id", profileInfo.ProfileID.ToString());
			request.SetAttribute("rank", profileInfo.RankId.ToString());
			request.SetAttribute("login_time", TimeUtils.LocalTimeToUTCTimestamp(profileInfo.LoginTime).ToString());
		}
	}
}
