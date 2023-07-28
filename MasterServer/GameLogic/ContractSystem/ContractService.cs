using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Telemetry;
using MasterServer.Users;
using NCrontab;

namespace MasterServer.GameLogic.ContractSystem
{
	// Token: 0x02000295 RID: 661
	[Service]
	[Singleton]
	internal class ContractService : ServiceModule, IContractService
	{
		// Token: 0x06000E46 RID: 3654 RVA: 0x00038F50 File Offset: 0x00037350
		public ContractService(IItemStats itemStat, IDALService dalService, IProfileItems profileItems, INotificationService notification, ICatalogService catalog, ITelemetryService telemetry, ILogService log, IGameRoomManager gameRoomManager, ISessionStorage sessionStorage, IItemsExpiration itemsExpiration, IUserRepository userRepository)
		{
			this.m_itemStats = itemStat;
			this.m_dalService = dalService;
			this.m_profileItemsService = profileItems;
			this.m_notificationService = notification;
			this.m_catalogService = catalog;
			this.m_telemetryService = telemetry;
			this.m_logService = log;
			this.m_gameRoomManager = gameRoomManager;
			this.m_sessionStorage = sessionStorage;
			this.m_itemsExpiration = itemsExpiration;
			this.m_userRepository = userRepository;
			this.UpdateCrontab("0 0 * * *");
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06000E47 RID: 3655 RVA: 0x00038FE9 File Offset: 0x000373E9
		private TimeSpan NextRotationTime
		{
			get
			{
				return this.m_crontab.GetNextOccurrence(DateTime.UtcNow) - DateTime.UtcNow;
			}
		}

		// Token: 0x06000E48 RID: 3656 RVA: 0x00039008 File Offset: 0x00037408
		private void UpdateCrontab(string cronStr)
		{
			this.m_crontab = CrontabSchedule.Parse(cronStr);
			IEnumerator<DateTime> enumerator = this.m_crontab.GetNextOccurrences(DateTime.UtcNow, DateTime.UtcNow.AddYears(1)).GetEnumerator();
			enumerator.MoveNext();
			DateTime d = enumerator.Current;
			enumerator.MoveNext();
			DateTime d2 = enumerator.Current;
			this.m_crontabPeriod = (uint)(d2 - d).TotalSeconds;
		}

		// Token: 0x06000E49 RID: 3657 RVA: 0x00039078 File Offset: 0x00037478
		public override void Start()
		{
			this.m_itemsExpiration.OnItemExpired += this.OnItemExpired;
			ConfigSection section = Resources.ModuleSettings.GetSection("Contracts");
			section.OnConfigChanged += this.OnConfigChanged;
			string cronStr = section.Get("ContractRotationTime");
			this.UpdateCrontab(cronStr);
			int seconds = int.Parse(section.Get("ContractNotificationExpirationTime"));
			this.m_notificationExpirationTime = new TimeSpan(0, 0, seconds);
			this.m_gameRoomManager.SessionStarted += this.OnSessionStarted;
		}

		// Token: 0x06000E4A RID: 3658 RVA: 0x00039107 File Offset: 0x00037507
		public ProfileContract GetProfileContract(ulong profileId)
		{
			return this.m_dalService.ContractSystem.GetContractInfo(profileId);
		}

		// Token: 0x06000E4B RID: 3659 RVA: 0x0003911C File Offset: 0x0003751C
		public ProfileContract RotateContract(ulong profileId)
		{
			ProfileContract profileContract = this.m_dalService.ContractSystem.GetContractInfo(profileId);
			SProfileItem sprofileItem = this.GetContractFromProfileItems(profileId);
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
			if (profileContract != null)
			{
				if (sprofileItem == null)
				{
					if (profileContract.ProfileItemId > 0UL)
					{
						return this.GetNextContract(profileInfo, profileContract);
					}
					if (!this.IsContractSetActive(profileContract.RotationId))
					{
						return this.GetNextContract(profileInfo, profileContract);
					}
				}
				else if (!sprofileItem.IsExpired && profileContract.ProfileItemId == 0UL && profileContract.Status == ProfileContract.ContractStatus.eCS_None)
				{
					return this.ActivateContract(profileId, sprofileItem.ProfileItemID, sprofileItem.GameItem.Name);
				}
			}
			else if (sprofileItem != null)
			{
				ContractDesc contractByName = this.m_itemStats.GetContractByName(sprofileItem.GameItem.Name);
				if (contractByName != null)
				{
					this.m_dalService.ContractSystem.AddContract(profileId, (ulong)contractByName.Id, this.NextRotationTime);
					return this.ActivateContract(profileId, sprofileItem.ProfileItemID, sprofileItem.GameItem.Name);
				}
			}
			if (profileContract == null)
			{
				profileContract = this.GetNextContract(profileInfo, null);
			}
			else if (profileContract.IsExpired)
			{
				if ((this.m_crontab.GetNextOccurrence(DateTime.UtcNow) - profileContract.RotationTimeUTC).TotalSeconds > this.m_crontabPeriod / 2U)
				{
					if (sprofileItem != null && sprofileItem.ProfileItemID != profileContract.ProfileItemId)
					{
						Log.Verbose("Deleting contract item {0} for profile {1}", new object[]
						{
							sprofileItem.GameItem.Name,
							profileId
						});
						this.DeleteContractItem(profileInfo, sprofileItem);
						sprofileItem = null;
					}
					if (sprofileItem == null)
					{
						sprofileItem = ((profileContract.ProfileItemId <= 0UL) ? null : this.m_profileItemsService.GetProfileItem(profileId, profileContract.ProfileItemId, EquipOptions.All));
					}
					if (sprofileItem != null && sprofileItem.IsExpired)
					{
						Log.Verbose("Deleting contract item {0} for profile {1}", new object[]
						{
							sprofileItem.GameItem.Name,
							profileId
						});
						this.DeleteContractItem(profileInfo, sprofileItem);
					}
					if (sprofileItem == null || sprofileItem.IsExpired)
					{
						profileContract = this.GetNextContract(profileInfo, profileContract);
					}
				}
				else
				{
					Log.Warning("[ContractService.RotateContract] MS and DB time isn't synchronized");
				}
			}
			return profileContract;
		}

		// Token: 0x06000E4C RID: 3660 RVA: 0x00039364 File Offset: 0x00037764
		public ProfileContract ActivateContract(ulong profileId, ulong profileItemId, string itemName)
		{
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
			ContractDesc contractByName = this.m_itemStats.GetContractByName(itemName);
			ProfileContract profileContract = this.m_dalService.ContractSystem.ActivateContract(profileId, profileItemId, itemName, contractByName.TotalProgress);
			this.LockContracts(profileInfo, profileContract);
			return profileContract;
		}

		// Token: 0x06000E4D RID: 3661 RVA: 0x000393B4 File Offset: 0x000377B4
		public ProfileContract SetContractProgress(ProfileContract contract, SProfileItem contractItem, uint progress, SRewardMultiplier dynamicMultiplier)
		{
			if (contract.Status != ProfileContract.ContractStatus.eCS_InProgress)
			{
				Log.Warning<ulong, uint>("Profile {0} is trying to set progress {1} for already finished contract", contract.ProfileId, progress);
				return contract;
			}
			contract = this.m_dalService.ContractSystem.SetContractProgress(contract.ProfileId, progress);
			if (contract.Status == ProfileContract.ContractStatus.eCS_Completed)
			{
				ContractDesc contractByName = this.m_itemStats.GetContractByName(contractItem.GameItem.Name);
				if (contractByName == null)
				{
					throw new ContractServiceException(string.Format("Can't find contract with id {0} and name {1}, for profile {2}", contract.ProfileItemId, contractItem.GameItem.Name, contract.ProfileId));
				}
				SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(contract.ProfileId);
				using (ILogGroup logGroup = this.m_logService.CreateGroup())
				{
					uint gameMoneyReward = contractByName.Reward.GiveReward(profileInfo.UserID, contract.ProfileId, dynamicMultiplier, logGroup);
					contract = this.m_dalService.ContractSystem.DeactivateContract(contract.ProfileId);
					this.m_notificationService.AddNotification<ContractNotification>(contract.ProfileId, ENotificationType.Contract, new ContractNotification(contractItem.GameItem.Name, true, gameMoneyReward), this.m_notificationExpirationTime, EDeliveryType.SendNowOrLater, EConfirmationType.None);
					TimeSpan completionTime = DateTime.UtcNow - contractItem.BuyTime;
					long value = (long)(completionTime.TotalMilliseconds / 100.0);
					string text = DateTime.Now.ToString("yyyy-MM-dd");
					this.m_telemetryService.AddMeasure(value, new object[]
					{
						"stat",
						"contract_average_complete_time",
						"item_type",
						contractItem.GameItem.Name,
						"date",
						text
					});
					this.m_telemetryService.AddMeasure(value, new object[]
					{
						"stat",
						"contract_min_complete_time",
						"item_type",
						contractItem.GameItem.Name,
						"date",
						text
					});
					this.m_telemetryService.AddMeasure(1L, new object[]
					{
						"stat",
						"contract_complete",
						"item_type",
						contractItem.GameItem.Name,
						"date",
						text
					});
					logGroup.ContractCompleteLog(profileInfo.UserID, contract.ProfileId, contractItem.GameItem.Name, completionTime);
				}
			}
			ProfileContract profileContract = contract;
			contract = this.RotateContract(contract.ProfileId);
			if (contract == null)
			{
				throw new NullReferenceException(string.Format("Rotate contract returned null for profile id: {0}. Previous contract name :{1}, rotation id: {2}.\n Contract dump :\n {3}", new object[]
				{
					profileContract.ProfileId,
					profileContract.ContractName,
					profileContract.RotationId,
					this.GetContractsDump()
				}));
			}
			this.NotifyContractProgress(contract);
			return contract;
		}

		// Token: 0x06000E4E RID: 3662 RVA: 0x0003969C File Offset: 0x00037A9C
		private string GetContractsDump()
		{
			StringBuilder stringBuilder = new StringBuilder();
			Dictionary<uint, List<ContractDesc>> contractsDesc = this.m_itemStats.GetContractsDesc();
			foreach (List<ContractDesc> list in contractsDesc.Values)
			{
				stringBuilder.Append("Contract set: ");
				foreach (ContractDesc contractDesc in list)
				{
					stringBuilder.AppendFormat("[Name : {0}, id : {1}, is active : {2}]", contractDesc.Name, contractDesc.Id, contractDesc.IsActive);
				}
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000E4F RID: 3663 RVA: 0x00039788 File Offset: 0x00037B88
		private void NotifyContractProgress(ProfileContract contract)
		{
			UserInfo.User user = this.m_userRepository.GetUser(contract.ProfileId);
			if (user != null)
			{
				QueryManager.RequestSt("notify_contracts", user.OnlineID, new object[]
				{
					contract
				});
			}
		}

		// Token: 0x06000E50 RID: 3664 RVA: 0x000397C8 File Offset: 0x00037BC8
		private ProfileContract GetNextContract(SProfileInfo profileInfo, ProfileContract currentContract)
		{
			Dictionary<uint, List<ContractDesc>> contractsDesc = this.m_itemStats.GetContractsDesc();
			int count = contractsDesc.Keys.Count;
			if (count == 0)
			{
				return null;
			}
			uint num = contractsDesc.Values.Max((List<ContractDesc> x) => x.Max((ContractDesc y) => y.Id)) + 1U;
			ProfileContract profileContract = null;
			uint currentRotationId;
			if (currentContract == null)
			{
				int index = this.m_contractsRotationRandom.Next(count);
				currentRotationId = contractsDesc.Keys.ElementAt(index);
			}
			else
			{
				this.LockContracts(profileInfo, currentContract);
				currentRotationId = (currentContract.RotationId + 1U) % num;
			}
			ulong? num2 = this.SkipInactiveContracts(currentRotationId, num, contractsDesc);
			if (num2 != null)
			{
				profileContract = this.m_dalService.ContractSystem.AddContract(profileInfo.Id, num2.Value, this.NextRotationTime);
				List<ContractDesc> contracts = contractsDesc[profileContract.RotationId];
				this.UnlockContracts(profileInfo, contracts);
			}
			return profileContract;
		}

		// Token: 0x06000E51 RID: 3665 RVA: 0x000398B4 File Offset: 0x00037CB4
		private ulong? SkipInactiveContracts(uint currentRotationId, uint maxRotationId, Dictionary<uint, List<ContractDesc>> contracts)
		{
			ulong num = 0UL;
			List<ContractDesc> source;
			bool flag;
			if (contracts.TryGetValue(currentRotationId, out source))
			{
				flag = source.All((ContractDesc x) => x.IsActive);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			while (!flag2 && num <= (ulong)maxRotationId)
			{
				num += 1UL;
				currentRotationId = (currentRotationId + 1U) % maxRotationId;
				bool flag3;
				if (contracts.TryGetValue(currentRotationId, out source))
				{
					flag3 = source.All((ContractDesc x) => x.IsActive);
				}
				else
				{
					flag3 = false;
				}
				flag2 = flag3;
			}
			return (num <= (ulong)maxRotationId) ? new ulong?((ulong)currentRotationId) : null;
		}

		// Token: 0x06000E52 RID: 3666 RVA: 0x0003996C File Offset: 0x00037D6C
		private void LockContracts(SProfileInfo profileInfo, ProfileContract profileContract)
		{
			Dictionary<ulong, SItem> unlockedItems = this.m_profileItemsService.GetUnlockedItems(profileInfo.Id);
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				foreach (SItem sitem in from x in unlockedItems.Values
				where x.Type == "contract"
				select x)
				{
					this.m_profileItemsService.LockItem(profileInfo.Id, sitem.ID);
					ContractDesc contractByName = this.m_itemStats.GetContractByName(sitem.Name);
					if (profileContract != null && contractByName != null && contractByName.Id != profileContract.RotationId)
					{
						Log.Warning("Contract {0} are being locked but it's rotation id {1} doesnot match current rotation id {2} for {3} profileId", new object[]
						{
							sitem.Name,
							contractByName.Id,
							profileContract.RotationId,
							profileInfo.Id
						});
					}
					logGroup.ItemLockedLog(profileInfo.UserID, profileInfo.Id, sitem.ID, sitem.Name);
				}
			}
		}

		// Token: 0x06000E53 RID: 3667 RVA: 0x00039AE4 File Offset: 0x00037EE4
		private void UnlockContracts(SProfileInfo profileInfo, IEnumerable<ContractDesc> contracts)
		{
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				foreach (ContractDesc contractDesc in contracts)
				{
					this.m_profileItemsService.UnlockItem(profileInfo.Id, contractDesc.ItemId);
					logGroup.ItemUnlockedLog(profileInfo.UserID, profileInfo.Id, contractDesc.ItemId, contractDesc.Name, LogGroup.ProduceType.Contract);
				}
			}
		}

		// Token: 0x06000E54 RID: 3668 RVA: 0x00039B98 File Offset: 0x00037F98
		private void DeleteContractItem(SProfileInfo profileInfo, SProfileItem item)
		{
			this.m_catalogService.DeleteCustomerItem(profileInfo.UserID, item.CatalogID);
			this.m_profileItemsService.DeleteProfileItem(item.EquipItem.ProfileID, item.ProfileItemID);
			this.m_logService.Event.ItemDestroyLog(profileInfo.UserID, profileInfo.Id, profileInfo.Nickname, profileInfo.RankInfo.RankId, item.ItemID, item.CatalogID, item.GameItem.Type, item.GameItem.Name, 0, string.Empty);
		}

		// Token: 0x06000E55 RID: 3669 RVA: 0x00039C34 File Offset: 0x00038034
		private SProfileItem GetContractFromProfileItems(ulong profileId)
		{
			Dictionary<ulong, SProfileItem> profileItems = this.m_profileItemsService.GetProfileItems(profileId, EquipOptions.ActiveOnly);
			return profileItems.Values.FirstOrDefault((SProfileItem x) => x.GameItem.Type == "contract");
		}

		// Token: 0x06000E56 RID: 3670 RVA: 0x00039C78 File Offset: 0x00038078
		private void OnConfigChanged(ConfigEventArgs args)
		{
			string name = args.Name;
			if (name != null)
			{
				if (!(name == "contractrotationtime"))
				{
					if (name == "contractnotificationexpirationTime")
					{
						this.m_notificationExpirationTime = new TimeSpan(0, 0, args.iValue);
					}
				}
				else
				{
					this.UpdateCrontab(args.sValue);
				}
			}
		}

		// Token: 0x06000E57 RID: 3671 RVA: 0x00039CE0 File Offset: 0x000380E0
		private void OnItemExpired(ulong userId, SProfileItem item)
		{
			if (item == null)
			{
				throw new NullReferenceException("item is null");
			}
			if (item.CustomerItem == null)
			{
				throw new NullReferenceException("item.CustomerItem is null");
			}
			if (item.CustomerItem.CatalogItem.Type != "contract")
			{
				return;
			}
			if (item.EquipItem == null)
			{
				throw new NullReferenceException("item.EquipItem is null");
			}
			ProfileContract profileContract = this.m_dalService.ContractSystem.DeactivateContract(item.EquipItem.ProfileID);
			if (profileContract != null && profileContract.Status == ProfileContract.ContractStatus.eCS_Failed)
			{
				if (item.GameItem == null)
				{
					throw new NullReferenceException("item.GameItem is null");
				}
				this.m_notificationService.AddNotification<ContractNotification>(item.EquipItem.ProfileID, ENotificationType.Contract, new ContractNotification(item.GameItem.Name, false), this.m_notificationExpirationTime, EDeliveryType.SendNowOrLater, EConfirmationType.None);
				string text = item.ExpirationTime.ToString("yyyy-MM-dd");
				this.m_telemetryService.AddMeasure(1L, new object[]
				{
					"stat",
					"contract_fail",
					"item_type",
					item.GameItem.Name,
					"date",
					text
				});
				this.m_logService.Event.ContractFailLog(userId, profileContract.ProfileId, item.GameItem.Name, item.ExpirationTime - item.BuyTime);
			}
			ProfileContract profileContract2 = profileContract;
			profileContract = this.RotateContract(item.EquipItem.ProfileID);
			if (profileContract == null)
			{
				throw new NullReferenceException(string.Format("Rotate contract returned null for profile id: {0}. Previous contract name :{1}, rotation id: {2}.\n Contract dump :\n {3}", new object[]
				{
					profileContract2.ProfileId,
					profileContract2.ContractName,
					profileContract2.RotationId,
					this.GetContractsDump()
				}));
			}
			this.NotifyContractProgress(profileContract);
		}

		// Token: 0x06000E58 RID: 3672 RVA: 0x00039EB4 File Offset: 0x000382B4
		private void OnSessionStarted(IGameRoom room, string session_id)
		{
			this.m_sessionStorage.AddData(session_id, ESessionData.Contracts, new SessionContracts());
		}

		// Token: 0x06000E59 RID: 3673 RVA: 0x00039EC8 File Offset: 0x000382C8
		private bool IsContractSetActive(uint rotationId)
		{
			Dictionary<uint, List<ContractDesc>> contractsDesc = this.m_itemStats.GetContractsDesc();
			List<ContractDesc> source;
			if (contractsDesc.TryGetValue(rotationId, out source))
			{
				return source.All((ContractDesc x) => x.IsActive);
			}
			return false;
		}

		// Token: 0x0400068C RID: 1676
		private readonly IItemStats m_itemStats;

		// Token: 0x0400068D RID: 1677
		private readonly IDALService m_dalService;

		// Token: 0x0400068E RID: 1678
		private readonly IProfileItems m_profileItemsService;

		// Token: 0x0400068F RID: 1679
		private readonly INotificationService m_notificationService;

		// Token: 0x04000690 RID: 1680
		private readonly ICatalogService m_catalogService;

		// Token: 0x04000691 RID: 1681
		private readonly ITelemetryService m_telemetryService;

		// Token: 0x04000692 RID: 1682
		private readonly ILogService m_logService;

		// Token: 0x04000693 RID: 1683
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000694 RID: 1684
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x04000695 RID: 1685
		private readonly IItemsExpiration m_itemsExpiration;

		// Token: 0x04000696 RID: 1686
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000697 RID: 1687
		private TimeSpan m_notificationExpirationTime;

		// Token: 0x04000698 RID: 1688
		private readonly Random m_contractsRotationRandom = new Random(DateTime.UtcNow.Ticks.GetHashCode());

		// Token: 0x04000699 RID: 1689
		private CrontabSchedule m_crontab;

		// Token: 0x0400069A RID: 1690
		private uint m_crontabPeriod;
	}
}
