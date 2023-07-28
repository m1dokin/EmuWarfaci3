using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x0200027D RID: 637
	[QueryAttributes(TagName = "clan_members_updated")]
	internal class ClanMemberUpdatedQuery : BaseQuery
	{
		// Token: 0x06000DF9 RID: 3577 RVA: 0x0003859C File Offset: 0x0003699C
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			string value = queryParams[0] as string;
			IEnumerable<SClanMemberUpdate> enumerable = queryParams[1] as IEnumerable<SClanMemberUpdate>;
			IEnumerable<ProfileInfo> source = queryParams[2] as IEnumerable<ProfileInfo>;
			request.SetAttribute("bcast_receivers", value);
			using (IEnumerator<SClanMemberUpdate> enumerator = enumerable.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SClanMemberUpdate update = enumerator.Current;
					XmlElement xmlElement = request.OwnerDocument.CreateElement("update");
					request.AppendChild(xmlElement);
					xmlElement.SetAttribute("profile_id", update.member_info.ProfileID.ToString());
					if (update.member_info != null && update.update_type != EMembersListUpdate.Remove)
					{
						ProfileInfo pi = source.First((ProfileInfo p) => p.ProfileID == update.member_info.ProfileID);
						ClanMemberInfo clanMemberInfo = new ClanMemberInfo(pi, update.member_info);
						xmlElement.AppendChild(clanMemberInfo.ToXml(request.OwnerDocument));
					}
				}
			}
		}
	}
}
