using System;
using System.Globalization;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000525 RID: 1317
	[QueryAttributes(TagName = "gameroom_offer")]
	internal class GameRoomOfferQuery : BaseQuery
	{
		// Token: 0x06001CA1 RID: 7329 RVA: 0x00072B28 File Offset: 0x00070F28
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			string value = queryParams[0].ToString();
			GameRoom gameRoom = (GameRoom)queryParams[1];
			Guid guid = (Guid)queryParams[2];
			string value2 = (string)queryParams[3];
			bool flag = (bool)queryParams[4];
			XmlElement newChild = gameRoom.FullStateSnapshot(RoomUpdate.Target.Client, request.OwnerDocument);
			request.RemoveAllAttributes();
			request.SetAttribute("from", value);
			request.SetAttribute("room_id", gameRoom.ID.ToString(CultureInfo.InvariantCulture));
			request.SetAttribute("token", value2);
			request.SetAttribute("ms_resource", Resources.XmppResource);
			request.SetAttribute("id", guid.ToString());
			request.SetAttribute("silent", (!flag) ? "0" : "1");
			request.AppendChild(newChild);
		}

		// Token: 0x04000D9E RID: 3486
		public const string QueryName = "gameroom_offer";
	}
}
