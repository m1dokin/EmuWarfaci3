using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoom.RoomExtensions.Vote.Queries
{
	// Token: 0x020004DC RID: 1244
	[QueryAttributes(TagName = "on_voting_vote")]
	internal class OnVotingVoteQuery : BaseQuery
	{
		// Token: 0x06001AE4 RID: 6884 RVA: 0x0006E4D0 File Offset: 0x0006C8D0
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			int num = (int)queryParams[0];
			int num2 = (int)queryParams[1];
			request.SetAttribute("yes", num.ToString());
			request.SetAttribute("no", num2.ToString());
		}

		// Token: 0x04000CD9 RID: 3289
		public const string Name = "on_voting_vote";
	}
}
