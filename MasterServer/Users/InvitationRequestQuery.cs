using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.InvitationSystem;
using MasterServer.GameRoomSystem;

namespace MasterServer.Users
{
	// Token: 0x020007DA RID: 2010
	[QueryAttributes(TagName = "invitation_request")]
	internal class InvitationRequestQuery : BaseQuery
	{
		// Token: 0x06002913 RID: 10515 RVA: 0x000B2104 File Offset: 0x000B0504
		public InvitationRequestQuery(IGameRoomManager gameRoomMgr)
		{
			this.m_gameRoomManager = gameRoomMgr;
		}

		// Token: 0x06002914 RID: 10516 RVA: 0x000B2114 File Offset: 0x000B0514
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			string value = queryParams[0].ToString();
			string value2 = queryParams[1].ToString();
			ulong room_id = (ulong)queryParams[2];
			string text = queryParams[3].ToString();
			string value3 = queryParams[4].ToString();
			request.RemoveAllAttributes();
			XmlElement newChild = ((CommonInitiatorData)queryParams[5]).ToXml(request.OwnerDocument);
			request.SetAttribute("from", value);
			request.SetAttribute("ticket", value2);
			request.SetAttribute("room_id", room_id.ToString());
			request.SetAttribute("ms_resource", Resources.XmppResource);
			request.SetAttribute("is_follow", text);
			request.SetAttribute("group_id", value3);
			request.AppendChild(newChild);
			IGameRoom room = this.m_gameRoomManager.GetRoom(room_id);
			if (text == "0")
			{
				if (room == null)
				{
					throw new RoomNotFoundException(room_id);
				}
				XmlElement newChild2 = room.FullStateSnapshot(RoomUpdate.Target.Client, request.OwnerDocument);
				request.AppendChild(newChild2);
			}
		}

		// Token: 0x040015E6 RID: 5606
		public const string QueryName = "invitation_request";

		// Token: 0x040015E7 RID: 5607
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
