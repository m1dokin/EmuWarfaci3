using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem.WinModels
{
	// Token: 0x02000333 RID: 819
	[Service]
	[Singleton]
	internal class SlotMachineWinModel : WinModelBase
	{
		// Token: 0x06001273 RID: 4723 RVA: 0x0004A270 File Offset: 0x00048670
		public SlotMachineWinModel(IGameRoomManager gameRoomManager, IUserRepository userRepository, ILogService logService) : base(userRepository, logService)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x06001274 RID: 4724 RVA: 0x0004A297 File Offset: 0x00048697
		public override void Init()
		{
			this.m_gameRoomManager.SessionStarted += this.OnSessionStarted;
			this.m_userRepository.UserLoggedOut += this.OnUserLoggedOut;
		}

		// Token: 0x06001275 RID: 4725 RVA: 0x0004A2C8 File Offset: 0x000486C8
		private void OnUserLoggedOut(UserInfo.User user, ELogoutType _)
		{
			object tokenMapLocker = this.m_tokenMapLocker;
			lock (tokenMapLocker)
			{
				this.ResetPrizeTokensCount(user.UserID, user.ProfileID);
			}
		}

		// Token: 0x06001276 RID: 4726 RVA: 0x0004A318 File Offset: 0x00048718
		private void OnSessionStarted(IGameRoom room, string sessionID)
		{
			List<RoomPlayer> list = new List<RoomPlayer>();
			list.AddRange(room.Players);
			object tokenMapLocker = this.m_tokenMapLocker;
			lock (tokenMapLocker)
			{
				list.SafeForEach(delegate(RoomPlayer rp)
				{
					this.ResetPrizeTokensCount(rp.UserID, rp.ProfileID);
				});
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06001277 RID: 4727 RVA: 0x0004A37C File Offset: 0x0004877C
		public override TopPrizeWinModel WinModel
		{
			get
			{
				return TopPrizeWinModel.SlotMachine;
			}
		}

		// Token: 0x06001278 RID: 4728 RVA: 0x0004A380 File Offset: 0x00048780
		public override int AddPrizeToken(ulong userId, string tokenName)
		{
			object tokenMapLocker = this.m_tokenMapLocker;
			int result;
			lock (tokenMapLocker)
			{
				Dictionary<string, ulong> dictionary;
				if (!this.m_tokenMap.TryGetValue(userId, out dictionary))
				{
					dictionary = (this.m_tokenMap[userId] = new Dictionary<string, ulong>());
				}
				ulong num;
				if (!dictionary.TryGetValue(tokenName, out num))
				{
					num = 0UL;
				}
				num = (dictionary[tokenName] = num + 1UL);
				result = (int)num;
			}
			return result;
		}

		// Token: 0x06001279 RID: 4729 RVA: 0x0004A408 File Offset: 0x00048808
		public override void ResetPrizeTokensCount(ulong userId, ulong profileId, string tokenName)
		{
			object tokenMapLocker = this.m_tokenMapLocker;
			lock (tokenMapLocker)
			{
				Dictionary<string, ulong> dictionary;
				if (this.m_tokenMap.TryGetValue(userId, out dictionary))
				{
					ulong tokenCount;
					if (dictionary.TryGetValue(tokenName, out tokenCount))
					{
						base.LogTopPrizeTokensResetForUser(userId, tokenName, tokenCount);
						dictionary.Remove(tokenName);
					}
				}
			}
		}

		// Token: 0x0600127A RID: 4730 RVA: 0x0004A484 File Offset: 0x00048884
		private void ResetPrizeTokensCount(ulong userId, ulong profileId)
		{
			Dictionary<string, ulong> arr;
			if (!this.m_tokenMap.TryGetValue(userId, out arr))
			{
				return;
			}
			arr.SafeForEach(delegate(KeyValuePair<string, ulong> t)
			{
				this.LogTopPrizeTokensReset(profileId, t.Key, t.Value);
			});
			this.m_tokenMap.Remove(userId);
		}

		// Token: 0x0600127B RID: 4731 RVA: 0x0004A4D8 File Offset: 0x000488D8
		public override Dictionary<string, ulong> GetCollectedPrizeTokensCount(ulong userId)
		{
			object tokenMapLocker = this.m_tokenMapLocker;
			Dictionary<string, ulong> result;
			lock (tokenMapLocker)
			{
				Dictionary<string, ulong> dictionary;
				result = ((!this.m_tokenMap.TryGetValue(userId, out dictionary)) ? new Dictionary<string, ulong>(0) : new Dictionary<string, ulong>(dictionary));
			}
			return result;
		}

		// Token: 0x0600127C RID: 4732 RVA: 0x0004A53C File Offset: 0x0004893C
		public override void Dispose()
		{
			this.m_gameRoomManager.SessionStarted -= this.OnSessionStarted;
			this.m_userRepository.UserLoggedOut -= this.OnUserLoggedOut;
		}

		// Token: 0x04000887 RID: 2183
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000888 RID: 2184
		private readonly Dictionary<ulong, Dictionary<string, ulong>> m_tokenMap = new Dictionary<ulong, Dictionary<string, ulong>>();

		// Token: 0x04000889 RID: 2185
		private readonly object m_tokenMapLocker = new object();
	}
}
