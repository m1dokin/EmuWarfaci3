using System;
using HK2Net;
using MasterServer.Core.Configuration;
using MasterServer.GameRoom.RoomExtensions.Vote.Exceptions;

namespace MasterServer.GameRoom.RoomExtensions.Vote
{
	// Token: 0x020004E5 RID: 1253
	[Service]
	[Singleton]
	internal class VoteConfigParser : IVoteConfigParser
	{
		// Token: 0x06001AFE RID: 6910 RVA: 0x0006E804 File Offset: 0x0006CC04
		public VoteConfig Parse(ConfigSection voteConfig)
		{
			float num;
			TimeSpan timeout;
			TimeSpan cooldown;
			TimeSpan canBeStartedAfter;
			try
			{
				if (!voteConfig.TryGet("success_threshold", out num, 0f) || num <= 0f || num > 1f)
				{
					throw new InvalidVoteConfigParameterException("success_threshold");
				}
				if (!voteConfig.TryGet("timeout_sec", out timeout, default(TimeSpan)))
				{
					throw new InvalidVoteConfigParameterException("timeout_sec");
				}
				if (!voteConfig.TryGet("cooldown_sec", out cooldown, default(TimeSpan)))
				{
					throw new InvalidVoteConfigParameterException("cooldown_sec");
				}
				if (!voteConfig.TryGet("can_be_started_after_sec", out canBeStartedAfter, default(TimeSpan)))
				{
					throw new InvalidVoteConfigParameterException("can_be_started_after_sec");
				}
			}
			catch (FormatException ex)
			{
				throw new InvalidVoteConfigParameterException(ex);
			}
			return new VoteConfig
			{
				Threshold = num,
				Timeout = timeout,
				Cooldown = cooldown,
				CanBeStartedAfter = canBeStartedAfter
			};
		}

		// Token: 0x04000CE1 RID: 3297
		public const string ThresholdAttribute = "success_threshold";

		// Token: 0x04000CE2 RID: 3298
		public const string TimeoutAttribute = "timeout_sec";

		// Token: 0x04000CE3 RID: 3299
		public const string CooldownAttribute = "cooldown_sec";

		// Token: 0x04000CE4 RID: 3300
		public const string CanBeStartedAfterAttribute = "can_be_started_after_sec";
	}
}
