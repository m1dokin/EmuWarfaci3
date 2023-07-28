using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000550 RID: 1360
	[QueryAttributes(TagName = "friend_list")]
	internal class FriendListQuery : BaseQuery
	{
		// Token: 0x06001D4E RID: 7502 RVA: 0x000767D4 File Offset: 0x00074BD4
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			IEnumerable<FriendInfo> enumerable = queryParams[0] as IEnumerable<FriendInfo>;
			foreach (FriendInfo friendInfo in enumerable)
			{
				XmlElement newChild = friendInfo.ToXml(request.OwnerDocument);
				request.AppendChild(newChild);
			}
		}

		// Token: 0x04000DFD RID: 3581
		public const string QueryName = "friend_list";
	}
}
