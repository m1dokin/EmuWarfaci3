using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005EE RID: 1518
	[QueryAttributes(TagName = "gameroom_on_expired")]
	internal class OnRoomExpiredQuery : BaseQuery
	{
		// Token: 0x06002048 RID: 8264 RVA: 0x00082A19 File Offset: 0x00080E19
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
		}
	}
}
