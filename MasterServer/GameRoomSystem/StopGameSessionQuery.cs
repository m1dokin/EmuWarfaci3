using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004F0 RID: 1264
	[QueryAttributes(TagName = "stop_game_session")]
	internal class StopGameSessionQuery : BaseQuery
	{
		// Token: 0x06001B2A RID: 6954 RVA: 0x0006EE70 File Offset: 0x0006D270
		public override void SendRequest(string onlineId, XmlElement request, params object[] args)
		{
			string value = args[0].ToString();
			request.SetAttribute("session_id", value);
		}

		// Token: 0x04000CFF RID: 3327
		public const string QueryName = "stop_game_session";
	}
}
