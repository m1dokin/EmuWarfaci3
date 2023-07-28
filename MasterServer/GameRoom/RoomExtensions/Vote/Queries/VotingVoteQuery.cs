using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.RoomExtensions.Vote.Queries
{
	// Token: 0x020004E1 RID: 1249
	internal abstract class VotingVoteQuery : BaseQuery
	{
		// Token: 0x06001AEB RID: 6891 RVA: 0x0006E250 File Offset: 0x0006C650
		protected VotingVoteQuery(IGameRoomManager gameRoomManager)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x06001AEC RID: 6892 RVA: 0x0006E260 File Offset: 0x0006C660
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			ulong profileId;
			if (!base.GetClientProfileId(fromJid, out profileId))
			{
				return -3;
			}
			bool answer = int.Parse(request.GetAttribute("answer")) == 1;
			int teamId = int.Parse(request.GetAttribute("team_id"));
			IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(profileId);
			if (roomByPlayer == null)
			{
				return -1;
			}
			roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				VoteExtension voteExtension = this.GetVoteExtension(r);
				GameVote vote = voteExtension.GetVote(teamId);
				if (vote == null)
				{
					Log.Warning<ulong>("No vote in progress for room {0}", r.ID);
				}
				else
				{
					vote.Vote(profileId, answer);
				}
			});
			return 0;
		}

		// Token: 0x06001AED RID: 6893
		protected abstract VoteExtension GetVoteExtension(IGameRoom room);

		// Token: 0x04000CDC RID: 3292
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
