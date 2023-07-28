using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Users;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x02000271 RID: 625
	[Service]
	[Singleton]
	internal class ClanInfoUpdater : ServiceModule, IClanInfoUpdater
	{
		// Token: 0x06000D7A RID: 3450 RVA: 0x00035B5C File Offset: 0x00033F5C
		public ClanInfoUpdater(IClanService clanService, ISessionInfoService sessionInfoService, IGameRoomManager gameRoomManager, IOnlineClient onlineClient, IDALService dalService, ILogService logService, INotificationService notificationService, IUserRepository userRepository, IQueryManager queryManager, IClanMemberNicknameSynchronizer clanMemberNicknameSynchronizer)
		{
			this.m_sessionInfoService = sessionInfoService;
			this.m_gameRoomManager = gameRoomManager;
			this.m_onlineClient = onlineClient;
			this.m_dalService = dalService;
			this.m_logService = logService;
			this.m_notificationService = notificationService;
			this.m_logService = logService;
			this.m_userRepository = userRepository;
			this.m_queryManager = queryManager;
			this.m_clanMemberNicknameSynchronizer = clanMemberNicknameSynchronizer;
			this.m_clanService = clanService;
		}

		// Token: 0x06000D7B RID: 3451 RVA: 0x00035BC4 File Offset: 0x00033FC4
		public override void Start()
		{
			base.Start();
			this.m_userRepository.UserLoggedIn += this.OnUserLoggedIn;
			this.m_clanService.ClanMemberListUpdated += this.ClanMemberListUpdated;
			this.m_clanService.ClanCreated += this.OnClanCreated;
			this.m_clanService.ClanDescriptionUpdated += this.OnClanDescriptionUpdated;
			this.m_clanMemberNicknameSynchronizer.ClanMemberRenamed += this.OnClanMemberRenamed;
		}

		// Token: 0x06000D7C RID: 3452 RVA: 0x00035C4C File Offset: 0x0003404C
		public override void Stop()
		{
			this.m_userRepository.UserLoggedIn -= this.OnUserLoggedIn;
			this.m_clanService.ClanMemberListUpdated -= this.ClanMemberListUpdated;
			this.m_clanService.ClanCreated -= this.OnClanCreated;
			this.m_clanService.ClanDescriptionUpdated -= this.OnClanDescriptionUpdated;
			this.m_clanMemberNicknameSynchronizer.ClanMemberRenamed -= this.OnClanMemberRenamed;
			base.Stop();
		}

		// Token: 0x06000D7D RID: 3453 RVA: 0x00035CD4 File Offset: 0x000340D4
		public void RepairAndSendClanInfo(string onlineId, ulong profileId)
		{
			ClanMember memberInfo = this.m_clanService.GetMemberInfo(profileId);
			if (memberInfo == null)
			{
				return;
			}
			ClanInfo clanInfo = this.m_clanService.GetClanInfo(memberInfo.ClanID);
			IEnumerable<ClanMember> clanMembers = this.m_clanService.GetClanMembers(memberInfo.ClanID);
			if (clanInfo != null)
			{
				this.ValidateClanMaster(clanInfo, clanMembers);
			}
			this.SendClanInfo(memberInfo.ClanID, memberInfo.Nickname, onlineId);
		}

		// Token: 0x06000D7E RID: 3454 RVA: 0x00035D3A File Offset: 0x0003413A
		private void OnUserLoggedIn(UserInfo.User user, ELoginType loginType)
		{
			if (loginType == ELoginType.Ordinary)
			{
				this.RepairAndSendClanInfo(user.OnlineID, user.ProfileID);
			}
		}

		// Token: 0x06000D7F RID: 3455 RVA: 0x00035D54 File Offset: 0x00034154
		private void SendClanInfo(ulong clan_id, string nickname, string onlineID)
		{
			Dictionary<ulong, ClanMember> clanMembers = this.m_clanService.GetClanMembers(clan_id).ToDictionary((ClanMember member) => member.ProfileID);
			ClanInfo clanInfo = this.m_clanService.GetClanInfo(clan_id);
			if (clanInfo == null)
			{
				return;
			}
			this.m_sessionInfoService.GetProfileInfoAsync(clanMembers.Keys).ContinueWith(delegate(Task<List<ProfileInfo>> t1)
			{
				List<ProfileInfo> result = t1.Result;
				if (string.IsNullOrEmpty(onlineID))
				{
					onlineID = result.FirstOrDefault((ProfileInfo x) => x.Nickname == nickname).OnlineID;
				}
				this.SendClanInfo(onlineID, clanInfo, result, clanMembers);
			});
		}

		// Token: 0x06000D80 RID: 3456 RVA: 0x00035DF8 File Offset: 0x000341F8
		private void ValidateClanMaster(ClanInfo clanInfo, IEnumerable<ClanMember> clanMembers)
		{
			if (clanMembers.FirstOrDefault((ClanMember x) => x.ProfileID == clanInfo.MasterId) == null)
			{
				List<SClanMemberUpdate> list = new List<SClanMemberUpdate>(1);
				ulong num = this.m_dalService.ClanSystem.RemoveClanMember(clanInfo.ClanID, clanInfo.MasterId);
				if (num != 0UL)
				{
					ClanMember memberInfo = this.m_clanService.GetMemberInfo(num);
					if (memberInfo != null)
					{
						list.Add(new SClanMemberUpdate(memberInfo, EMembersListUpdate.Update));
						this.m_logService.Event.ClanSetRoleLog(clanInfo.MasterId, num, EClanRole.MASTER, clanInfo.ClanID);
						this.SendNotification(num, "@clans_you_are_promoted_to_master");
						this.ClanMemberListUpdated(clanInfo.ClanID, list);
					}
					else
					{
						Log.Warning<ulong, ulong>("Clan Leave can't find master info, clan id {0}, profile id {1}", clanInfo.ClanID, num);
					}
					clanInfo = this.m_clanService.GetClanInfo(clanInfo.ClanID);
					clanMembers = this.m_clanService.GetClanMembers(clanInfo.ClanID);
				}
				else
				{
					Log.Warning<ulong>("Can't find new Master for clan {0}", clanInfo.ClanID);
				}
			}
		}

		// Token: 0x06000D81 RID: 3457 RVA: 0x00035F33 File Offset: 0x00034333
		private void SendNotification(ulong profile_id, string message)
		{
			this.m_notificationService.AddNotification<string>(profile_id, ENotificationType.Message, message, TimeSpan.FromDays(1.0), EDeliveryType.SendNowOrLater, EConfirmationType.None);
		}

		// Token: 0x06000D82 RID: 3458 RVA: 0x00035F54 File Offset: 0x00034354
		private void SendClanInfo(string receiver, ClanInfo info, IEnumerable<ProfileInfo> profiles, IDictionary<ulong, ClanMember> clanMembers)
		{
			Log.Verbose("Clan Members of {0} resolved", new object[]
			{
				receiver
			});
			List<ClanMemberInfo> list = new List<ClanMemberInfo>();
			foreach (ProfileInfo pi in profiles)
			{
				ClanMember clanMember;
				if (clanMembers.TryGetValue(pi.ProfileID, out clanMember))
				{
					if (pi.IsPartial())
					{
						SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(clanMember.ProfileID);
						pi.Complete(profileInfo);
					}
					list.Add(new ClanMemberInfo(pi, clanMember));
				}
			}
			this.m_queryManager.Request("clan_info", receiver, new object[]
			{
				info,
				list
			});
		}

		// Token: 0x06000D83 RID: 3459 RVA: 0x00036030 File Offset: 0x00034430
		private void OnClanDescriptionUpdated(ulong clan_id, string description)
		{
			List<string> list = new List<string>();
			foreach (ClanMember clanMember in this.m_clanService.GetClanMembers(clan_id))
			{
				list.Add(clanMember.Nickname);
			}
			this.m_sessionInfoService.GetProfileInfo(list, delegate(IEnumerable<ProfileInfo> info)
			{
				foreach (ProfileInfo profileInfo in info)
				{
					if (!string.IsNullOrEmpty(profileInfo.OnlineID))
					{
						this.m_queryManager.Request("clan_description_updated", profileInfo.OnlineID, new object[]
						{
							description
						});
					}
				}
			});
		}

		// Token: 0x06000D84 RID: 3460 RVA: 0x000360C8 File Offset: 0x000344C8
		private void OnClanCreated(ulong clan_id, ulong clan_master_id, string clan_master)
		{
			List<ulong> pids = new List<ulong>
			{
				clan_master_id
			};
			this.BroadcastClanMemberListUpdated(pids);
		}

		// Token: 0x06000D85 RID: 3461 RVA: 0x000360EC File Offset: 0x000344EC
		private void ClanMemberListUpdated(ulong clan_id, List<SClanMemberUpdate> updates)
		{
			List<ulong> list = new List<ulong>();
			ClanInfo clanInfo = this.m_clanService.GetClanInfo(clan_id);
			IEnumerable<ClanMember> clanMembers = this.m_clanService.GetClanMembers(clan_id);
			if (clanInfo != null)
			{
				this.ValidateClanMaster(clanInfo, clanMembers);
			}
			foreach (SClanMemberUpdate sclanMemberUpdate in updates)
			{
				if (sclanMemberUpdate.update_type == EMembersListUpdate.Add || sclanMemberUpdate.update_type == EMembersListUpdate.Remove || sclanMemberUpdate.update_type == EMembersListUpdate.Disband)
				{
					list.Add(sclanMemberUpdate.member_info.ProfileID);
					if (sclanMemberUpdate.update_type == EMembersListUpdate.Remove)
					{
						this.m_sessionInfoService.GetProfileInfo(sclanMemberUpdate.member_info.Nickname, delegate(ProfileInfo profileInfo)
						{
							if (!string.IsNullOrEmpty(profileInfo.OnlineID))
							{
								this.NotifyClanLeave(profileInfo.OnlineID);
							}
						});
					}
					if (sclanMemberUpdate.update_type == EMembersListUpdate.Add)
					{
						this.SendClanInfo(clan_id, sclanMemberUpdate.member_info.Nickname, null);
					}
				}
			}
			this.BroadcastClanMemberListUpdated(list);
			List<ulong> list2 = new List<ulong>();
			using (IEnumerator<ClanMember> enumerator2 = clanMembers.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					ClanMember member = enumerator2.Current;
					if (!Utils.Contains<SClanMemberUpdate>(updates, (SClanMemberUpdate X) => X.update_type == EMembersListUpdate.Remove && X.member_info.Nickname == member.Nickname))
					{
						list2.Add(member.ProfileID);
					}
					for (int i = 0; i < updates.Count; i++)
					{
						if (updates[i].member_info.ProfileID == member.ProfileID)
						{
							SClanMemberUpdate value = updates[i];
							value.member_info = member;
							updates[i] = value;
						}
					}
				}
			}
			this.m_sessionInfoService.GetProfileInfo(list2, delegate(IEnumerable<ProfileInfo> r)
			{
				this.NotifyMembersUpdated(r, updates);
			});
		}

		// Token: 0x06000D86 RID: 3462 RVA: 0x0003632C File Offset: 0x0003472C
		private void OnClanMemberRenamed(ClanMember clanMember)
		{
			SClanMemberUpdate item = new SClanMemberUpdate(clanMember, EMembersListUpdate.Update);
			this.ClanMemberListUpdated(clanMember.ClanID, new List<SClanMemberUpdate>
			{
				item
			});
		}

		// Token: 0x06000D87 RID: 3463 RVA: 0x0003635C File Offset: 0x0003475C
		private void BroadcastClanMemberListUpdated(List<ulong> pids)
		{
			ClanInfoUpdater.<BroadcastClanMemberListUpdated>c__AnonStorey5 <BroadcastClanMemberListUpdated>c__AnonStorey = new ClanInfoUpdater.<BroadcastClanMemberListUpdated>c__AnonStorey5();
			<BroadcastClanMemberListUpdated>c__AnonStorey.pids = pids;
			int i;
			for (i = <BroadcastClanMemberListUpdated>c__AnonStorey.pids.Count; i > 0; i--)
			{
				IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(<BroadcastClanMemberListUpdated>c__AnonStorey.pids[i - 1]);
				if (roomByPlayer != null)
				{
					roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						PlayerClanInfo extension = r.GetExtension<PlayerClanInfo>();
						extension.UpdateClanName(<BroadcastClanMemberListUpdated>c__AnonStorey.pids[i - 1]);
					});
					<BroadcastClanMemberListUpdated>c__AnonStorey.pids.RemoveAt(i - 1);
				}
			}
			if (<BroadcastClanMemberListUpdated>c__AnonStorey.pids.Count > 0)
			{
				this.m_queryManager.Request("player_clan_info_updated", "k01." + this.m_onlineClient.XmppHost, new object[]
				{
					<BroadcastClanMemberListUpdated>c__AnonStorey.pids
				});
			}
		}

		// Token: 0x06000D88 RID: 3464 RVA: 0x00036443 File Offset: 0x00034843
		private void NotifyClanLeave(string receiver)
		{
			this.m_queryManager.Request("clan_info", receiver, new object[2]);
		}

		// Token: 0x06000D89 RID: 3465 RVA: 0x0003645C File Offset: 0x0003485C
		private void NotifyMembersUpdated(IEnumerable<ProfileInfo> recipients, IList<SClanMemberUpdate> updates)
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (IEnumerator<ProfileInfo> enumerator = recipients.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ProfileInfo recipient = enumerator.Current;
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(",");
					}
					if (!string.IsNullOrEmpty(recipient.OnlineID) && !Utils.Contains<SClanMemberUpdate>(updates, (SClanMemberUpdate X) => X.member_info.Nickname == recipient.Nickname && X.update_type == EMembersListUpdate.Add))
					{
						stringBuilder.Append(recipient.OnlineID);
					}
				}
			}
			SClanMemberUpdate sclanMemberUpdate = updates.FirstOrDefault((SClanMemberUpdate x) => x.member_info.ClanRole == EClanRole.MASTER);
			if (stringBuilder.Length > 0)
			{
				this.m_queryManager.Request("clan_members_updated", "k01." + this.m_onlineClient.XmppHost, new object[]
				{
					stringBuilder.ToString(),
					updates,
					recipients
				});
				if (sclanMemberUpdate.member_info != null)
				{
					this.m_queryManager.Request("clan_masterbanner_update", "k01." + this.m_onlineClient.XmppHost, new object[]
					{
						stringBuilder.ToString(),
						sclanMemberUpdate.member_info.ProfileID
					});
				}
			}
		}

		// Token: 0x0400063D RID: 1597
		private readonly ISessionInfoService m_sessionInfoService;

		// Token: 0x0400063E RID: 1598
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x0400063F RID: 1599
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x04000640 RID: 1600
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000641 RID: 1601
		private readonly IClanService m_clanService;

		// Token: 0x04000642 RID: 1602
		private readonly IClanMemberNicknameSynchronizer m_clanMemberNicknameSynchronizer;

		// Token: 0x04000643 RID: 1603
		private readonly IDALService m_dalService;

		// Token: 0x04000644 RID: 1604
		private readonly ILogService m_logService;

		// Token: 0x04000645 RID: 1605
		private readonly INotificationService m_notificationService;

		// Token: 0x04000646 RID: 1606
		private readonly IQueryManager m_queryManager;
	}
}
