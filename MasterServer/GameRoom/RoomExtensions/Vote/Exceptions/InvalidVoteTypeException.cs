using System;

namespace MasterServer.GameRoom.RoomExtensions.Vote.Exceptions
{
	// Token: 0x020004D1 RID: 1233
	internal class InvalidVoteTypeException : VoteConfigException
	{
		// Token: 0x06001AB9 RID: 6841 RVA: 0x0006D84B File Offset: 0x0006BC4B
		public InvalidVoteTypeException(string voteType) : base(string.Format("There is no config for vote '{0}'", voteType))
		{
		}
	}
}
