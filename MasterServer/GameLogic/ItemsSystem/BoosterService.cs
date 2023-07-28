using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000315 RID: 789
	[Service]
	[Singleton]
	internal class BoosterService : ServiceModule, IBoosterService
	{
		// Token: 0x06001209 RID: 4617 RVA: 0x000477C2 File Offset: 0x00045BC2
		public BoosterService(IGameRoomManager gameRoomManager, ISessionStorage sessionStorage, IProfileItems profileItemsService, IItemStats itemStats)
		{
			this.m_gameRoomManager = gameRoomManager;
			this.m_sessionStorage = sessionStorage;
			this.m_profileItemsService = profileItemsService;
			this.m_itemStats = itemStats;
		}

		// Token: 0x0600120A RID: 4618 RVA: 0x000477F4 File Offset: 0x00045BF4
		public override void Start()
		{
			IOnlineVariables service = ServicesManager.GetService<IOnlineVariables>();
			this.m_maxBoostEffects[BoosterType.XPBooster] = float.Parse(service.Get("max_xp_boost_effect", OnlineVariableDestination.Client));
			this.m_maxBoostEffects[BoosterType.VPBooster] = float.Parse(service.Get("max_vp_boost_effect", OnlineVariableDestination.Client));
			this.m_maxBoostEffects[BoosterType.GMBooster] = float.Parse(service.Get("max_gm_boost_effect", OnlineVariableDestination.Client));
			this.m_maxBoostEffects[BoosterType.ICBooster] = float.Parse(service.Get("max_ic_boost_effect", OnlineVariableDestination.Client));
			this.m_gameRoomManager.SessionStarted += this.OnSessionStarted;
		}

		// Token: 0x0600120B RID: 4619 RVA: 0x00047892 File Offset: 0x00045C92
		private void OnSessionStarted(IGameRoom room, string session_id)
		{
			this.m_sessionStorage.AddData(session_id, ESessionData.Boosters, new SessionBoosters());
		}

		// Token: 0x0600120C RID: 4620 RVA: 0x000478A8 File Offset: 0x00045CA8
		public Dictionary<BoosterType, float> GetBoosters(ulong profile_id)
		{
			List<BoosterDesc> profileBoosters = this.GetProfileBoosters(profile_id);
			return this.StackBoosters(profileBoosters);
		}

		// Token: 0x0600120D RID: 4621 RVA: 0x000478C4 File Offset: 0x00045CC4
		public bool HasVipItem(ulong profile_id)
		{
			IProfileItems service = ServicesManager.GetService<IProfileItems>();
			IItemStats service2 = ServicesManager.GetService<IItemStats>();
			Dictionary<ulong, SProfileItem> profileItems = service.GetProfileItems(profile_id, EquipOptions.ActiveOnly | EquipOptions.FilterByTags);
			foreach (KeyValuePair<ulong, SProfileItem> keyValuePair in profileItems)
			{
				if (service2.IsVipItem(keyValuePair.Value.ItemID) && !keyValuePair.Value.IsExpired)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600120E RID: 4622 RVA: 0x00047960 File Offset: 0x00045D60
		private Dictionary<BoosterType, float> StackBoosters(List<BoosterDesc> boosters)
		{
			Dictionary<BoosterType, float> dictionary = new Dictionary<BoosterType, float>();
			IEnumerable<BoosterType> enumerable = Enum.GetValues(typeof(BoosterType)).Cast<BoosterType>();
			foreach (BoosterType key in enumerable)
			{
				float num = 0f;
				float num2 = 0f;
				foreach (BoosterDesc boosterDesc in boosters)
				{
					float num3 = 0f;
					if (boosterDesc.Params.TryGetValue(key, out num3))
					{
						if (boosterDesc.StackOption == BoosterStackOption.SumStackOption)
						{
							num += num3;
						}
						else if (boosterDesc.StackOption == BoosterStackOption.MaxStackOption)
						{
							num2 = Math.Max(num2, num3);
						}
					}
				}
				float val = this.m_maxBoostEffects[key];
				dictionary.Add(key, Math.Min(val, num + num2));
			}
			return dictionary;
		}

		// Token: 0x0600120F RID: 4623 RVA: 0x00047A8C File Offset: 0x00045E8C
		private List<BoosterDesc> GetProfileBoosters(ulong profile_id)
		{
			Dictionary<ulong, SProfileItem> profileItems = this.m_profileItemsService.GetProfileItems(profile_id, EquipOptions.ActiveOnly | EquipOptions.FilterByTags);
			List<BoosterDesc> list = new List<BoosterDesc>();
			foreach (SProfileItem sprofileItem in profileItems.Values)
			{
				if (!sprofileItem.IsExpired && this.m_itemStats.IsBoosterItem(sprofileItem.ItemID))
				{
					BoosterDesc boosterDesc = this.m_itemStats.GetBoosterDesc(sprofileItem.ItemID);
					if (boosterDesc == null)
					{
						Log.Warning<ulong>("Can't find booster for item {0}", sprofileItem.ItemID);
					}
					else
					{
						list.Add(boosterDesc);
					}
				}
			}
			return list;
		}

		// Token: 0x04000838 RID: 2104
		private Dictionary<BoosterType, float> m_maxBoostEffects = new Dictionary<BoosterType, float>();

		// Token: 0x04000839 RID: 2105
		private IGameRoomManager m_gameRoomManager;

		// Token: 0x0400083A RID: 2106
		private ISessionStorage m_sessionStorage;

		// Token: 0x0400083B RID: 2107
		private readonly IProfileItems m_profileItemsService;

		// Token: 0x0400083C RID: 2108
		private readonly IItemStats m_itemStats;
	}
}
