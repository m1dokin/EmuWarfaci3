using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoom.RoomExtensions.Vote.Data;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.RoomExtensions.Vote.Queries
{
	// Token: 0x020004DF RID: 1247
	internal abstract class StartVoteQuery<T> : BaseQuery where T : VoteExtension
	{
		// Token: 0x06001AE7 RID: 6887 RVA: 0x0006E51F File Offset: 0x0006C91F
		protected StartVoteQuery(IGameRoomManager gameRoomManager, VoteType voteType)
		{
			this.m_gameRoomManager = gameRoomManager;
			this.m_voteType = voteType;
		}

		// Token: 0x06001AE8 RID: 6888 RVA: 0x0006E538 File Offset: 0x0006C938
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			StartIngameVoteParams requestParams = new StartIngameVoteParams(this.m_gameRoomManager, request, this.m_voteType);
			VoteError errorCode = VoteError.None;
			requestParams.InitiatorRoom.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				T extension = requestParams.InitiatorRoom.GetExtension<T>();
				errorCode = extension.StartVote(requestParams.Initiator, requestParams.Target, requestParams.VotersList, requestParams.InitiatorTeamId);
			});
			return (int)((errorCode == VoteError.None) ? VoteError.None : errorCode);
		}

		// Token: 0x04000CDA RID: 3290
		protected readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000CDB RID: 3291
		private readonly VoteType m_voteType;
	}
}
