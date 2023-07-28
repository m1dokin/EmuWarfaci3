using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.GameLogic.InGameEventSystem;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.SpecialProfileRewards;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002CF RID: 719
	[CustomRule("ingame_reward")]
	internal class InGameRewardRule : CustomRule
	{
		// Token: 0x06000F54 RID: 3924 RVA: 0x0003D8D4 File Offset: 0x0003BCD4
		public InGameRewardRule(XmlElement config, IUserRepository userRepository, IInGameEventsService inGameEventService, IGameRoomManager gameRoomManager, ISpecialProfileRewardService specialProfileRewardService, ILogService logService, ITagService tagService, INotificationService notificationService) : base(config, userRepository, logService, notificationService, tagService, InGameRewardRule.RULE_CFG_ATTRS)
		{
			this.m_inGameEventService = inGameEventService;
			this.m_gameRoomManager = gameRoomManager;
			this.m_specialProfileRewardService = specialProfileRewardService;
			this.m_missionFilter = config.GetAttribute("mission_filter").Trim().ToLower();
			this.m_rewardLimit = int.Parse(config.GetAttribute("reward_limit"));
		}

		// Token: 0x06000F55 RID: 3925 RVA: 0x0003D954 File Offset: 0x0003BD54
		protected override ulong GetRuleID(XmlElement config)
		{
			ulong num = (ulong)((long)"ingame_reward".GetHashCode());
			string[] array = new string[]
			{
				"mission_filter",
				"reward_limit"
			};
			for (int num2 = 0; num2 != array.Length; num2++)
			{
				num ^= (ulong)((ulong)((long)config.GetAttribute(array[num2]).GetHashCode()) << 32 * (num2 % 2));
			}
			return num;
		}

		// Token: 0x06000F56 RID: 3926 RVA: 0x0003D9B5 File Offset: 0x0003BDB5
		public override bool IsActive()
		{
			return base.Enabled;
		}

		// Token: 0x06000F57 RID: 3927 RVA: 0x0003D9C0 File Offset: 0x0003BDC0
		public override void Activate()
		{
			this.m_inGameEventService.OnInGameReward += this.OnInGameReward;
			this.m_gameRoomManager.SessionStarted += this.OnSessionStarted;
			this.m_gameRoomManager.SessionEnded += this.OnSessionEnded;
		}

		// Token: 0x06000F58 RID: 3928 RVA: 0x0003DA14 File Offset: 0x0003BE14
		public override void Dispose()
		{
			this.m_inGameEventService.OnInGameReward -= this.OnInGameReward;
			this.m_gameRoomManager.SessionStarted -= this.OnSessionStarted;
			this.m_gameRoomManager.SessionEnded -= this.OnSessionEnded;
		}

		// Token: 0x06000F59 RID: 3929 RVA: 0x0003DA68 File Offset: 0x0003BE68
		private void OnInGameReward(string sessionId, string missionType, SProfileInfo profile, string rewardSet)
		{
			if (!string.Equals(this.m_missionFilter, missionType, StringComparison.InvariantCultureIgnoreCase))
			{
				return;
			}
			object @lock = this.m_lock;
			InGameRewardRule.InGameRewardSessionData inGameRewardSessionData;
			lock (@lock)
			{
				if (!this.m_inGameRewardSessionDataBySessionId.TryGetValue(sessionId, out inGameRewardSessionData))
				{
					Log.Warning<string>("Got in game reward after session '{0}' is ended", sessionId);
					return;
				}
				int num;
				if (!inGameRewardSessionData.RewardCountByProfileId.TryGetValue(profile.Id, out num))
				{
					num = 0;
				}
				if (num >= this.m_rewardLimit)
				{
					Log.Warning<SProfileInfo, int>("In game reward limit '{0}' for profile {1} is reached.", profile, this.m_rewardLimit);
					throw new InGameRewardRewardLimitReachedException(profile.Id, this.m_rewardLimit);
				}
				inGameRewardSessionData.RewardCountByProfileId[profile.Id] = num + 1;
			}
			try
			{
				using (ILogGroup logGroup = this.LogService.CreateGroup())
				{
					base.ReportCustomRuleTriggered(profile.UserID, profile.Id, logGroup);
					List<SNotification> notifications = this.m_specialProfileRewardService.ProcessEvent(rewardSet, profile.Id, logGroup);
					base.SendNotifications(profile.Id, notifications);
				}
			}
			catch
			{
				object lock2 = this.m_lock;
				lock (lock2)
				{
					Dictionary<ulong, int> rewardCountByProfileId;
					ulong id;
					(rewardCountByProfileId = inGameRewardSessionData.RewardCountByProfileId)[id = profile.Id] = rewardCountByProfileId[id] - 1;
				}
				throw;
			}
		}

		// Token: 0x06000F5A RID: 3930 RVA: 0x0003DC10 File Offset: 0x0003C010
		private void OnSessionStarted(IGameRoom room, string sessionId)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_inGameRewardSessionDataBySessionId.Add(sessionId, new InGameRewardRule.InGameRewardSessionData());
			}
		}

		// Token: 0x06000F5B RID: 3931 RVA: 0x0003DC60 File Offset: 0x0003C060
		private void OnSessionEnded(IGameRoom room, string sessionId, bool abnormal)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_inGameRewardSessionDataBySessionId.Remove(sessionId);
			}
		}

		// Token: 0x06000F5C RID: 3932 RVA: 0x0003DCAC File Offset: 0x0003C0AC
		public override string ToString()
		{
			return string.Format("{0} id={1} mission_filter='{2}' message={3} reward_limit='{4}'", new object[]
			{
				"ingame_reward",
				base.RuleID,
				this.m_missionFilter,
				base.Message,
				this.m_rewardLimit
			});
		}

		// Token: 0x0400071C RID: 1820
		private const string RULE_NAME = "ingame_reward";

		// Token: 0x0400071D RID: 1821
		private static readonly string[] RULE_CFG_ATTRS = new string[]
		{
			"enabled",
			"mission_filter",
			"reward_limit",
			"message",
			"use_notification"
		};

		// Token: 0x0400071E RID: 1822
		private readonly IInGameEventsService m_inGameEventService;

		// Token: 0x0400071F RID: 1823
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000720 RID: 1824
		private readonly ISpecialProfileRewardService m_specialProfileRewardService;

		// Token: 0x04000721 RID: 1825
		private readonly string m_missionFilter;

		// Token: 0x04000722 RID: 1826
		private readonly int m_rewardLimit;

		// Token: 0x04000723 RID: 1827
		private readonly object m_lock = new object();

		// Token: 0x04000724 RID: 1828
		private readonly Dictionary<string, InGameRewardRule.InGameRewardSessionData> m_inGameRewardSessionDataBySessionId = new Dictionary<string, InGameRewardRule.InGameRewardSessionData>();

		// Token: 0x020002D0 RID: 720
		private class InGameRewardSessionData
		{
			// Token: 0x04000725 RID: 1829
			public readonly Dictionary<ulong, int> RewardCountByProfileId = new Dictionary<ulong, int>();
		}
	}
}
