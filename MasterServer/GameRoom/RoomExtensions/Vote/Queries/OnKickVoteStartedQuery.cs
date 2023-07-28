using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoom.RoomExtensions.Vote.Queries
{
	// Token: 0x020004DB RID: 1243
	[QueryAttributes(TagName = "on_kick_voting_started")]
	internal class OnKickVoteStartedQuery : BaseQuery
	{
		// Token: 0x06001AE2 RID: 6882 RVA: 0x0006E44C File Offset: 0x0006C84C
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			string value = (string)queryParams[0];
			string value2 = (string)queryParams[1];
			int num = (int)queryParams[2];
			int num2 = (int)queryParams[3];
			request.SetAttribute("initiator", value);
			request.SetAttribute("target", value2);
			request.SetAttribute("yes_votes_required", num.ToString());
			request.SetAttribute("no_votes_required", num2.ToString());
		}

		// Token: 0x04000CD8 RID: 3288
		public const string Name = "on_kick_voting_started";
	}
}
