using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005ED RID: 1517
	[QueryAttributes(TagName = "gameroom_loosemaster")]
	internal class LooseMasterQuery : BaseQuery
	{
		// Token: 0x06002046 RID: 8262 RVA: 0x000829FC File Offset: 0x00080DFC
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			request.SetAttribute("time", queryParams[0].ToString());
		}
	}
}
