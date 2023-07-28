using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Jobs;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.InvitationSystem;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Platform.ProfanityCheck;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x0200027B RID: 635
	[Service]
	[Singleton]
	internal class ClanService : ServiceModule, IClanService, IRewardProcessor
	{
		// Token: 0x06000DBD RID: 3517 RVA: 0x00036A10 File Offset: 0x00034E10
		public ClanService(IDALService dalService, INotificationService notificationService, ISessionInfoService sessionInfoService, IProfanityCheckService profanityCheckService, IItemCache itemCache, IProfileItems profileItems, ICatalogService catalogService, IOnlineVariables onlineVariables, INameReservationService nameReservationService, IGameRoomManager gameRoomManager, ILogService logService, IJobSchedulerService jobSchedulerService, IUserRepository userRepository)
		{
			this.m_dalService = dalService;
			this.m_notificationService = notificationService;
			this.m_sessionInfoService = sessionInfoService;
			this.m_profanityCheckService = profanityCheckService;
			this.m_itemCache = itemCache;
			this.m_profileItems = profileItems;
			this.m_catalogService = catalogService;
			this.m_onlineVariables = onlineVariables;
			this.m_nameReservationService = nameReservationService;
			this.m_gameRoomManager = gameRoomManager;
			this.m_logService = logService;
			this.m_jobSchedulerService = jobSchedulerService;
			this.m_userRepository = userRepository;
		}

		// Token: 0x1400002F RID: 47
		// (add) Token: 0x06000DBE RID: 3518 RVA: 0x00036A88 File Offset: 0x00034E88
		// (remove) Token: 0x06000DBF RID: 3519 RVA: 0x00036AC0 File Offset: 0x00034EC0
		public event ClanCreatedDelegate ClanCreated;

		// Token: 0x14000030 RID: 48
		// (add) Token: 0x06000DC0 RID: 3520 RVA: 0x00036AF8 File Offset: 0x00034EF8
		// (remove) Token: 0x06000DC1 RID: 3521 RVA: 0x00036B30 File Offset: 0x00034F30
		public event ClanRemovedDelegate ClanRemoved;

		// Token: 0x14000031 RID: 49
		// (add) Token: 0x06000DC2 RID: 3522 RVA: 0x00036B68 File Offset: 0x00034F68
		// (remove) Token: 0x06000DC3 RID: 3523 RVA: 0x00036BA0 File Offset: 0x00034FA0
		public event ClanDescriptionUpdatedDelegate ClanDescriptionUpdated;

		// Token: 0x14000032 RID: 50
		// (add) Token: 0x06000DC4 RID: 3524 RVA: 0x00036BD8 File Offset: 0x00034FD8
		// (remove) Token: 0x06000DC5 RID: 3525 RVA: 0x00036C10 File Offset: 0x00035010
		public event ClanMemberListUpdatedDelegate ClanMemberListUpdated;

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06000DC6 RID: 3526 RVA: 0x00036C46 File Offset: 0x00035046
		public uint MaxClanSize
		{
			get
			{
				return this.m_maxClanSize;
			}
		}

		// Token: 0x06000DC7 RID: 3527 RVA: 0x00036C50 File Offset: 0x00035050
		public override void Init()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("Clans");
			section.OnConfigChanged += this.OnConfigChanged;
			this.m_notificationService.OnNotificationConfirmed += this.OnNotificationConfirmed;
			this.m_notificationService.OnNotificationExpired += this.OnNotificationExpired;
			this.m_maxClanSize = uint.Parse(section.Get("MaxClanSize"));
			this.m_clanNameMinSize = uint.Parse(section.Get("ClanNameMinSize"));
			this.m_clanNameMaxSize = uint.Parse(section.Get("ClanNameMaxSize"));
			this.m_kickedUserInviteTimeout = uint.Parse(section.Get("KickedUserInviteTimeout"));
			this.m_inviteExpiration = TimeSpan.FromSeconds((double)int.Parse(section.Get("InviteExpirationTimeout")));
		}

		// Token: 0x06000DC8 RID: 3528 RVA: 0x00036D21 File Offset: 0x00035121
		public override void Start()
		{
			base.Start();
			if (Resources.DBUpdaterPermission)
			{
				this.m_jobSchedulerService.AddJob("clan_fixup");
			}
		}

		// Token: 0x06000DC9 RID: 3529 RVA: 0x00036D43 File Offset: 0x00035143
		public override void Stop()
		{
			this.m_notificationService.OnNotificationConfirmed -= this.OnNotificationConfirmed;
			this.m_notificationService.OnNotificationExpired -= this.OnNotificationExpired;
		}

		// Token: 0x06000DCA RID: 3530 RVA: 0x00036D74 File Offset: 0x00035174
		private void OnClanMemberUpdated(ulong clan_id, ClanMember member_info, EMembersListUpdate update_type)
		{
			List<SClanMemberUpdate> updates = new List<SClanMemberUpdate>
			{
				new SClanMemberUpdate(member_info, update_type)
			};
			this.InvokeClanMemberListUpdatedEvent(clan_id, updates);
		}

		// Token: 0x06000DCB RID: 3531 RVA: 0x00036DA0 File Offset: 0x000351A0
		public EAddMemberResult AddClanMember(ulong clan_id, ulong profile_id)
		{
			if (this.m_dalService.ProfileSystem.GetProfileInfo(profile_id).Id != profile_id)
			{
				return EAddMemberResult.ProfileDoesNotExist;
			}
			return this.AddClanMemberImpl(clan_id, profile_id);
		}

		// Token: 0x06000DCC RID: 3532 RVA: 0x00036DD8 File Offset: 0x000351D8
		private EAddMemberResult AddClanMemberImpl(ulong clan_id, ulong profile_id)
		{
			EAddMemberResult eaddMemberResult = this.m_dalService.ClanSystem.AddClanMember(clan_id, profile_id, this.m_maxClanSize);
			if (eaddMemberResult == EAddMemberResult.Succeed)
			{
				ClanMember memberInfo = this.GetMemberInfo(profile_id);
				this.OnClanMemberUpdated(clan_id, memberInfo, EMembersListUpdate.Add);
			}
			return eaddMemberResult;
		}

		// Token: 0x06000DCD RID: 3533 RVA: 0x00036E16 File Offset: 0x00035216
		public ClanInfo GetClanInfo(ulong clan_id)
		{
			return this.m_dalService.ClanSystem.GetClanInfo(clan_id);
		}

		// Token: 0x06000DCE RID: 3534 RVA: 0x00036E29 File Offset: 0x00035229
		public ClanInfo GetClanInfoByName(string clanName)
		{
			return this.m_dalService.ClanSystem.GetClanInfoByName(clanName);
		}

		// Token: 0x06000DCF RID: 3535 RVA: 0x00036E3C File Offset: 0x0003523C
		private void OnConfigChanged(ConfigEventArgs e)
		{
			string name = e.Name;
			if (name != null)
			{
				if (!(name == "maxclansize"))
				{
					if (!(name == "clannameminsize"))
					{
						if (!(name == "clannamemaxsize"))
						{
							if (!(name == "kickeduserinvitetimeout"))
							{
								if (name == "inviteexpirationtimeout")
								{
									this.m_inviteExpiration = TimeSpan.FromSeconds((double)e.iValue);
								}
							}
							else
							{
								this.m_kickedUserInviteTimeout = e.uValue;
							}
						}
						else
						{
							this.m_clanNameMaxSize = e.uValue;
						}
					}
					else
					{
						this.m_clanNameMinSize = e.uValue;
					}
				}
				else
				{
					this.m_maxClanSize = e.uValue;
				}
			}
		}

		// Token: 0x06000DD0 RID: 3536 RVA: 0x00036F06 File Offset: 0x00035306
		public uint FixupClans()
		{
			return this.m_dalService.ClanSystem.FixupClans();
		}

		// Token: 0x06000DD1 RID: 3537 RVA: 0x00036F18 File Offset: 0x00035318
		private void OnNotificationConfirmed(SNotification notif, XmlNode confirmation_node)
		{
			if (notif.Type != ENotificationType.ClanInvite)
			{
				return;
			}
			SInvitationClanData data = Utils.GetTypeFromByteArray<SInvitationClanData>(notif.Data);
			string location = confirmation_node.Attributes["location"].Value;
			EInviteStatus status = (EInviteStatus)int.Parse(confirmation_node.Attributes["result"].Value);
			EInviteStatus result_status = status;
			if (status == EInviteStatus.Accepted)
			{
				result_status = ((!notif.IsExpired) ? this.InviteResponse(data, status) : EInviteStatus.Expired);
			}
			this.m_sessionInfoService.GetProfileInfoAsync(new ulong[]
			{
				data.Initiator.ProfileId,
				data.target_id
			}).ContinueWith(delegate(Task<List<ProfileInfo>> t1)
			{
				List<ProfileInfo> result = t1.Result;
				ProfileInfo profileInfo = result.FirstOrDefault((ProfileInfo x) => x.ProfileID == data.Initiator.ProfileId);
				ProfileInfo profileInfo2 = result.FirstOrDefault((ProfileInfo x) => x.ProfileID == data.target_id);
				SProfileInfo profileInfo3 = this.m_dalService.ProfileSystem.GetProfileInfo(data.target_id);
				ulong points = profileInfo3.RankInfo.Points;
				profileInfo2.Complete(profileInfo3);
				ClanMember memberInfo = this.m_dalService.ClanSystem.GetMemberInfo(data.target_id);
				this.SendInvitationResult(data.Initiator.ProfileId, profileInfo2.ProfileID, profileInfo2.Nickname, profileInfo2.OnlineID, (memberInfo == null) ? 0UL : memberInfo.InviteDate, profileInfo2.Status, location, points, result_status);
				this.m_logService.Event.ClanInviteMemberLog(data.Initiator.ProfileId, data.target_id, data.clan_id, result_status);
				if (result_status != status)
				{
					this.SendInvitationResult(data.target_id, profileInfo2.ProfileID, profileInfo2.Nickname, profileInfo2.OnlineID, 0UL, profileInfo2.Status, string.Empty, 0UL, result_status);
				}
			});
		}

		// Token: 0x06000DD2 RID: 3538 RVA: 0x00037014 File Offset: 0x00035414
		private void OnNotificationExpired(SPendingNotification notification)
		{
			ENotificationType type = (ENotificationType)notification.Type;
			if (type != ENotificationType.ClanInvite)
			{
				return;
			}
			SInvitationClanData typeFromByteArray = Utils.GetTypeFromByteArray<SInvitationClanData>(notification.Data);
			this.SendInvitationResult(typeFromByteArray.Initiator.ProfileId, typeFromByteArray.target_id, typeFromByteArray.reciever_name, string.Empty, 0UL, UserStatus.Online, string.Empty, 0UL, EInviteStatus.Expired);
			this.m_logService.Event.ClanInviteMemberLog(typeFromByteArray.Initiator.ProfileId, typeFromByteArray.target_id, typeFromByteArray.clan_id, EInviteStatus.Expired);
		}

		// Token: 0x06000DD3 RID: 3539 RVA: 0x0003709C File Offset: 0x0003549C
		private bool IsValidClanName(string name)
		{
			if ((long)name.Length < (long)((ulong)this.m_clanNameMinSize) || (long)name.Length > (long)((ulong)this.m_clanNameMaxSize))
			{
				return false;
			}
			char[] arr = new char[]
			{
				'-',
				'_',
				'.',
				'*',
				'[',
				']',
				'(',
				')'
			};
			UnicodeCategory[] array = new UnicodeCategory[5];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>{33a8b3dd-83c5-4517-9b3a-a5bd83bb24dc}.$field-3).FieldHandle);
			UnicodeCategory[] arr2 = array;
			for (int num = 0; num != name.Length; num++)
			{
				if (!Utils.Contains<char>(arr, name[num]))
				{
					UnicodeCategory unicodeCategory = char.GetUnicodeCategory(name[num]);
					if (!Utils.Contains<UnicodeCategory>(arr2, unicodeCategory))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000DD4 RID: 3540 RVA: 0x00037140 File Offset: 0x00035540
		private EClanCreationStatus IsCensoredClanName(ulong creatorUserId, string clanName)
		{
			ProfanityCheckResult profanityCheckResult = this.m_profanityCheckService.CheckClanName(creatorUserId, clanName);
			if (profanityCheckResult == ProfanityCheckResult.Failed)
			{
				return EClanCreationStatus.CensoredName;
			}
			if (profanityCheckResult != ProfanityCheckResult.Reserved)
			{
				return EClanCreationStatus.Created;
			}
			return EClanCreationStatus.NameReserved;
		}

		// Token: 0x06000DD5 RID: 3541 RVA: 0x00037174 File Offset: 0x00035574
		private string FilterClanDescription(ulong initiatorUserId, string description)
		{
			string @string = Encoding.UTF8.GetString(Convert.FromBase64String(description));
			StringBuilder stringBuilder = new StringBuilder(@string);
			ProfanityCheckResult profanityCheckResult = this.m_profanityCheckService.FilterClanDescription(initiatorUserId, stringBuilder);
			return (profanityCheckResult == ProfanityCheckResult.Succeeded) ? description : Convert.ToBase64String(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
		}

		// Token: 0x06000DD6 RID: 3542 RVA: 0x000371C8 File Offset: 0x000355C8
		private SProfileItem GetClanItem(ulong profile_id)
		{
			Dictionary<string, SItem> allItemsByName = this.m_itemCache.GetAllItemsByName();
			SItem sitem;
			if (!allItemsByName.TryGetValue(this.m_onlineVariables.Get("clans.clan_item", OnlineVariableDestination.Client), out sitem))
			{
				return null;
			}
			ulong id = sitem.ID;
			Dictionary<ulong, SProfileItem> profileItems = this.m_profileItems.GetProfileItems(profile_id, EquipOptions.ActiveOnly | EquipOptions.FilterByTags, (SProfileItem item) => item.ItemID == id);
			if (profileItems.Count == 0)
			{
				return null;
			}
			using (Dictionary<ulong, SProfileItem>.Enumerator enumerator = profileItems.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					KeyValuePair<ulong, SProfileItem> keyValuePair = enumerator.Current;
					return keyValuePair.Value;
				}
			}
			return null;
		}

		// Token: 0x06000DD7 RID: 3543 RVA: 0x00037294 File Offset: 0x00035694
		public EClanCreationStatus CreateClan(ulong clanOwnerId, ref ulong clanID, string clanName, string description)
		{
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(clanOwnerId);
			SProfileItem clanItem = this.GetClanItem(profileInfo.Id);
			if (clanItem == null)
			{
				this.m_logService.Event.ClanCreateLog(clanOwnerId, clanID, clanName, EClanCreationStatus.NeedBuyItem);
				return EClanCreationStatus.NeedBuyItem;
			}
			EClanCreationStatus eclanCreationStatus = this.CreateClanWithoutItem(clanOwnerId, ref clanID, clanName, description);
			if (eclanCreationStatus == EClanCreationStatus.Created)
			{
				this.m_catalogService.DeleteCustomerItem(profileInfo.UserID, clanItem.CatalogID);
				this.m_profileItems.DeleteProfileItem(profileInfo.Id, clanItem.ProfileItemID);
			}
			return eclanCreationStatus;
		}

		// Token: 0x06000DD8 RID: 3544 RVA: 0x00037324 File Offset: 0x00035724
		public EClanCreationStatus CreateClanWithoutItem(ulong clanOwnerId, ref ulong clanID, string clanName, string description)
		{
			EClanCreationStatus eclanCreationStatus = this.CreateClanImpl(clanOwnerId, ref clanID, clanName, description);
			this.m_logService.Event.ClanCreateLog(clanOwnerId, clanID, clanName, eclanCreationStatus);
			return eclanCreationStatus;
		}

		// Token: 0x06000DD9 RID: 3545 RVA: 0x00037354 File Offset: 0x00035754
		private EClanCreationStatus CreateClanImpl(ulong clanOwnerId, ref ulong clanID, string clanName, string description)
		{
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(clanOwnerId);
			if (profileInfo.Id != clanOwnerId)
			{
				return EClanCreationStatus.ServiceError;
			}
			if (!this.IsValidClanName(clanName))
			{
				return EClanCreationStatus.InvalidName;
			}
			EClanCreationStatus eclanCreationStatus = this.IsCensoredClanName(profileInfo.UserID, clanName);
			if (eclanCreationStatus != EClanCreationStatus.Created)
			{
				return eclanCreationStatus;
			}
			string description2 = this.FilterClanDescription(profileInfo.UserID, description);
			ClanMember memberInfo = this.m_dalService.ClanSystem.GetMemberInfo(profileInfo.Id);
			if (memberInfo != null)
			{
				return EClanCreationStatus.AlreadyClanMember;
			}
			NameReservationResult nameReservationResult = this.m_nameReservationService.ReserveName(clanName, NameReservationGroup.CLAN_NAMES, profileInfo.UserID);
			if (nameReservationResult != NameReservationResult.OK)
			{
				Log.Warning<string, ulong, NameReservationResult>("Failed to reserve clan name '{0}' for user {1}: {2}", clanName, profileInfo.UserID, nameReservationResult);
				return EClanCreationStatus.DuplicateName;
			}
			clanID = this.m_dalService.ClanSystem.CreateClan(profileInfo.Id, clanName, description2);
			if (clanID == 0UL)
			{
				return EClanCreationStatus.DuplicateName;
			}
			this.InvokeClanCreatedEvent(clanID, profileInfo.Id, profileInfo.Nickname);
			return EClanCreationStatus.Created;
		}

		// Token: 0x06000DDA RID: 3546 RVA: 0x00037448 File Offset: 0x00035848
		public bool RemoveClan(ulong initiator_id)
		{
			ClanMember memberInfo = this.m_dalService.ClanSystem.GetMemberInfo(initiator_id);
			if (memberInfo == null)
			{
				Log.Error<ulong>("[ClanService.RemoveClan] Initiator is not clan member (profile_id = {0})", initiator_id);
				return false;
			}
			if (memberInfo.ClanRole != EClanRole.MASTER)
			{
				Log.Error<ulong>("[ClanService.RemoveClan] Initiator is not master (profile_id = {0})", initiator_id);
				return false;
			}
			ClanInfo clanInfo = this.GetClanInfo(memberInfo.ClanID);
			if (clanInfo == null)
			{
				Log.Error<ulong>("[ClanService.RemoveClan] Clan not found (clan_id = {0})", memberInfo.ClanID);
				return false;
			}
			IEnumerable<ClanMember> clanMembers = this.m_dalService.ClanSystem.GetClanMembers(memberInfo.ClanID);
			this.RemoveClanFromDB(clanInfo);
			this.InvokeClanRemovedEvent(memberInfo.ClanID);
			this.m_logService.Event.ClanDeleteLog(clanInfo.ClanID);
			List<SClanMemberUpdate> list = new List<SClanMemberUpdate>();
			foreach (ClanMember clanMember in clanMembers)
			{
				if (clanMember.ProfileID != memberInfo.ProfileID)
				{
					this.SendNotification(clanMember.ProfileID, "@clans_your_clan_disbanded");
					list.Add(new SClanMemberUpdate(clanMember, EMembersListUpdate.Remove));
				}
				else
				{
					list.Add(new SClanMemberUpdate(clanMember, EMembersListUpdate.Disband));
				}
			}
			this.InvokeClanMemberListUpdatedEvent(memberInfo.ClanID, list);
			return true;
		}

		// Token: 0x06000DDB RID: 3547 RVA: 0x00037598 File Offset: 0x00035998
		public IEnumerable<ClanMember> GetClanMembers(ulong clan_id)
		{
			return this.m_dalService.ClanSystem.GetClanMembers(clan_id);
		}

		// Token: 0x06000DDC RID: 3548 RVA: 0x000375AC File Offset: 0x000359AC
		private bool CanKickClanMember(ClanMember initiator, ClanMember target)
		{
			return initiator.ClanID == target.ClanID && initiator.ProfileID != target.ProfileID && (initiator.ClanRole == EClanRole.MASTER || (initiator.ClanRole == EClanRole.OFFICER && target.ClanRole == EClanRole.REGULAR));
		}

		// Token: 0x06000DDD RID: 3549 RVA: 0x00037608 File Offset: 0x00035A08
		public bool KickFromClan(ulong initiator_id, ulong target_id)
		{
			ClanMember memberInfo = this.m_dalService.ClanSystem.GetMemberInfo(initiator_id);
			if (memberInfo == null)
			{
				Log.Error<ulong>("[ClanService.KickFromClan] Initiator is not clan member (profile_id = {0})", initiator_id);
				return false;
			}
			ClanMember memberInfo2 = this.m_dalService.ClanSystem.GetMemberInfo(target_id);
			if (memberInfo2 == null)
			{
				Log.Error<ulong>("[ClanService.KickFromClan] Target is not clan member (profile_id = {0})", target_id);
				return false;
			}
			if (!this.CanKickClanMember(memberInfo, memberInfo2))
			{
				Log.Error<ulong>("[ClanService.KickFromClan] Initiator don't have permissions for kick (profile_id = {0})", initiator_id);
				return false;
			}
			this.m_dalService.ClanSystem.KickClanMember(memberInfo2.ClanID, memberInfo2.ProfileID);
			this.m_logService.Event.ClanKickLog(initiator_id, target_id, memberInfo.ClanID);
			this.SendNotification(target_id, "@clans_you_was_kicked");
			this.OnClanMemberUpdated(memberInfo.ClanID, memberInfo2, EMembersListUpdate.Remove);
			return true;
		}

		// Token: 0x06000DDE RID: 3550 RVA: 0x000376C8 File Offset: 0x00035AC8
		public bool LeaveClan(ulong profile_id)
		{
			ClanMember memberInfo = this.m_dalService.ClanSystem.GetMemberInfo(profile_id);
			if (memberInfo == null)
			{
				Log.Error<ulong>("[ClanService.LeaveClan] Initiator is not clan member (profile_id = {0})", profile_id);
				return false;
			}
			ClanInfo clanInfo = this.m_dalService.ClanSystem.GetClanInfo(memberInfo.ClanID);
			if (clanInfo == null)
			{
				Log.Error<ulong>("[ClanService.LeaveClan] Clan not found (clan_id = {0})", memberInfo.ClanID);
				return false;
			}
			if (clanInfo.MembersCount == 1)
			{
				using (ILogGroup logGroup = this.m_logService.CreateGroup())
				{
					this.RemoveClanFromDB(clanInfo);
					this.OnClanMemberUpdated(memberInfo.ClanID, memberInfo, EMembersListUpdate.Disband);
					this.InvokeClanRemovedEvent(memberInfo.ClanID);
					logGroup.ClanLeaveLog(memberInfo.ProfileID, clanInfo.MembersCount, clanInfo.ClanID);
					logGroup.ClanDeleteLog(clanInfo.ClanID);
					return true;
				}
			}
			List<SClanMemberUpdate> list = new List<SClanMemberUpdate>(2);
			ulong num = this.m_dalService.ClanSystem.RemoveClanMember(memberInfo.ClanID, memberInfo.ProfileID);
			this.m_logService.Event.ClanLeaveLog(memberInfo.ProfileID, clanInfo.MembersCount, clanInfo.ClanID);
			if (num != 0UL)
			{
				this.SendNotification(num, "@clans_you_are_promoted_to_master");
				ClanMember memberInfo2 = this.GetMemberInfo(num);
				if (memberInfo2 != null)
				{
					list.Add(new SClanMemberUpdate(memberInfo2, EMembersListUpdate.Update));
					this.m_logService.Event.ClanSetRoleLog(memberInfo.ProfileID, memberInfo2.ProfileID, memberInfo2.ClanRole, clanInfo.ClanID);
				}
				else
				{
					Log.Warning<ulong, ulong>("Clan Leave can't find master info, clan id {0}, profile id {1}", clanInfo.ClanID, num);
				}
			}
			else if (memberInfo.ClanRole == EClanRole.MASTER)
			{
				Log.Warning<ulong>("Can't find new Master for clan {0}", clanInfo.ClanID);
			}
			list.Add(new SClanMemberUpdate(memberInfo, EMembersListUpdate.Remove));
			this.InvokeClanMemberListUpdatedEvent(memberInfo.ClanID, list);
			return true;
		}

		// Token: 0x06000DDF RID: 3551 RVA: 0x000378AC File Offset: 0x00035CAC
		private Task<EInviteStatus> Invite(ulong initiatorId, ulong target_id, string target_nickname)
		{
			Task<EInviteStatus> task = this.InviteImpl(initiatorId, target_id, target_nickname);
			return task.ContinueWith<EInviteStatus>(delegate(Task<EInviteStatus> t)
			{
				ClanMember memberInfo = this.GetMemberInfo(initiatorId);
				this.m_logService.Event.ClanInviteMemberLog(initiatorId, target_id, (memberInfo == null) ? 0UL : memberInfo.ClanID, task.Result);
				return t.Result;
			});
		}

		// Token: 0x06000DE0 RID: 3552 RVA: 0x00037904 File Offset: 0x00035D04
		public Task<EInviteStatus> Invite(UserInfo.User initiator, ulong target_id, string target_nickname)
		{
			return this.Invite(initiator.ProfileID, target_id, target_nickname);
		}

		// Token: 0x06000DE1 RID: 3553 RVA: 0x00037914 File Offset: 0x00035D14
		public Task<EInviteStatus> Invite(ulong sourceId, ulong targetId)
		{
			string nickname = this.m_dalService.ProfileSystem.GetProfileInfo(targetId).Nickname;
			return this.Invite(sourceId, targetId, nickname);
		}

		// Token: 0x06000DE2 RID: 3554 RVA: 0x00037944 File Offset: 0x00035D44
		private Task<EInviteStatus> InviteImpl(ulong initiatorId, ulong target_id, string target_nickname)
		{
			ClanMember memberInfo = this.GetMemberInfo(initiatorId);
			if (memberInfo == null)
			{
				return TaskHelpers.Completed<EInviteStatus>(EInviteStatus.InvalidState);
			}
			if (memberInfo.ClanRole != EClanRole.MASTER && memberInfo.ClanRole != EClanRole.OFFICER)
			{
				return TaskHelpers.Completed<EInviteStatus>(EInviteStatus.NoPermission);
			}
			ClanInfo clanInfo = this.GetClanInfo(memberInfo.ClanID);
			if (clanInfo == null)
			{
				return TaskHelpers.Completed<EInviteStatus>(EInviteStatus.ServiceError);
			}
			if ((long)clanInfo.MembersCount >= (long)((ulong)this.MaxClanSize))
			{
				return TaskHelpers.Completed<EInviteStatus>(EInviteStatus.LimitReached);
			}
			if (target_id == 0UL)
			{
				return TaskHelpers.Completed<EInviteStatus>(EInviteStatus.TargetInvalid);
			}
			if (initiatorId == target_id)
			{
				return TaskHelpers.Completed<EInviteStatus>(EInviteStatus.TargetInvalid);
			}
			IEnumerable<SNotification> pendingByType = this.m_notificationService.GetPendingByType(target_id, ENotificationType.ClanInvite);
			foreach (SNotification snotification in pendingByType)
			{
				if (!snotification.IsExpired && Utils.GetTypeFromByteArray<SInvitationClanData>(snotification.Data).clan_id == clanInfo.ClanID)
				{
					return TaskHelpers.Completed<EInviteStatus>(EInviteStatus.Duplicate);
				}
			}
			ClanMember memberInfo2 = this.GetMemberInfo(target_id);
			EInviteStatus status = EInviteStatus.Pending;
			if (memberInfo2 != null)
			{
				ClanInfo clanInfo2 = this.GetClanInfo(memberInfo2.ClanID);
				if (clanInfo2 != null)
				{
					status = EInviteStatus.AlreadyClanMember;
				}
			}
			double kickTime = this.GetKickTime(memberInfo.ClanID, target_id);
			if (kickTime < this.m_kickedUserInviteTimeout)
			{
				status = EInviteStatus.KickTimeout;
			}
			if (status == EInviteStatus.Pending)
			{
				Task task = this.SendInvitationRequest(target_id, target_nickname, initiatorId);
				task.ContinueWith(delegate(Task t)
				{
					Log.Error(t.Exception);
				}, TaskContinuationOptions.OnlyOnFaulted);
				return task.ContinueWith<EInviteStatus>((Task t) => status);
			}
			return TaskHelpers.Completed<EInviteStatus>(status);
		}

		// Token: 0x06000DE3 RID: 3555 RVA: 0x00037B2C File Offset: 0x00035F2C
		private EInviteStatus InviteResponse(SInvitationClanData invitatioData, EInviteStatus status)
		{
			if (status == EInviteStatus.Accepted)
			{
				ClanMember memberInfo = this.GetMemberInfo(invitatioData.Initiator.ProfileId);
				ClanMember memberInfo2 = this.GetMemberInfo(invitatioData.target_id);
				if (memberInfo == null)
				{
					return EInviteStatus.Expired;
				}
				if (memberInfo.ClanID != invitatioData.clan_id)
				{
					return EInviteStatus.Expired;
				}
				if (memberInfo2 != null && memberInfo2.ClanID == memberInfo.ClanID)
				{
					return status;
				}
				EAddMemberResult eaddMemberResult = this.AddClanMemberImpl(memberInfo.ClanID, invitatioData.target_id);
				if (eaddMemberResult != EAddMemberResult.Succeed)
				{
					if (eaddMemberResult != EAddMemberResult.Duplicate)
					{
						if (eaddMemberResult == EAddMemberResult.LimitReached)
						{
							status = EInviteStatus.LimitReached;
						}
					}
					else
					{
						status = EInviteStatus.AlreadyClanMember;
					}
				}
				else
				{
					status = EInviteStatus.Accepted;
				}
			}
			return status;
		}

		// Token: 0x06000DE4 RID: 3556 RVA: 0x00037BE0 File Offset: 0x00035FE0
		public void RemoteClanInfoUpdate(ulong profileId)
		{
			IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(profileId);
			if (roomByPlayer != null)
			{
				roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					PlayerClanInfo extension = r.GetExtension<PlayerClanInfo>();
					extension.UpdateClanName(profileId);
				});
			}
		}

		// Token: 0x06000DE5 RID: 3557 RVA: 0x00037C25 File Offset: 0x00036025
		private void SendNotification(ulong profile_id, string message)
		{
			this.m_notificationService.AddNotification<string>(profile_id, ENotificationType.Message, message, TimeSpan.FromDays(1.0), EDeliveryType.SendNowOrLater, EConfirmationType.None);
		}

		// Token: 0x06000DE6 RID: 3558 RVA: 0x00037C48 File Offset: 0x00036048
		public bool SetClanInfo(ulong initiator_id, string description)
		{
			ClanMember memberInfo = this.m_dalService.ClanSystem.GetMemberInfo(initiator_id);
			if (memberInfo == null)
			{
				Log.Error<ulong>("[ClanService.SetClanInfo] User is not clan member (profile_id = {0})", initiator_id);
				return false;
			}
			if (memberInfo.ClanRole != EClanRole.MASTER)
			{
				Log.Error<ulong>("[ClanService.SetClanInfo] User is not clan master (profile_id = {0})", initiator_id);
				return false;
			}
			string description2 = this.FilterClanDescription(initiator_id, description);
			this.m_dalService.ClanSystem.SetClanInfo(memberInfo.ClanID, description2);
			this.InvokeClanDescriptionUpdatedEvent(memberInfo.ClanID, description2);
			return true;
		}

		// Token: 0x06000DE7 RID: 3559 RVA: 0x00037CC4 File Offset: 0x000360C4
		public void AddClanPoints(ulong target_id, ulong clan_points)
		{
			ClanMember memberInfo = this.m_dalService.ClanSystem.GetMemberInfo(target_id);
			if (memberInfo == null)
			{
				Log.Error<ulong>("[ClanService.AddClanPoints] User is not clan member (profile_id = {0})", target_id);
				return;
			}
			memberInfo.ClanPoints = this.m_dalService.ClanSystem.AddClanPoints(memberInfo.ClanID, target_id, clan_points);
			this.InvokeClanMemberListUpdatedEvent(memberInfo.ClanID, new SClanMemberUpdate(memberInfo, EMembersListUpdate.Update));
		}

		// Token: 0x06000DE8 RID: 3560 RVA: 0x00037D28 File Offset: 0x00036128
		public bool SetClanRole(ulong initiator_id, ulong target_id, EClanRole role)
		{
			if (initiator_id == target_id)
			{
				Log.Error<ulong>("[ClanService.SetClanRole] User trying to kick himself (profile_id = {0})", initiator_id);
				return false;
			}
			ClanMember memberInfo = this.m_dalService.ClanSystem.GetMemberInfo(initiator_id);
			if (memberInfo == null)
			{
				Log.Error<ulong>("[ClanService.SetClanRole] Initiator is not clan member (profile_id = {0})", initiator_id);
				return false;
			}
			ClanMember memberInfo2 = this.m_dalService.ClanSystem.GetMemberInfo(target_id);
			if (memberInfo2 == null)
			{
				Log.Error<ulong>("[ClanService.SetClanRole] Target is not clan member (profile_id = {0})", target_id);
				return false;
			}
			if (memberInfo.ClanID != memberInfo2.ClanID)
			{
				Log.Error<ulong, ulong>("[ClanService.SetClanRole] Target and initiator are in diffirent clans (initiator_pid = {0}, target_pid = {1})", initiator_id, target_id);
				return false;
			}
			if (memberInfo.ClanRole != EClanRole.MASTER)
			{
				Log.Error<ulong>("[ClanService.SetClanRole] Initiator is not clan master (profile_id = {0})", initiator_id);
				return false;
			}
			if (memberInfo2.ClanRole == role)
			{
				Log.Error<ulong>("[ClanService.SetClanRole] Trying to set the same role (profile_id = {0})", target_id);
				return false;
			}
			List<SClanMemberUpdate> list = new List<SClanMemberUpdate>(2);
			this.m_dalService.ClanSystem.SetUserClanRole(memberInfo2.ClanID, initiator_id, target_id, (uint)role);
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				if (role == EClanRole.MASTER)
				{
					list.Add(new SClanMemberUpdate(memberInfo, EMembersListUpdate.Update));
					logGroup.ClanSetRoleLog(initiator_id, initiator_id, EClanRole.OFFICER, memberInfo2.ClanID);
				}
				logGroup.ClanSetRoleLog(initiator_id, target_id, role, memberInfo2.ClanID);
			}
			string message = string.Empty;
			if (role != EClanRole.REGULAR)
			{
				if (role != EClanRole.OFFICER)
				{
					if (role == EClanRole.MASTER)
					{
						message = "@clans_you_are_promoted_to_master";
					}
				}
				else
				{
					message = "@clans_you_are_promoted_to_officer";
				}
			}
			else
			{
				message = "@clans_you_are_demoted_to_regular";
			}
			this.SendNotification(target_id, message);
			list.Add(new SClanMemberUpdate(memberInfo2, EMembersListUpdate.Update));
			this.InvokeClanMemberListUpdatedEvent(memberInfo2.ClanID, list);
			return true;
		}

		// Token: 0x06000DE9 RID: 3561 RVA: 0x00037ED0 File Offset: 0x000362D0
		public ClanMember GetMemberInfo(ulong profile_id)
		{
			return this.m_dalService.ClanSystem.GetMemberInfo(profile_id);
		}

		// Token: 0x06000DEA RID: 3562 RVA: 0x00037EE4 File Offset: 0x000362E4
		public ClanInfo GetClanInfoByPid(ulong profile_id)
		{
			ClanMember memberInfo = this.GetMemberInfo(profile_id);
			return (memberInfo != null) ? this.GetClanInfo(memberInfo.ClanID) : null;
		}

		// Token: 0x06000DEB RID: 3563 RVA: 0x00037F14 File Offset: 0x00036314
		public double GetKickTime(ulong clan_id, ulong profile_id)
		{
			foreach (SClanKick sclanKick in this.m_dalService.ClanSystem.GetClanKicks(profile_id))
			{
				if (sclanKick.clan_id == clan_id)
				{
					return DateTime.Now.Subtract(sclanKick.kick_date).TotalSeconds;
				}
			}
			return double.PositiveInfinity;
		}

		// Token: 0x06000DEC RID: 3564 RVA: 0x00037FB0 File Offset: 0x000363B0
		public RewardOutputData ProcessRewardData(ulong userId, RewardProcessorState state, MissionContext missionContext, RewardOutputData aggRewardData, ILogGroup logGroup)
		{
			if (state != RewardProcessorState.Process)
			{
				return aggRewardData;
			}
			if (this.GetMemberInfo(aggRewardData.profileId) != null)
			{
				this.AddClanPoints(aggRewardData.profileId, (ulong)aggRewardData.gainedClanPoints);
			}
			return aggRewardData;
		}

		// Token: 0x06000DED RID: 3565 RVA: 0x00037FE4 File Offset: 0x000363E4
		private Task SendInvitationRequest(ulong receiverId, string receiverName, ulong initiatorProfileId)
		{
			ClanMember memberInfo = this.GetMemberInfo(initiatorProfileId);
			SInvitationClanData data = new SInvitationClanData
			{
				Initiator = CommonInitiatorData.CreateInitiatorData(this, this.m_userRepository, initiatorProfileId),
				clan_id = memberInfo.ClanID,
				target_id = receiverId,
				reciever_name = receiverName
			};
			return this.m_notificationService.AddNotification<SInvitationClanData>(receiverId, ENotificationType.ClanInvite, data, this.m_inviteExpiration, EDeliveryType.SendNowOrLater, EConfirmationType.Confirmation);
		}

		// Token: 0x06000DEE RID: 3566 RVA: 0x00038050 File Offset: 0x00036450
		private void SendInvitationResult(ulong receiver_id, ulong profileId, string nickname, string onlineId, ulong inviteDate, UserStatus userStatus, string location, ulong expirience, EInviteStatus result)
		{
			SInvitationResult data = new SInvitationResult
			{
				ProfileId = profileId,
				OnlineID = onlineId,
				Nickname = nickname,
				Location = location,
				Status = userStatus,
				Experience = expirience,
				Result = result,
				InvitationDate = inviteDate
			};
			this.m_notificationService.AddNotification<SInvitationResult>(receiver_id, ENotificationType.ClanInviteResult, data, this.m_inviteExpiration, EDeliveryType.SendNowOrLater, EConfirmationType.None);
		}

		// Token: 0x06000DEF RID: 3567 RVA: 0x000380C8 File Offset: 0x000364C8
		private void InvokeClanCreatedEvent(ulong clan_id, ulong clan_master_id, string clan_master)
		{
			if (this.ClanCreated == null)
			{
				return;
			}
			try
			{
				this.ClanCreated(clan_id, clan_master_id, clan_master);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x06000DF0 RID: 3568 RVA: 0x00038110 File Offset: 0x00036510
		private void RemoveClanFromDB(ClanInfo clan)
		{
			this.m_dalService.ClanSystem.RemoveClan(clan.ClanID);
			this.m_nameReservationService.CancelNameReservation(clan.Name, NameReservationGroup.CLAN_NAMES, 0UL);
		}

		// Token: 0x06000DF1 RID: 3569 RVA: 0x00038140 File Offset: 0x00036540
		private void InvokeClanRemovedEvent(ulong clan_id)
		{
			if (this.ClanRemoved == null)
			{
				return;
			}
			try
			{
				this.ClanRemoved(clan_id);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x06000DF2 RID: 3570 RVA: 0x00038188 File Offset: 0x00036588
		private void InvokeClanDescriptionUpdatedEvent(ulong clan_id, string description)
		{
			if (this.ClanDescriptionUpdated == null)
			{
				return;
			}
			try
			{
				this.ClanDescriptionUpdated(clan_id, description);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x06000DF3 RID: 3571 RVA: 0x000381D0 File Offset: 0x000365D0
		private void InvokeClanMemberListUpdatedEvent(ulong clan_id, SClanMemberUpdate update)
		{
			this.InvokeClanMemberListUpdatedEvent(clan_id, new List<SClanMemberUpdate>
			{
				update
			});
		}

		// Token: 0x06000DF4 RID: 3572 RVA: 0x000381F4 File Offset: 0x000365F4
		private void InvokeClanMemberListUpdatedEvent(ulong clan_id, List<SClanMemberUpdate> updates)
		{
			if (this.ClanMemberListUpdated == null)
			{
				return;
			}
			try
			{
				this.ClanMemberListUpdated(clan_id, updates);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x0400065A RID: 1626
		private readonly IDALService m_dalService;

		// Token: 0x0400065B RID: 1627
		private readonly INotificationService m_notificationService;

		// Token: 0x0400065C RID: 1628
		private readonly ISessionInfoService m_sessionInfoService;

		// Token: 0x0400065D RID: 1629
		private readonly IProfanityCheckService m_profanityCheckService;

		// Token: 0x0400065E RID: 1630
		private readonly IItemCache m_itemCache;

		// Token: 0x0400065F RID: 1631
		private readonly IProfileItems m_profileItems;

		// Token: 0x04000660 RID: 1632
		private readonly ICatalogService m_catalogService;

		// Token: 0x04000661 RID: 1633
		private readonly IOnlineVariables m_onlineVariables;

		// Token: 0x04000662 RID: 1634
		private readonly INameReservationService m_nameReservationService;

		// Token: 0x04000663 RID: 1635
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000664 RID: 1636
		private readonly ILogService m_logService;

		// Token: 0x04000665 RID: 1637
		private readonly IJobSchedulerService m_jobSchedulerService;

		// Token: 0x04000666 RID: 1638
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000667 RID: 1639
		private uint m_maxClanSize;

		// Token: 0x04000668 RID: 1640
		private uint m_clanNameMinSize;

		// Token: 0x04000669 RID: 1641
		private uint m_clanNameMaxSize;

		// Token: 0x0400066A RID: 1642
		private uint m_kickedUserInviteTimeout;

		// Token: 0x0400066B RID: 1643
		private TimeSpan m_inviteExpiration;
	}
}
