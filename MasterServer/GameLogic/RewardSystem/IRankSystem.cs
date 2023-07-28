using System;
using HK2Net;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Users;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005AF RID: 1455
	[Contract]
	internal interface IRankSystem
	{
		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06001F28 RID: 7976
		uint ChannelMinRank { get; }

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06001F29 RID: 7977
		uint ChannelMaxRank { get; }

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x06001F2A RID: 7978
		uint GlobalMaxRank { get; }

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x06001F2B RID: 7979
		bool RankClusteringEnabled { get; }

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x06001F2C RID: 7980
		int NewbieRank { get; }

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x06001F2D RID: 7981
		Resources.ChannelRankGroup ChannelRankGroup { get; }

		// Token: 0x06001F2E RID: 7982
		ulong GetExperience(int rankId);

		// Token: 0x06001F2F RID: 7983
		SRankInfo CalculateRankInfo(ulong points);

		// Token: 0x06001F30 RID: 7984
		ulong CalculateExperience(SRankInfo rankInfo);

		// Token: 0x06001F31 RID: 7985
		void ValidateExperience(ProfileProxy profile);

		// Token: 0x06001F32 RID: 7986
		ulong AddExperience(ulong profile_id, ulong gained_exp, LevelChangeReason reason, ILogGroup logGroup);

		// Token: 0x06001F33 RID: 7987
		Resources.ChannelRankGroup ClassifyRankGroup(int rank);

		// Token: 0x06001F34 RID: 7988
		bool CanJoinChannel(int rank);

		// Token: 0x06001F35 RID: 7989
		bool CanCreateProfileOnChannel();

		// Token: 0x14000076 RID: 118
		// (add) Token: 0x06001F36 RID: 7990
		// (remove) Token: 0x06001F37 RID: 7991
		event OnProfileRankChangedDeleg OnProfileRankChanged;
	}
}
