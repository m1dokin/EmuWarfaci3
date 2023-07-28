using System;
using System.Collections.Generic;
using System.Threading;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.RewardSystem.RankConfig;
using MasterServer.Users;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005B0 RID: 1456
	[Service]
	[Singleton]
	internal class RankSystem : ServiceModule, IRankSystem, IRewardProcessor
	{
		// Token: 0x06001F38 RID: 7992 RVA: 0x0007EA26 File Offset: 0x0007CE26
		public RankSystem(IDALService dalService, IUserRepository userRepository, ISessionInfoService sessionInfoService, IConfigProvider<ChannelRankConfig> rankConfigProvider)
		{
			this.m_dalService = dalService;
			this.m_userRepository = userRepository;
			this.m_sessionInfoService = sessionInfoService;
			this.m_rankConfigProvider = rankConfigProvider;
		}

		// Token: 0x17000338 RID: 824
		// (get) Token: 0x06001F39 RID: 7993 RVA: 0x0007EA4C File Offset: 0x0007CE4C
		public uint ChannelMinRank
		{
			get
			{
				ChannelRankRestriction rankRestriction = this.m_rankConfig.RankRestriction;
				return rankRestriction.ChannelMinRank;
			}
		}

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06001F3A RID: 7994 RVA: 0x0007EA6C File Offset: 0x0007CE6C
		public uint ChannelMaxRank
		{
			get
			{
				ChannelRankRestriction rankRestriction = this.m_rankConfig.RankRestriction;
				return rankRestriction.ChannelMaxRank;
			}
		}

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06001F3B RID: 7995 RVA: 0x0007EA8B File Offset: 0x0007CE8B
		public uint GlobalMaxRank
		{
			get
			{
				return this.m_rankConfig.GlobalMaxRank;
			}
		}

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x06001F3C RID: 7996 RVA: 0x0007EA98 File Offset: 0x0007CE98
		public bool RankClusteringEnabled
		{
			get
			{
				NewbieProtectionRankClustering newbieProtectionRankClustering = this.m_rankConfig.NewbieProtectionRankClustering;
				return newbieProtectionRankClustering.RankClusteringEnabled && Resources.Channel == Resources.ChannelType.PVE;
			}
		}

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06001F3D RID: 7997 RVA: 0x0007EAC8 File Offset: 0x0007CEC8
		public int NewbieRank
		{
			get
			{
				NewbieProtectionRankClustering newbieProtectionRankClustering = this.m_rankConfig.NewbieProtectionRankClustering;
				return newbieProtectionRankClustering.NewbieRank;
			}
		}

		// Token: 0x14000077 RID: 119
		// (add) Token: 0x06001F3E RID: 7998 RVA: 0x0007EAE8 File Offset: 0x0007CEE8
		// (remove) Token: 0x06001F3F RID: 7999 RVA: 0x0007EB20 File Offset: 0x0007CF20
		public event OnProfileRankChangedDeleg OnProfileRankChanged;

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x06001F40 RID: 8000 RVA: 0x0007EB58 File Offset: 0x0007CF58
		public Resources.ChannelRankGroup ChannelRankGroup
		{
			get
			{
				int usersOnline = this.m_usersOnline;
				if (!this.RankClusteringEnabled || usersOnline == 0)
				{
					return Resources.ChannelRankGroup.All;
				}
				return ((float)this.m_skilledOnline / (float)usersOnline <= 0.5f) ? Resources.ChannelRankGroup.Newbie : Resources.ChannelRankGroup.Skilled;
			}
		}

		// Token: 0x06001F41 RID: 8001 RVA: 0x0007EB9C File Offset: 0x0007CF9C
		public override void Init()
		{
			this.m_rankConfig = this.m_rankConfigProvider.Get();
			this.m_rankConfigProvider.Changed += this.OnConfigChanged;
			this.m_userRepository.UserLoggedIn += this.OnUserLoggedIn;
			this.m_userRepository.UserLoggedOut += this.OnUserLoggedOut;
			this.m_userRepository.UserInfoChanged += this.OnUserInfoChanged;
		}

		// Token: 0x06001F42 RID: 8002 RVA: 0x0007EC18 File Offset: 0x0007D018
		public override void Stop()
		{
			this.m_rankConfigProvider.Changed -= this.OnConfigChanged;
			this.m_userRepository.UserLoggedIn -= this.OnUserLoggedIn;
			this.m_userRepository.UserLoggedOut -= this.OnUserLoggedOut;
			this.m_userRepository.UserInfoChanged -= this.OnUserInfoChanged;
		}

		// Token: 0x06001F43 RID: 8003 RVA: 0x0007EC81 File Offset: 0x0007D081
		public SRankInfo CalculateRankInfo(ulong points)
		{
			return RankCurveUtils.CalculateRankInfo(points, this.m_rankConfig.ExpCurve);
		}

		// Token: 0x06001F44 RID: 8004 RVA: 0x0007EC94 File Offset: 0x0007D094
		public ulong CalculateExperience(SRankInfo rankInfo)
		{
			return RankCurveUtils.CalculatePoints(rankInfo, this.m_rankConfig.ExpCurve);
		}

		// Token: 0x06001F45 RID: 8005 RVA: 0x0007ECA7 File Offset: 0x0007D0A7
		public ulong GetExperience(int rankId)
		{
			return RankCurveUtils.GetPoints(rankId, this.m_rankConfig.ExpCurve);
		}

		// Token: 0x06001F46 RID: 8006 RVA: 0x0007ECBC File Offset: 0x0007D0BC
		public void ValidateExperience(ProfileProxy profile)
		{
			List<ulong> expCurve = this.m_rankConfig.ExpCurve;
			SRankInfo rankInfo = profile.ProfileInfo.RankInfo;
			if (rankInfo.RankStart == 0UL && rankInfo.NextRankStart == 0UL)
			{
				SRankInfo rank_info = this.CalculateRankInfo(rankInfo.Points);
				profile.SetProfileRankInfo(rankInfo.Points, rank_info);
			}
			else if (rankInfo.RankId >= expCurve.Count)
			{
				SRankInfo rank_info2 = this.CalculateRankInfo(expCurve[expCurve.Count - 1]);
				profile.SetProfileRankInfo(rankInfo.Points, rank_info2);
			}
			else if (rankInfo.RankStart != expCurve[rankInfo.RankId - 1] || rankInfo.NextRankStart != expCurve[rankInfo.RankId])
			{
				SRankInfo rank_info3 = default(SRankInfo);
				rank_info3.RankId = rankInfo.RankId;
				rank_info3.Points = this.CalculateExperience(rankInfo);
				rank_info3.RankStart = expCurve[rank_info3.RankId - 1];
				rank_info3.NextRankStart = expCurve[rank_info3.RankId];
				profile.SetProfileRankInfo(rankInfo.Points, rank_info3);
			}
		}

		// Token: 0x06001F47 RID: 8007 RVA: 0x0007EDF0 File Offset: 0x0007D1F0
		public ulong AddExperience(ulong profile_id, ulong gained_exp, LevelChangeReason reason, ILogGroup logGroup)
		{
			SRankInfo old_rank = default(SRankInfo);
			SRankInfo new_rank = default(SRankInfo);
			SProfileInfo profileInfo = default(SProfileInfo);
			ulong result_gained_exp = 0UL;
			Utils.Retry(delegate
			{
				profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profile_id);
				old_rank = profileInfo.RankInfo;
				new_rank = this.CalculateRankInfo(old_rank.Points + gained_exp);
				result_gained_exp = new_rank.Points - old_rank.Points;
				return result_gained_exp <= 0UL || this.m_dalService.ProfileSystem.SetProfileRankInfo(profile_id, old_rank.Points, new_rank);
			});
			UserInfo.User user = this.m_userRepository.GetUser(profile_id);
			if (user != null)
			{
				UserInfo.User user2 = user.CloneWithRank(new_rank);
				this.m_userRepository.SetUserInfo(user2);
				user = user2;
			}
			if (logGroup != null)
			{
				logGroup.RankUpdatedLog(profileInfo.UserID, profile_id, gained_exp, old_rank.Points, old_rank.RankId, new_rank.Points, new_rank.RankId, reason);
			}
			if (old_rank.RankId != new_rank.RankId)
			{
				if (user != null)
				{
					this.m_sessionInfoService.UpdateProfileStatus(user);
				}
				this.FireOnProfileRankChangedEvent(profileInfo, new_rank, old_rank, logGroup);
				if (logGroup != null)
				{
					logGroup.CharacterLevelUpLog(profileInfo.UserID, (user == null) ? "0.0.0.0" : user.IP, profile_id, profileInfo.Nickname, new_rank.RankId, new_rank.Points, DateTime.Now - profileInfo.LastRankedTime, (user == null) ? TimeSpan.Zero : user.Playtime, reason);
				}
			}
			return result_gained_exp;
		}

		// Token: 0x06001F48 RID: 8008 RVA: 0x0007EFBF File Offset: 0x0007D3BF
		public Resources.ChannelRankGroup ClassifyRankGroup(int rank)
		{
			if (!this.RankClusteringEnabled)
			{
				return Resources.ChannelRankGroup.All;
			}
			return (rank > this.NewbieRank) ? Resources.ChannelRankGroup.Skilled : Resources.ChannelRankGroup.Newbie;
		}

		// Token: 0x06001F49 RID: 8009 RVA: 0x0007EFE1 File Offset: 0x0007D3E1
		public bool CanJoinChannel(int rank)
		{
			return Resources.IsDevMode || ((ulong)this.ChannelMinRank <= (ulong)((long)rank) && (long)rank <= (long)((ulong)this.ChannelMaxRank));
		}

		// Token: 0x06001F4A RID: 8010 RVA: 0x0007F00E File Offset: 0x0007D40E
		public bool CanCreateProfileOnChannel()
		{
			return this.CanJoinChannel(1);
		}

		// Token: 0x06001F4B RID: 8011 RVA: 0x0007F017 File Offset: 0x0007D417
		public RewardOutputData ProcessRewardData(ulong userId, RewardProcessorState state, MissionContext missionContext, RewardOutputData aggRewardData, ILogGroup logGroup)
		{
			if (state != RewardProcessorState.Process)
			{
				return aggRewardData;
			}
			aggRewardData.gainedExp = (uint)this.AddExperience(aggRewardData.profileId, (ulong)aggRewardData.gainedExp, LevelChangeReason.NormalReward, logGroup);
			return aggRewardData;
		}

		// Token: 0x06001F4C RID: 8012 RVA: 0x0007F044 File Offset: 0x0007D444
		private void OnConfigChanged(ChannelRankConfig config)
		{
			Interlocked.Exchange<ChannelRankConfig>(ref this.m_rankConfig, config);
		}

		// Token: 0x06001F4D RID: 8013 RVA: 0x0007F054 File Offset: 0x0007D454
		private void OnUserLoggedIn(UserInfo.User user, ELoginType loginType)
		{
			int value = (this.ClassifyRankGroup(user.Rank) == Resources.ChannelRankGroup.Newbie) ? 0 : 1;
			Interlocked.Increment(ref this.m_usersOnline);
			Interlocked.Add(ref this.m_skilledOnline, value);
		}

		// Token: 0x06001F4E RID: 8014 RVA: 0x0007F094 File Offset: 0x0007D494
		private void OnUserLoggedOut(UserInfo.User user, ELogoutType logout_type)
		{
			int num = (this.ClassifyRankGroup(user.Rank) == Resources.ChannelRankGroup.Newbie) ? 0 : 1;
			Interlocked.Add(ref this.m_skilledOnline, -num);
			Interlocked.Decrement(ref this.m_usersOnline);
		}

		// Token: 0x06001F4F RID: 8015 RVA: 0x0007F0D8 File Offset: 0x0007D4D8
		private void OnUserInfoChanged(UserInfo.User old_info, UserInfo.User new_info)
		{
			int num = (this.ClassifyRankGroup(old_info.Rank) == Resources.ChannelRankGroup.Newbie) ? 0 : 1;
			int num2 = (this.ClassifyRankGroup(new_info.Rank) == Resources.ChannelRankGroup.Newbie) ? 0 : 1;
			Interlocked.Add(ref this.m_skilledOnline, num2 - num);
		}

		// Token: 0x06001F50 RID: 8016 RVA: 0x0007F128 File Offset: 0x0007D528
		private void FireOnProfileRankChangedEvent(SProfileInfo profile, SRankInfo newRank, SRankInfo oldRank, ILogGroup logGroup)
		{
			if (this.OnProfileRankChanged == null)
			{
				return;
			}
			foreach (Delegate @delegate in this.OnProfileRankChanged.GetInvocationList())
			{
				try
				{
					((OnProfileRankChangedDeleg)@delegate)(profile, newRank, oldRank, logGroup);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		// Token: 0x04000F35 RID: 3893
		private int m_usersOnline;

		// Token: 0x04000F36 RID: 3894
		private int m_skilledOnline;

		// Token: 0x04000F37 RID: 3895
		private ChannelRankConfig m_rankConfig;

		// Token: 0x04000F38 RID: 3896
		private readonly IDALService m_dalService;

		// Token: 0x04000F39 RID: 3897
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000F3A RID: 3898
		private readonly ISessionInfoService m_sessionInfoService;

		// Token: 0x04000F3B RID: 3899
		private readonly IConfigProvider<ChannelRankConfig> m_rankConfigProvider;
	}
}
