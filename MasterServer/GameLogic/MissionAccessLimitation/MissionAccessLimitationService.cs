using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Users;

namespace MasterServer.GameLogic.MissionAccessLimitation
{
	// Token: 0x0200039D RID: 925
	[Service]
	[Singleton]
	internal class MissionAccessLimitationService : ServiceModule, IMissionAccessLimitationService
	{
		// Token: 0x06001484 RID: 5252 RVA: 0x000533BE File Offset: 0x000517BE
		public MissionAccessLimitationService(IItemStats itemStats, IProfileItems profileItems, IItemsPurchase itemsPurchase, IGameRoomManager gameRoomManager, IUserRepository userRepository)
		{
			this.m_itemStats = itemStats;
			this.m_profileItems = profileItems;
			this.m_itemsPurchase = itemsPurchase;
			this.m_gameRoomManager = gameRoomManager;
			this.m_userRepository = userRepository;
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x000533EC File Offset: 0x000517EC
		public override void Start()
		{
			base.Start();
			ConfigSection section = Resources.ModuleSettings.GetSection("PveMissionAccessLimitation");
			section.Get("Enabled", out this.m_enabled);
			section.OnConfigChanged += this.OnConfigChanged;
			this.m_itemsPurchase.OnItemPurchased += this.OnItemPurchased;
		}

		// Token: 0x06001486 RID: 5254 RVA: 0x0005344C File Offset: 0x0005184C
		public override void Stop()
		{
			base.Stop();
			ConfigSection section = Resources.ModuleSettings.GetSection("PveMissionAccessLimitation");
			section.OnConfigChanged -= this.OnConfigChanged;
			this.m_itemsPurchase.OnItemPurchased -= this.OnItemPurchased;
		}

		// Token: 0x06001487 RID: 5255 RVA: 0x00053498 File Offset: 0x00051898
		public bool CanJoinMission(ulong profileId, string missionType)
		{
			if (this.m_enabled == 0)
			{
				return true;
			}
			SItem accessItem = this.m_itemStats.GetAccessItemByMisisonType(missionType);
			if (accessItem == null)
			{
				return true;
			}
			Dictionary<ulong, SProfileItem> profileItems = this.m_profileItems.GetProfileItems(profileId, EquipOptions.ActiveOnly | EquipOptions.FilterByTags, (SProfileItem x) => x.GameItem.IsAccessItem && x.GameItem.Name.Equals(accessItem.Name));
			return profileItems.Any((KeyValuePair<ulong, SProfileItem> profileItem) => profileItem.Value.Quantity > 0UL);
		}

		// Token: 0x06001488 RID: 5256 RVA: 0x00053514 File Offset: 0x00051914
		public void OnConfigChanged(ConfigEventArgs e)
		{
			if (string.Equals(e.Name, "Enabled", StringComparison.CurrentCultureIgnoreCase))
			{
				Interlocked.Exchange(ref this.m_enabled, e.iValue);
			}
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x00053540 File Offset: 0x00051940
		public void OnItemPurchased(SItem item, ulong profileId)
		{
			if (Resources.Channel != Resources.ChannelType.PVE)
			{
				return;
			}
			if (item.IsAccessItem)
			{
				IGameRoom room = this.m_gameRoomManager.GetRoomByPlayer(profileId);
				if (room != null)
				{
					room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						RoomPlayer player = room.GetPlayer(profileId);
						if (player.RoomStatus != RoomPlayer.EStatus.CantBeReady)
						{
							return;
						}
						MissionContext mission = room.GetState<MissionState>(AccessMode.ReadOnly).Mission;
						SItem accessItemByMisisonType = this.m_itemStats.GetAccessItemByMisisonType(mission.missionType.Name);
						if (accessItemByMisisonType == null || item.Name != accessItemByMisisonType.Name)
						{
							return;
						}
						UserInfo.User user = this.m_userRepository.GetUser(profileId);
						if (!user.ProfileProgression.IsMissionTypeUnlocked(mission.missionType.Name))
						{
							return;
						}
						player.RoomStatus = RoomPlayer.EStatus.NotReady;
					});
				}
			}
		}

		// Token: 0x040009A1 RID: 2465
		private readonly IItemStats m_itemStats;

		// Token: 0x040009A2 RID: 2466
		private readonly IProfileItems m_profileItems;

		// Token: 0x040009A3 RID: 2467
		private readonly IItemsPurchase m_itemsPurchase;

		// Token: 0x040009A4 RID: 2468
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x040009A5 RID: 2469
		private readonly IUserRepository m_userRepository;

		// Token: 0x040009A6 RID: 2470
		private int m_enabled;
	}
}
