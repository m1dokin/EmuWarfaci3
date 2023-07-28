using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004A9 RID: 1193
	[QueryAttributes(TagName = "pause_game_session")]
	internal class PauseGameSessionQuery : BaseQuery
	{
		// Token: 0x06001964 RID: 6500 RVA: 0x000671B4 File Offset: 0x000655B4
		public override void SendRequest(string onlineId, XmlElement request, params object[] args)
		{
			string value = args[0].ToString();
			request.SetAttribute("session_id", value);
		}

		// Token: 0x04000C27 RID: 3111
		public const string QueryName = "pause_game_session";
	}
}
