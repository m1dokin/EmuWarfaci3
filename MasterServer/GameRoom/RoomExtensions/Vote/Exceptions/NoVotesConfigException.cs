using System;

namespace MasterServer.GameRoom.RoomExtensions.Vote.Exceptions
{
	// Token: 0x020004D2 RID: 1234
	internal class NoVotesConfigException : VoteConfigException
	{
		// Token: 0x06001ABA RID: 6842 RVA: 0x0006D85E File Offset: 0x0006BC5E
		public NoVotesConfigException(string votesSection) : base(string.Format("There is no '{0}' section in config", votesSection))
		{
		}
	}
}
