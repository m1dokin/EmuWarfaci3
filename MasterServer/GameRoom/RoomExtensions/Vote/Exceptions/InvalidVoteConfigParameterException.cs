using System;

namespace MasterServer.GameRoom.RoomExtensions.Vote.Exceptions
{
	// Token: 0x020004D0 RID: 1232
	internal class InvalidVoteConfigParameterException : VoteConfigException
	{
		// Token: 0x06001AB7 RID: 6839 RVA: 0x0006D820 File Offset: 0x0006BC20
		public InvalidVoteConfigParameterException(string parameterName) : base(string.Format("There is no parameter '{0}' in votes config", parameterName))
		{
		}

		// Token: 0x06001AB8 RID: 6840 RVA: 0x0006D833 File Offset: 0x0006BC33
		public InvalidVoteConfigParameterException(Exception ex) : base(string.Format("Error in votes config: {0}", ex.Message))
		{
		}
	}
}
