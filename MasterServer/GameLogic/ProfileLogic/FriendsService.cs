using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.GameLogic.InvitationSystem;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.StatsTracking;
using MasterServer.Platform.Nickname;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000554 RID: 1364
	[Service]
	[Singleton]
	internal class FriendsService : ServiceModule, IFriendsService
	{
		// Token: 0x06001D58 RID: 7512 RVA: 0x00076974 File Offset: 0x00074D74
		public FriendsService(IDALService dalService, ISessionInfoService sessionInfoService, INotificationService notificationService, IUserRepository userRepository, ILogService logService, IExternalNicknameSyncService externalNicknameSync, IProfileValidationService profileValidationService, IStatsTracker statsTracker, IClanService clanService)
		{
			this.m_dalService = dalService;
			this.m_sessionInfoService = sessionInfoService;
			this.m_notificationService = notificationService;
			this.m_userRepository = userRepository;
			this.m_logService = logService;
			this.m_externalNicknameSyncService = externalNicknameSync;
			this.m_profileValidationService = profileValidationService;
			this.m_statsTracker = statsTracker;
			this.m_clanService = clanService;
		}

		// Token: 0x06001D59 RID: 7513 RVA: 0x000769CC File Offset: 0x00074DCC
		public override void Init()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("Friends");
			section.OnConfigChanged += this.OnConfigChanged;
			this.m_notificationService.OnNotificationConfirmed += this.OnNotificationConfirmed;
			this.m_notificationService.OnNotificationExpired += this.OnNotificationExpired;
			this.m_externalNicknameSyncService.ProfileRenamed += this.OnProfileRenamed;
			this.m_friendsLimit = uint.Parse(section.Get("Limit"));
			this.m_inviteExpiration = new TimeSpan(0, 0, int.Parse(section.Get("AddFriendExpiration")));
		}

		// Token: 0x06001D5A RID: 7514 RVA: 0x00076A73 File Offset: 0x00074E73
		public override void Stop()
		{
			this.m_externalNicknameSyncService.ProfileRenamed -= this.OnProfileRenamed;
		}

		// Token: 0x06001D5B RID: 7515 RVA: 0x00076A8C File Offset: 0x00074E8C
		public void SendFriendListUpdate(ulong profileId, string onlineId)
		{
			List<ulong> list = (from f in this.m_dalService.ProfileSystem.GetFriends(profileId)
			select f.ProfileID).ToList<ulong>();
			Log.Verbose("Resolving {0} friends for {1}", new object[]
			{
				list.Count,
				onlineId
			});
			this.m_sessionInfoService.GetProfileInfoAsync(list).ContinueWith(delegate(Task<List<ProfileInfo>> t1)
			{
				this.OnFriendListResolved(onlineId, t1.Result);
			});
		}

		// Token: 0x06001D5C RID: 7516 RVA: 0x00076B2C File Offset: 0x00074F2C
		private void RemoveFriend(ulong initiatorId, string targetNickname)
		{
			Log.Verbose("Canceling friendship between {0} and {1}", new object[]
			{
				initiatorId,
				targetNickname
			});
			SFriend sfriend;
			if (!Utils.Find<SFriend>(this.m_dalService.ProfileSystem.GetFriends(initiatorId), (SFriend x) => x.Nickname == targetNickname, out sfriend))
			{
				return;
			}
			this.m_notificationService.DeletePendingByType(sfriend.ProfileID, ENotificationType.FriendInviteResult);
			this.m_dalService.ProfileSystem.RemoveFriend(initiatorId, sfriend.ProfileID);
			this.m_logService.Event.FriendRemoveLog(initiatorId, sfriend.ProfileID);
			this.m_sessionInfoService.GetProfileInfo(targetNickname, new ProfileInfoCallback(this.OnFriendRemoveResolved));
			this.OnFriendListChanged(initiatorId, sfriend.ProfileID, false);
		}

		// Token: 0x06001D5D RID: 7517 RVA: 0x00076C05 File Offset: 0x00075005
		public void RemoveFriend(UserInfo.User initiator, string targetNickname)
		{
			this.RemoveFriend(initiator.ProfileID, targetNickname);
		}

		// Token: 0x06001D5E RID: 7518 RVA: 0x00076C14 File Offset: 0x00075014
		public void RemoveFriend(ulong sourceId, ulong targetId)
		{
			string nickname = this.m_dalService.ProfileSystem.GetProfileInfo(targetId).Nickname;
			this.RemoveFriend(sourceId, nickname);
		}

		// Token: 0x06001D5F RID: 7519 RVA: 0x00076C44 File Offset: 0x00075044
		private Task<EInviteStatus> Invite(ulong initiatorUserId, ulong initiatorProfileId, ulong targetProfileId, string targetNickname)
		{
			Task<EInviteStatus> task = this.InviteImpl(initiatorUserId, initiatorProfileId, targetProfileId, targetNickname);
			return task.ContinueWith<EInviteStatus>(delegate(Task<EInviteStatus> t)
			{
				this.m_logService.Event.FriendInviteLog(initiatorProfileId, targetProfileId, t.Result);
				return t.Result;
			});
		}

		// Token: 0x06001D60 RID: 7520 RVA: 0x00076C94 File Offset: 0x00075094
		public Task<EInviteStatus> Invite(UserInfo.User initiator, ulong targetProfileId, string targetNickname)
		{
			return this.Invite(initiator.UserID, initiator.ProfileID, targetProfileId, targetNickname);
		}

		// Token: 0x06001D61 RID: 7521 RVA: 0x00076CAC File Offset: 0x000750AC
		public Task<EInviteStatus> Invite(ulong sourceId, ulong targetId)
		{
			ulong userID = this.m_dalService.ProfileSystem.GetProfileInfo(sourceId).UserID;
			string targetNickname = this.m_dalService.ProfileSystem.GetProfileInfo(targetId).Nickname ?? string.Empty;
			return this.Invite(userID, sourceId, targetId, targetNickname);
		}

		// Token: 0x06001D62 RID: 7522 RVA: 0x00076D04 File Offset: 0x00075104
		private Task<EInviteStatus> InviteImpl(ulong initiatorUserId, ulong initiatorProfileId, ulong targetProfileId, string targetNickname)
		{
			if (this.m_profileValidationService.ValidateNickname(targetNickname) != NameValidationResult.NoError)
			{
				return TaskHelpers.Completed<EInviteStatus>(EInviteStatus.TargetInvalid);
			}
			IEnumerable<SFriend> friends = this.m_dalService.ProfileSystem.GetFriends(initiatorProfileId);
			if ((long)friends.Count<SFriend>() >= (long)((ulong)this.m_friendsLimit))
			{
				return TaskHelpers.Completed<EInviteStatus>(EInviteStatus.LimitReached);
			}
			if (targetProfileId == 0UL)
			{
				return TaskHelpers.Completed<EInviteStatus>(EInviteStatus.TargetInvalid);
			}
			if (targetProfileId == initiatorProfileId)
			{
				return TaskHelpers.Completed<EInviteStatus>(EInviteStatus.TargetInvalid);
			}
			if (friends.Any((SFriend f) => f.ProfileID == targetProfileId))
			{
				return TaskHelpers.Completed<EInviteStatus>(EInviteStatus.Duplicate);
			}
			IEnumerable<SNotification> pendingByType = this.m_notificationService.GetPendingByType(targetProfileId, ENotificationType.FriendInvite);
			foreach (SNotification snotification in pendingByType)
			{
				if (!snotification.IsExpired && Utils.GetTypeFromByteArray<SInvitationFriendData>(snotification.Data).Initiator.ProfileId == initiatorProfileId)
				{
					return TaskHelpers.Completed<EInviteStatus>(EInviteStatus.InviteInProgress);
				}
			}
			pendingByType = this.m_notificationService.GetPendingByType(initiatorProfileId, ENotificationType.FriendInvite);
			foreach (SNotification snotification2 in pendingByType)
			{
				if (Utils.GetTypeFromByteArray<SInvitationFriendData>(snotification2.Data).Initiator.ProfileId == targetProfileId)
				{
					return TaskHelpers.Completed<EInviteStatus>(EInviteStatus.InviteInProgress);
				}
			}
			IEnumerable<SFriend> friends2 = this.m_dalService.ProfileSystem.GetFriends(targetProfileId);
			if ((long)friends2.Count<SFriend>() >= (long)((ulong)this.m_friendsLimit))
			{
				return TaskHelpers.Completed<EInviteStatus>(EInviteStatus.TargetLimitReached);
			}
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(targetProfileId);
			if (Resources.BootstrapMode && !this.m_userRepository.IsSameBootstrap(initiatorUserId, profileInfo.UserID))
			{
				return TaskHelpers.Completed<EInviteStatus>(EInviteStatus.TargetInvalid);
			}
			Task task = this.SendInvitationRequest(targetProfileId, profileInfo.UserID, profileInfo.Nickname, initiatorProfileId);
			task.ContinueWith(delegate(Task t)
			{
				Log.Error(t.Exception);
			}, TaskContinuationOptions.OnlyOnFaulted);
			return task.ContinueWith<EInviteStatus>((Task t) => EInviteStatus.Pending);
		}

		// Token: 0x06001D63 RID: 7523 RVA: 0x00076FA4 File Offset: 0x000753A4
		private EInviteStatus InviteResponse(ulong initiatorId, ulong targetId, EInviteStatus status)
		{
			try
			{
				if (status == EInviteStatus.Accepted)
				{
					EAddMemberResult eaddMemberResult = this.m_dalService.ProfileSystem.AddFriend(initiatorId, targetId, this.m_friendsLimit);
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
							status = EInviteStatus.Duplicate;
						}
					}
					else
					{
						status = EInviteStatus.Accepted;
					}
					if (status == EInviteStatus.Accepted)
					{
						this.OnFriendListChanged(initiatorId, targetId, true);
					}
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
				status = EInviteStatus.ServiceError;
			}
			return status;
		}

		// Token: 0x06001D64 RID: 7524 RVA: 0x00077038 File Offset: 0x00075438
		private void OnNotificationConfirmed(SNotification notif, XmlNode confirmationNode)
		{
			if (notif.Type != ENotificationType.FriendInvite)
			{
				return;
			}
			SInvitationFriendData data = Utils.GetTypeFromByteArray<SInvitationFriendData>(notif.Data);
			string location = confirmationNode.Attributes["location"].Value;
			EInviteStatus status = (EInviteStatus)int.Parse(confirmationNode.Attributes["result"].Value);
			EInviteStatus resultStatus = status;
			if (Resources.BootstrapMode && !this.m_userRepository.IsSameBootstrap(data.Initiator.UserId, data.TargetUserId))
			{
				status = EInviteStatus.TargetInvalid;
				resultStatus = EInviteStatus.TargetInvalid;
			}
			if (status == EInviteStatus.Accepted)
			{
				resultStatus = ((!notif.IsExpired) ? this.InviteResponse(data.Initiator.ProfileId, data.TargetId, status) : EInviteStatus.Expired);
			}
			this.m_sessionInfoService.GetProfileInfoAsync(new ulong[]
			{
				data.Initiator.ProfileId,
				data.TargetId
			}).ContinueWith(delegate(Task<List<ProfileInfo>> t)
			{
				List<ProfileInfo> result = t.Result;
				ProfileInfo profileInfo = result.FirstOrDefault((ProfileInfo x) => x.ProfileID == data.Initiator.ProfileId);
				ProfileInfo profileInfo2 = result.FirstOrDefault((ProfileInfo x) => x.ProfileID == data.TargetId);
				SProfileInfo profileInfo3 = this.m_dalService.ProfileSystem.GetProfileInfo(data.TargetId);
				profileInfo2.Complete(profileInfo3);
				ulong points = profileInfo3.RankInfo.Points;
				this.SendInvitationResult(data.Initiator.ProfileId, profileInfo2.ProfileID, profileInfo2.Nickname, profileInfo2.OnlineID, profileInfo2.Status, location, points, resultStatus);
				this.m_logService.Event.FriendInviteLog(data.Initiator.ProfileId, data.TargetId, resultStatus);
				if (status == EInviteStatus.Accepted)
				{
					SProfileInfo profileInfo4 = this.m_dalService.ProfileSystem.GetProfileInfo(data.Initiator.ProfileId);
					profileInfo.Complete(profileInfo4);
					points = profileInfo4.RankInfo.Points;
					this.SendInvitationResult(data.TargetId, profileInfo.ProfileID, profileInfo.Nickname, profileInfo.OnlineID, profileInfo.Status, string.Empty, points, resultStatus);
				}
			});
		}

		// Token: 0x06001D65 RID: 7525 RVA: 0x0007718C File Offset: 0x0007558C
		private void OnNotificationExpired(SPendingNotification notification)
		{
			ENotificationType type = (ENotificationType)notification.Type;
			if (type != ENotificationType.FriendInvite)
			{
				return;
			}
			SInvitationFriendData typeFromByteArray = Utils.GetTypeFromByteArray<SInvitationFriendData>(notification.Data);
			this.SendInvitationResult(typeFromByteArray.Initiator.ProfileId, typeFromByteArray.TargetId, typeFromByteArray.RecieverName, string.Empty, UserStatus.Online, string.Empty, 0UL, EInviteStatus.Expired);
			this.m_logService.Event.FriendInviteLog(typeFromByteArray.Initiator.ProfileId, typeFromByteArray.TargetId, EInviteStatus.Expired);
		}

		// Token: 0x06001D66 RID: 7526 RVA: 0x0007720C File Offset: 0x0007560C
		private void OnProfileRenamed(ulong profileId)
		{
			IEnumerable<string> nicknames = from f in this.m_dalService.ProfileSystem.GetFriends(profileId)
			select f.Nickname;
			this.m_sessionInfoService.GetProfileInfo(nicknames, delegate(IEnumerable<ProfileInfo> friends)
			{
				foreach (ProfileInfo profileInfo in friends)
				{
					this.SendFriendListUpdate(profileInfo.ProfileID, profileInfo.OnlineID);
				}
			});
		}

		// Token: 0x06001D67 RID: 7527 RVA: 0x00077268 File Offset: 0x00075668
		private Task SendInvitationRequest(ulong receiverId, ulong receiverUserId, string receiverName, ulong initiatorProfileId)
		{
			SInvitationFriendData data = new SInvitationFriendData
			{
				Initiator = CommonInitiatorData.CreateInitiatorData(this.m_clanService, this.m_userRepository, initiatorProfileId),
				TargetId = receiverId,
				TargetUserId = receiverUserId,
				RecieverName = receiverName
			};
			return this.m_notificationService.AddNotification<SInvitationFriendData>(receiverId, ENotificationType.FriendInvite, data, this.m_inviteExpiration, EDeliveryType.SendNowOrLater, EConfirmationType.Confirmation);
		}

		// Token: 0x06001D68 RID: 7528 RVA: 0x000772CC File Offset: 0x000756CC
		private void SendInvitationResult(ulong receiverId, ulong profileId, string nickname, string onlineId, UserStatus userStatus, string location, ulong expirience, EInviteStatus result)
		{
			SInvitationResult data = new SInvitationResult
			{
				ProfileId = profileId,
				OnlineID = onlineId,
				Nickname = nickname,
				Location = location,
				Status = userStatus,
				Experience = expirience,
				Result = result
			};
			this.m_notificationService.AddNotification<SInvitationResult>(receiverId, ENotificationType.FriendInviteResult, data, this.m_inviteExpiration, EDeliveryType.SendNowOrLater, EConfirmationType.None);
		}

		// Token: 0x06001D69 RID: 7529 RVA: 0x00077340 File Offset: 0x00075740
		private void OnConfigChanged(ConfigEventArgs e)
		{
			if (string.Equals(e.Name, "Limit", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_friendsLimit = e.uValue;
			}
			else if (string.Equals(e.Name, "AddFriendExpiration", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_inviteExpiration = new TimeSpan(0, 0, e.iValue);
			}
		}

		// Token: 0x06001D6A RID: 7530 RVA: 0x000773A0 File Offset: 0x000757A0
		private void OnFriendListResolved(string receiver, IEnumerable<ProfileInfo> friends)
		{
			Log.Verbose("Friends of {0} resolved", new object[]
			{
				receiver
			});
			List<FriendInfo> list = new List<FriendInfo>();
			foreach (ProfileInfo pi in friends)
			{
				SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(pi.ProfileID);
				pi.Complete(profileInfo);
				ulong points = profileInfo.RankInfo.Points;
				FriendInfo item = new FriendInfo(pi, points);
				list.Add(item);
			}
			QueryManager.RequestSt("friend_list", receiver, new object[]
			{
				list
			});
		}

		// Token: 0x06001D6B RID: 7531 RVA: 0x0007745C File Offset: 0x0007585C
		private void OnFriendRemoveResolved(ProfileInfo removeTarget)
		{
			if (!string.IsNullOrEmpty(removeTarget.OnlineID))
			{
				this.SendFriendListUpdate(removeTarget.ProfileID, removeTarget.OnlineID);
			}
		}

		// Token: 0x06001D6C RID: 7532 RVA: 0x00077484 File Offset: 0x00075884
		private void OnFriendListChanged(ulong profileId1, ulong profileId2, bool friendAdded)
		{
			Log.Verbose("OnFriendListChanged : {0} {1}", new object[]
			{
				profileId1,
				profileId2
			});
			int num = (!friendAdded) ? -1 : 1;
			this.m_statsTracker.ChangeStatistics(profileId1, EStatsEvent.ADD_FRIEND, num);
			this.m_statsTracker.ChangeStatistics(profileId2, EStatsEvent.ADD_FRIEND, num);
		}

		// Token: 0x04000E00 RID: 3584
		private TimeSpan m_inviteExpiration;

		// Token: 0x04000E01 RID: 3585
		private uint m_friendsLimit;

		// Token: 0x04000E02 RID: 3586
		private readonly IDALService m_dalService;

		// Token: 0x04000E03 RID: 3587
		private readonly ISessionInfoService m_sessionInfoService;

		// Token: 0x04000E04 RID: 3588
		private readonly INotificationService m_notificationService;

		// Token: 0x04000E05 RID: 3589
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000E06 RID: 3590
		private readonly ILogService m_logService;

		// Token: 0x04000E07 RID: 3591
		private readonly IExternalNicknameSyncService m_externalNicknameSyncService;

		// Token: 0x04000E08 RID: 3592
		private readonly IProfileValidationService m_profileValidationService;

		// Token: 0x04000E09 RID: 3593
		private readonly IStatsTracker m_statsTracker;

		// Token: 0x04000E0A RID: 3594
		private readonly IClanService m_clanService;
	}
}
