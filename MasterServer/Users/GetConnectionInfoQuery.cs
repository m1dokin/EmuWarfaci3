using System;
using System.Xml;
using MasterServer.CryOnlineNET;
using Util.Common;

namespace MasterServer.Users
{
	// Token: 0x020007F3 RID: 2035
	[QueryAttributes(TagName = "get_connection_info")]
	internal class GetConnectionInfoQuery : BaseQuery
	{
		// Token: 0x060029DA RID: 10714 RVA: 0x000B465B File Offset: 0x000B2A5B
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			request.SetAttribute("online_id", (string)queryParams[0]);
		}

		// Token: 0x060029DB RID: 10715 RVA: 0x000B4670 File Offset: 0x000B2A70
		public override object HandleResponse(SOnlineQuery query, XmlElement response)
		{
			ulong num = ulong.Parse(response.GetAttribute("login_time"));
			DateTime loginTime;
			try
			{
				loginTime = TimeUtils.UTCTimestampToUTCTime(num);
			}
			catch (ArgumentOutOfRangeException innerException)
			{
				throw new ArgumentOutOfRangeException(string.Format("Can't create LoginTime from timestamp {0}", num), innerException);
			}
			return new SessionInfo
			{
				UserID = ulong.Parse(response.GetAttribute("user_id")),
				IPAddress = response.GetAttribute("ip"),
				Tags = new UserTags(response.GetAttribute("tags")),
				LoginTime = loginTime
			};
		}
	}
}
