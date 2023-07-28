using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200076F RID: 1903
	[QueryAttributes(TagName = "gameroom_on_kicked")]
	internal class OnKickedQuery : BaseQuery
	{
		// Token: 0x0600277F RID: 10111 RVA: 0x000A849C File Offset: 0x000A689C
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			GameRoomPlayerRemoveReason gameRoomPlayerRemoveReason = (GameRoomPlayerRemoveReason)queryParams[0];
			string name = "reason";
			int num = (int)gameRoomPlayerRemoveReason;
			request.SetAttribute(name, num.ToString());
		}
	}
}
