using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000770 RID: 1904
	[QueryAttributes(TagName = "srv_player_kicked")]
	internal class SrvPlayerKickedQuery : BaseQuery
	{
		// Token: 0x06002781 RID: 10113 RVA: 0x000A84D4 File Offset: 0x000A68D4
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			ulong num = (ulong)queryParams[0];
			GameRoomPlayerRemoveReason gameRoomPlayerRemoveReason = (GameRoomPlayerRemoveReason)queryParams[1];
			request.SetAttribute("profile_id", num.ToString());
			string name = "reason";
			int num2 = (int)gameRoomPlayerRemoveReason;
			request.SetAttribute(name, num2.ToString());
		}
	}
}
