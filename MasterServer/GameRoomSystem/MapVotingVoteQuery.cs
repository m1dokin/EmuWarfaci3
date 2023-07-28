using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200049C RID: 1180
	[QueryAttributes(TagName = "map_voting_vote", QoSClass = "map_voting_vote")]
	internal class MapVotingVoteQuery : BaseQuery
	{
		// Token: 0x0600192C RID: 6444 RVA: 0x00066AFE File Offset: 0x00064EFE
		public MapVotingVoteQuery(IGameRoomManager gameRoomManager)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x0600192D RID: 6445 RVA: 0x00066B10 File Offset: 0x00064F10
		public override int QueryGetResponse(string from, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "map_voting_vote"))
			{
				ulong profileId;
				if (!base.GetClientProfileId(from, out profileId))
				{
					result = -3;
				}
				else
				{
					string map = request.GetAttribute("mission_uid");
					IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(profileId);
					if (roomByPlayer != null)
					{
						roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
						{
							MapVotingExtension extension = r.GetExtension<MapVotingExtension>();
							extension.CountVote(profileId, map);
						});
					}
					else
					{
						Log.Error<ulong>("Unable to find room when vote from profileId: {0} has arrived", profileId);
					}
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x04000C08 RID: 3080
		public const string QueryName = "map_voting_vote";

		// Token: 0x04000C09 RID: 3081
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
