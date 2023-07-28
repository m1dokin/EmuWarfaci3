using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.GameInterface;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.GameLogic.PunishmentSystem
{
	// Token: 0x0200058D RID: 1421
	[Service]
	[Singleton]
	internal class PunishmentService : ServiceModule, IPunishmentService
	{
		// Token: 0x06001E8A RID: 7818 RVA: 0x0007BBC8 File Offset: 0x00079FC8
		public PunishmentService(IUserRepository userRepository, IDALService dalService, IQueryManager queryManager, IOnlineClient onlineClient, ILogService logService, IGameRoomManager gameRoomManager, IAuthService authService, ITimeSource timeSource)
		{
			this.m_userRepository = userRepository;
			this.m_dalService = dalService;
			this.m_queryManager = queryManager;
			this.m_onlineClient = onlineClient;
			this.m_logService = logService;
			this.m_gameRoomManager = gameRoomManager;
			this.m_authService = authService;
			this.m_timeSource = timeSource;
			ConfigSection section = Resources.ModuleSettings.GetSection("Punishment");
			int cacheTimeoutSec = int.Parse(section.Get("MakeScreenShootTimeOut"));
			this.m_screenshotTasks = new CacheDictionary<long, TaskCompletionSource<string>>(cacheTimeoutSec, CacheDictionaryMode.Expiration);
			this.m_screenshotTasks.ItemExpired += this.ScreenshotTaskTimeouted;
		}

		// Token: 0x14000074 RID: 116
		// (add) Token: 0x06001E8B RID: 7819 RVA: 0x0007BC60 File Offset: 0x0007A060
		// (remove) Token: 0x06001E8C RID: 7820 RVA: 0x0007BC98 File Offset: 0x0007A098
		public event Action<ulong, DateTime, string> PlayerBanned;

		// Token: 0x14000075 RID: 117
		// (add) Token: 0x06001E8D RID: 7821 RVA: 0x0007BCD0 File Offset: 0x0007A0D0
		// (remove) Token: 0x06001E8E RID: 7822 RVA: 0x0007BD08 File Offset: 0x0007A108
		public event Action<ulong> PlayerUnBanned;

		// Token: 0x06001E8F RID: 7823 RVA: 0x0007BD3E File Offset: 0x0007A13E
		public override void Init()
		{
			this.m_userRepository.UserLoggedIn += this.OnUserLoggedIn;
		}

		// Token: 0x06001E90 RID: 7824 RVA: 0x0007BD57 File Offset: 0x0007A157
		public override void Stop()
		{
			base.Stop();
			this.m_userRepository.UserLoggedIn -= this.OnUserLoggedIn;
			this.m_screenshotTasks.Dispose();
		}

		// Token: 0x06001E91 RID: 7825 RVA: 0x0007BD84 File Offset: 0x0007A184
		public bool IsBanned(IProfileProxy profile)
		{
			return profile.ProfileInfo.BanTime > this.m_timeSource.Now();
		}

		// Token: 0x06001E92 RID: 7826 RVA: 0x0007BDB0 File Offset: 0x0007A1B0
		public void BanPlayer(ulong profileId, TimeSpan time, string message, BanReportSource source)
		{
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
			if (profileInfo.Id != profileId)
			{
				Log.Info<ulong>("Profile with {0} id doesn't exist", profileId);
				return;
			}
			if (time == TimeSpan.Zero)
			{
				Log.Info<ulong>("There is attempt to ban profile {0} for 0 seconds", profileId);
				return;
			}
			DateTime dateTime = this.UpdateBanTime(profileId, time);
			this.m_queryManager.Request("force_close_connection", this.m_onlineClient.TargetRoute, new object[]
			{
				profileInfo.Nickname
			});
			this.m_logService.Event.CharacterBanLog(profileInfo.UserID, profileId, dateTime, source);
			this.RaisePlayerBannedEvent(profileInfo.UserID, dateTime, message);
		}

		// Token: 0x06001E93 RID: 7827 RVA: 0x0007BE64 File Offset: 0x0007A264
		public void UnBanPlayer(ulong profileId)
		{
			this.UpdateBanTime(profileId, TimeSpan.Zero);
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
			if (profileInfo.Id != profileId)
			{
				Log.Info<ulong>("Profile with {0} id doesn't exist", profileId);
				return;
			}
			this.m_logService.Event.CharacterUnBanLog(profileInfo.UserID, profileId);
			this.RaisePlayerUnBannedEvent(profileInfo.UserID);
		}

		// Token: 0x06001E94 RID: 7828 RVA: 0x0007BED0 File Offset: 0x0007A2D0
		public void MutePlayer(ulong profileId, TimeSpan time)
		{
			DateTime dateTime = this.m_timeSource.Now() + time;
			this.m_dalService.ProfileSystem.UpdateMuteTime(profileId, dateTime);
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
			if (profileInfo.Id != profileId)
			{
				Log.Info<ulong>("Profile with {0} id doesn't exist", profileId);
				return;
			}
			this.m_queryManager.Request("mute_user", this.m_onlineClient.TargetRoute, new object[]
			{
				profileInfo.Nickname,
				dateTime
			});
			this.m_logService.Event.CharacterMuteLog(profileInfo.UserID, profileId, dateTime);
		}

		// Token: 0x06001E95 RID: 7829 RVA: 0x0007BF7C File Offset: 0x0007A37C
		public void UnMute(ulong profileId)
		{
			DateTime dateTime = this.m_timeSource.Now();
			this.m_dalService.ProfileSystem.UpdateMuteTime(profileId, dateTime);
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
			if (profileInfo.Id != profileId)
			{
				Log.Info<ulong>("Profile with {0} id doesn't exist", profileId);
				return;
			}
			this.m_queryManager.Request("mute_user", this.m_onlineClient.TargetRoute, new object[]
			{
				profileInfo.Nickname,
				dateTime
			});
			this.m_logService.Event.CharacterUnMuteLog(profileInfo.UserID, profileId);
		}

		// Token: 0x06001E96 RID: 7830 RVA: 0x0007C020 File Offset: 0x0007A420
		public void KickPlayer(ulong profileId)
		{
			string text = string.Format("kick {0}", profileId);
			if (this.CanKick(profileId))
			{
				this.m_queryManager.Request("master_server_bcast", this.m_onlineClient.TargetRoute, new object[]
				{
					"kick",
					text
				});
			}
		}

		// Token: 0x06001E97 RID: 7831 RVA: 0x0007C078 File Offset: 0x0007A478
		public bool KickPlayerLocal(ulong profileId, GameRoomPlayerRemoveReason reason)
		{
			IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(profileId);
			if (roomByPlayer == null || !this.CanKick(profileId))
			{
				return false;
			}
			roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				KickExtension extension = r.GetExtension<KickExtension>();
				extension.KickPlayer(profileId, reason);
			});
			return true;
		}

		// Token: 0x06001E98 RID: 7832 RVA: 0x0007C0D8 File Offset: 0x0007A4D8
		public void ForceLogout(ulong profileId)
		{
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
			if (profileInfo.Empty)
			{
				Log.Error<ulong>("Trying to disconnect unexisting profile {0}", profileId);
				return;
			}
			this.m_queryManager.Request("force_close_connection", this.m_onlineClient.TargetRoute, new object[]
			{
				profileInfo.Nickname
			});
		}

		// Token: 0x06001E99 RID: 7833 RVA: 0x0007C13A File Offset: 0x0007A53A
		public string MakeScreenShot(ulong profileId, bool frontBuffer, int count, float scaleWidth, float scaleHeight)
		{
			return this.MakeScreenShot(profileId, frontBuffer, count, scaleWidth, scaleHeight, this.m_screenshotKey, Resources.Jid);
		}

		// Token: 0x06001E9A RID: 7834 RVA: 0x0007C154 File Offset: 0x0007A554
		public string MakeScreenShot(ulong profileId, bool frontBuffer, int count, float scaleWidth, float scaleHeight, long screenshotId, string initiator)
		{
			bool flag = this.m_userRepository.GetUser(profileId) != null;
			if (flag)
			{
				IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(profileId);
				string serverOnlineId = string.Empty;
				if (roomByPlayer == null)
				{
					string text = "Player is not connected to room";
					if (Resources.Jid == initiator)
					{
						return text;
					}
					this.m_queryManager.Request("remote_screenshot_result", initiator, new object[]
					{
						screenshotId,
						text
					});
					return string.Empty;
				}
				else
				{
					roomByPlayer.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
					{
						ServerExtension extension = r.GetExtension<ServerExtension>();
						serverOnlineId = extension.ServerOnlineID;
					});
					if (string.IsNullOrEmpty(serverOnlineId))
					{
						string text2 = "Player is not connected to dedicated server";
						if (Resources.Jid == initiator)
						{
							return text2;
						}
						this.m_queryManager.Request("remote_screenshot_result", initiator, new object[]
						{
							screenshotId,
							text2
						});
						return string.Empty;
					}
					else
					{
						if (Resources.Jid == initiator)
						{
							Task<string> task = this.AddScreenshotTask();
							this.m_queryManager.Request("remote_screenshot", serverOnlineId, new object[]
							{
								profileId,
								frontBuffer,
								count,
								scaleWidth,
								scaleHeight,
								task.AsyncState,
								initiator
							});
							task.Wait();
							return task.Result;
						}
						this.m_queryManager.Request("remote_screenshot", serverOnlineId, new object[]
						{
							profileId,
							frontBuffer,
							count,
							scaleWidth,
							scaleHeight,
							screenshotId,
							initiator
						});
					}
				}
			}
			else if (Resources.Jid == initiator)
			{
				Task<string> task2 = this.AddScreenshotTask();
				this.m_queryManager.Request("master_server_bcast", this.m_onlineClient.TargetRoute, new object[]
				{
					"remote_screenshot",
					profileId,
					frontBuffer,
					count,
					scaleWidth,
					scaleHeight,
					task2.AsyncState,
					"no_self_send"
				});
				task2.Wait();
				return task2.Result;
			}
			return string.Empty;
		}

		// Token: 0x06001E9B RID: 7835 RVA: 0x0007C3C8 File Offset: 0x0007A7C8
		public void OnScreenShotResult(long screenshotId, string result)
		{
			TaskCompletionSource<string> taskCompletionSource;
			if (this.m_screenshotTasks.Pop(screenshotId, out taskCompletionSource))
			{
				taskCompletionSource.SetResult(result);
			}
		}

		// Token: 0x06001E9C RID: 7836 RVA: 0x0007C3F0 File Offset: 0x0007A7F0
		public bool CanKick(ulong profileId)
		{
			UserInfo.User user = this.m_userRepository.GetUser(profileId);
			AccessLevel accessLevel = (user == null) ? AccessLevel.Basic : user.AccessLvl;
			if (user == null)
			{
				SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
				List<SUserAccessLevel> accessLevel2 = this.m_authService.GetAccessLevel(profileInfo.UserID);
				accessLevel = SUserAccessLevel.GetUserAccessLevel(profileInfo.UserID, "0.0.0.0", accessLevel2).accessLevel;
			}
			return accessLevel == AccessLevel.Basic;
		}

		// Token: 0x06001E9D RID: 7837 RVA: 0x0007C468 File Offset: 0x0007A868
		private Task<string> AddScreenshotTask()
		{
			long num = Interlocked.Increment(ref this.m_screenshotKey);
			TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>(num);
			this.m_screenshotTasks.Add(num, taskCompletionSource);
			return taskCompletionSource.Task;
		}

		// Token: 0x06001E9E RID: 7838 RVA: 0x0007C4A4 File Offset: 0x0007A8A4
		private DateTime UpdateBanTime(ulong profileId, TimeSpan time)
		{
			DateTime dateTime = this.m_timeSource.Now().SafeAdd(time);
			this.m_dalService.ProfileSystem.UpdateBanTime(profileId, dateTime);
			return dateTime;
		}

		// Token: 0x06001E9F RID: 7839 RVA: 0x0007C4D6 File Offset: 0x0007A8D6
		private void ScreenshotTaskTimeouted(long key, TaskCompletionSource<string> tcs)
		{
			tcs.SetResult("Screenshot failed by timeout");
		}

		// Token: 0x06001EA0 RID: 7840 RVA: 0x0007C4E3 File Offset: 0x0007A8E3
		private void RaisePlayerBannedEvent(ulong userId, DateTime expiresOn, string message)
		{
			this.PlayerBanned.SafeInvokeEach(userId, expiresOn, message);
		}

		// Token: 0x06001EA1 RID: 7841 RVA: 0x0007C4F3 File Offset: 0x0007A8F3
		private void RaisePlayerUnBannedEvent(ulong userId)
		{
			this.PlayerUnBanned.SafeInvokeEach(userId);
		}

		// Token: 0x06001EA2 RID: 7842 RVA: 0x0007C504 File Offset: 0x0007A904
		private void OnUserLoggedIn(UserInfo.User user, ELoginType loginType)
		{
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(user.ProfileID);
			if (profileInfo.MuteTime > DateTime.Now)
			{
				this.m_queryManager.Request("mute_user", this.m_onlineClient.TargetRoute, new object[]
				{
					user.Nickname,
					profileInfo.MuteTime
				});
			}
		}

		// Token: 0x04000EDD RID: 3805
		private readonly IAuthService m_authService;

		// Token: 0x04000EDE RID: 3806
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000EDF RID: 3807
		private readonly IDALService m_dalService;

		// Token: 0x04000EE0 RID: 3808
		private readonly IQueryManager m_queryManager;

		// Token: 0x04000EE1 RID: 3809
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x04000EE2 RID: 3810
		private readonly ILogService m_logService;

		// Token: 0x04000EE3 RID: 3811
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000EE4 RID: 3812
		private readonly ITimeSource m_timeSource;

		// Token: 0x04000EE5 RID: 3813
		private readonly CacheDictionary<long, TaskCompletionSource<string>> m_screenshotTasks;

		// Token: 0x04000EE6 RID: 3814
		private long m_screenshotKey;
	}
}
