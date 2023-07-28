using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200080F RID: 2063
	[DebugQuery]
	[QueryAttributes(TagName = "room_command")]
	internal class RoomCommandQuery : BaseQuery
	{
		// Token: 0x06002A48 RID: 10824 RVA: 0x000B67BC File Offset: 0x000B4BBC
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			string value = queryParams[0].ToString();
			string value2 = queryParams[1].ToString();
			request.SetAttribute("bcast_receivers", value);
			request.SetAttribute("command", value2);
		}

		// Token: 0x04001694 RID: 5780
		public const string QueryName = "room_command";
	}
}
