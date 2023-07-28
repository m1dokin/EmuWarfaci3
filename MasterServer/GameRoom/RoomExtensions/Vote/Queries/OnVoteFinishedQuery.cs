using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoom.RoomExtensions.Vote.Queries
{
	// Token: 0x020004DA RID: 1242
	[QueryAttributes(TagName = "on_voting_finished")]
	internal class OnVoteFinishedQuery : BaseQuery
	{
		// Token: 0x06001AE0 RID: 6880 RVA: 0x0006E3D0 File Offset: 0x0006C7D0
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			VoteResult voteResult = (VoteResult)queryParams[0];
			int num = (int)queryParams[1];
			int num2 = (int)queryParams[2];
			string name = "result";
			int num3 = (int)voteResult;
			request.SetAttribute(name, num3.ToString());
			request.SetAttribute("yes", num.ToString());
			request.SetAttribute("no", num2.ToString());
		}

		// Token: 0x04000CD7 RID: 3287
		public const string Name = "on_voting_finished";
	}
}
