using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoom.RoomExtensions.Vote.Queries
{
	// Token: 0x020004D9 RID: 1241
	[QueryAttributes(TagName = "on_surrender_voting_started")]
	internal class OnSurrenderVoteStartedQuery : BaseQuery
	{
		// Token: 0x06001ADE RID: 6878 RVA: 0x0006E364 File Offset: 0x0006C764
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			string value = (string)queryParams[0];
			int num = (int)queryParams[1];
			int num2 = (int)queryParams[2];
			request.SetAttribute("initiator", value);
			request.SetAttribute("yes_votes_required", num.ToString());
			request.SetAttribute("no_votes_required", num2.ToString());
		}

		// Token: 0x04000CD6 RID: 3286
		public const string Name = "on_surrender_voting_started";
	}
}
