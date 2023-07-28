using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.CryOnlineNET;
using Util.Common;

namespace MasterServer.Users
{
	// Token: 0x020007F5 RID: 2037
	[QueryAttributes(TagName = "profile_info_get_status", CompressionType = ECompressType.eCS_NoCompression)]
	internal class ProfileInfoGetStatusQuery : BaseQuery
	{
		// Token: 0x060029DF RID: 10719 RVA: 0x000B47BC File Offset: 0x000B2BBC
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			IEnumerable<string> source = (IEnumerable<string>)queryParams[0];
			IEnumerable<ulong> source2 = (IEnumerable<ulong>)queryParams[1];
			string value = string.Join(",", source.ToArray<string>());
			request.SetAttribute("nickname", value);
			string value2 = string.Join(",", (from x in source2
			select x.ToString()).ToArray<string>());
			request.SetAttribute("profileId", value2);
		}

		// Token: 0x060029E0 RID: 10720 RVA: 0x000B4838 File Offset: 0x000B2C38
		public override object HandleResponse(SOnlineQuery query, XmlElement response)
		{
			List<ProfileInfo> list = new List<ProfileInfo>();
			IEnumerator enumerator = response.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlElement xmlElement = (XmlElement)obj;
					if (!(xmlElement.Name != "profile_info"))
					{
						list.AddRange(from XmlElement pel in xmlElement.ChildNodes
						select new ProfileInfo
						{
							Nickname = pel.GetAttribute("nickname"),
							Status = (UserStatus)int.Parse(pel.GetAttribute("status")),
							ProfileID = ulong.Parse(pel.GetAttribute("profile_id")),
							OnlineID = pel.GetAttribute("online_id"),
							UserID = ulong.Parse(pel.GetAttribute("user_id")),
							IPAddress = pel.GetAttribute("ip_address"),
							RankId = int.Parse(pel.GetAttribute("rank")),
							LoginTime = TimeUtils.UTCTimestampToUTCTime(ulong.Parse(pel.GetAttribute("login_time")))
						});
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return list;
		}

		// Token: 0x04001626 RID: 5670
		public const string QueryName = "profile_info_get_status";
	}
}
