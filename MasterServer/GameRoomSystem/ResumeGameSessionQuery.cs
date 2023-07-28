using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004AC RID: 1196
	[QueryAttributes(TagName = "resume_game_session")]
	internal class ResumeGameSessionQuery : BaseQuery
	{
		// Token: 0x0600196A RID: 6506 RVA: 0x00067328 File Offset: 0x00065728
		public override void SendRequest(string onlineId, XmlElement request, params object[] args)
		{
			string value = args[0].ToString();
			request.SetAttribute("session_id", value);
		}

		// Token: 0x04000C2B RID: 3115
		public const string QueryName = "resume_game_session";
	}
}
