using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Core.Services;
using MasterServer.GameRoomSystem;
using Util.Common;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005A8 RID: 1448
	[Service]
	[Singleton]
	internal class RewardMultiplierService : ServiceModule, IRewardMultiplierService
	{
		// Token: 0x06001F13 RID: 7955 RVA: 0x0007E503 File Offset: 0x0007C903
		public RewardMultiplierService(IGameRoomManager gameRoomManager, ISessionStorage sessionStorage)
		{
			this.m_gameRoomManager = gameRoomManager;
			this.m_sessionStorage = sessionStorage;
		}

		// Token: 0x06001F14 RID: 7956 RVA: 0x0007E524 File Offset: 0x0007C924
		public override void Start()
		{
			this.m_gameRoomManager.SessionStarted += this.OnSessionStarted;
		}

		// Token: 0x06001F15 RID: 7957 RVA: 0x0007E53D File Offset: 0x0007C93D
		public override void Stop()
		{
			this.m_gameRoomManager.SessionStarted -= this.OnSessionStarted;
		}

		// Token: 0x06001F16 RID: 7958 RVA: 0x0007E556 File Offset: 0x0007C956
		private void OnSessionStarted(IGameRoom room, string sessionID)
		{
			this.m_sessionStorage.AddData(sessionID, ESessionData.RewardMultiplier, new SessionRewardMultiplier());
		}

		// Token: 0x06001F17 RID: 7959 RVA: 0x0007E56C File Offset: 0x0007C96C
		public Task<SRewardMultiplier> GetResultMultiplier(ulong profileID)
		{
			List<Task<SRewardMultiplier>> list = new List<Task<SRewardMultiplier>>();
			SRewardMultiplier multiplier = SRewardMultiplier.Empty;
			object providers = this.m_providers;
			lock (providers)
			{
				if (!this.m_providers.Any<IRewardMultiplierProvider>())
				{
					return TaskHelpers.Completed<SRewardMultiplier>(multiplier);
				}
				foreach (IRewardMultiplierProvider rewardMultiplierProvider in this.m_providers)
				{
					list.Add(rewardMultiplierProvider.GetMultipliers(profileID));
				}
			}
			return Task.Factory.ContinueWhenAll<SRewardMultiplier, SRewardMultiplier>(list.ToArray(), delegate(Task<SRewardMultiplier>[] tasks)
			{
				foreach (Task<SRewardMultiplier> task in tasks)
				{
					multiplier *= task.Result;
				}
				return multiplier;
			});
		}

		// Token: 0x06001F18 RID: 7960 RVA: 0x0007E658 File Offset: 0x0007CA58
		public void RegisterRewardMultiplierProvider(IRewardMultiplierProvider provider)
		{
			object providers = this.m_providers;
			lock (providers)
			{
				this.m_providers.Add(provider);
			}
		}

		// Token: 0x06001F19 RID: 7961 RVA: 0x0007E6A4 File Offset: 0x0007CAA4
		public void UnregisterRewardMultiplierProvider(IRewardMultiplierProvider provider)
		{
			object providers = this.m_providers;
			lock (providers)
			{
				this.m_providers.Remove(provider);
			}
		}

		// Token: 0x04000F27 RID: 3879
		private readonly List<IRewardMultiplierProvider> m_providers = new List<IRewardMultiplierProvider>();

		// Token: 0x04000F28 RID: 3880
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000F29 RID: 3881
		private readonly ISessionStorage m_sessionStorage;
	}
}
