using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Timers;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.PunishmentSystem;
using MasterServer.GFaceAPI;
using MasterServer.GFaceAPI.Responses;
using MasterServer.Platform.Nickname;
using MasterServer.Platform.ProfanityCheck;
using MasterServer.Users;

namespace MasterServer.GFace.Services
{
	// Token: 0x0200052E RID: 1326
	[Service]
	[Singleton]
	internal class GFaceNicknameService : ServiceModule, INicknameProvider, INicknameReservationService, IExternalNicknameSyncService
	{
		// Token: 0x06001CCC RID: 7372 RVA: 0x00073BD0 File Offset: 0x00071FD0
		public GFaceNicknameService(IGFaceAPIService gfaceApiService, IUserRepository userRepsitory, IPunishmentService punishmentSerivce, ISessionInfoService sessionInfoService, IDALService dalService, IProfileValidationService profileValidationService, IProfanityCheckService profanityCheckService)
		{
			this.m_gfaceApiService = gfaceApiService;
			this.m_userRepository = userRepsitory;
			this.m_punishmentService = punishmentSerivce;
			this.m_dalService = dalService;
			this.m_sessionInfoService = sessionInfoService;
			this.m_profileValidationService = profileValidationService;
			this.m_profanityCheckService = profanityCheckService;
			this.m_random = new Random((int)DateTime.Now.Ticks);
		}

		// Token: 0x1400006D RID: 109
		// (add) Token: 0x06001CCD RID: 7373 RVA: 0x00073C70 File Offset: 0x00072070
		// (remove) Token: 0x06001CCE RID: 7374 RVA: 0x00073CA8 File Offset: 0x000720A8
		public event ProfileRenamedDelegate ProfileRenamed;

		// Token: 0x06001CCF RID: 7375 RVA: 0x00073CE0 File Offset: 0x000720E0
		public override void Init()
		{
			base.Init();
			ConfigSection section = Resources.GFaceSettings.GetSection("GFaceNicknameService");
			this.Config(section);
			section.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x06001CD0 RID: 7376 RVA: 0x00073D1C File Offset: 0x0007211C
		public override void Start()
		{
			base.Start();
			this.m_userRepository.UserLoggedIn += this.OnUserLoggedIn;
			if (this.m_broadcastUpdates)
			{
				this.m_timer = new SafeTimer(new TimerCallback(this.OnCleanUpTick), null, this.m_pendingRequestTimeout, this.m_pendingRequestTimeout);
			}
		}

		// Token: 0x06001CD1 RID: 7377 RVA: 0x00073D78 File Offset: 0x00072178
		public override void Stop()
		{
			this.m_userRepository.UserLoggedIn -= this.OnUserLoggedIn;
			ConfigSection section = Resources.GFaceSettings.GetSection("GFaceNicknameService");
			section.OnConfigChanged -= this.OnConfigChanged;
			base.Stop();
		}

		// Token: 0x06001CD2 RID: 7378 RVA: 0x00073DC4 File Offset: 0x000721C4
		private void OnConfigChanged(ConfigEventArgs args)
		{
			ConfigSection section = Resources.GFaceSettings.GetSection("GFaceNicknameService");
			this.Reconfig(section);
		}

		// Token: 0x06001CD3 RID: 7379 RVA: 0x00073DE8 File Offset: 0x000721E8
		private void Config(ConfigSection secNickname)
		{
			bool flag;
			secNickname.Get("no_update_broadcasting", out flag);
			TimeSpan pendingRequestTimeout;
			if (!secNickname.TryGet("pending_request_timeout_ms", out pendingRequestTimeout, default(TimeSpan)))
			{
				throw new ApplicationException("Failed to get the value of config `gface.nickname.pending_request_timeout_ms`");
			}
			this.m_broadcastUpdates = !flag;
			object lockReqQueue = this.m_lockReqQueue;
			lock (lockReqQueue)
			{
				this.m_pendingRequestTimeout = pendingRequestTimeout;
			}
		}

		// Token: 0x06001CD4 RID: 7380 RVA: 0x00073E6C File Offset: 0x0007226C
		private void Reconfig(ConfigSection secNickname)
		{
			object lockTimer = this.m_lockTimer;
			lock (lockTimer)
			{
				this.Config(secNickname);
				if (!this.m_broadcastUpdates)
				{
					if (this.m_timer != null)
					{
						this.m_timer.Dispose();
						object lockReqQueue = this.m_lockReqQueue;
						lock (lockReqQueue)
						{
							this.m_pendingUpdateRequests.Clear();
						}
					}
					this.m_timer = null;
				}
				else if (this.m_timer == null)
				{
					this.m_timer = new SafeTimer(new TimerCallback(this.OnCleanUpTick), null, this.m_pendingRequestTimeout, this.m_pendingRequestTimeout);
				}
				else
				{
					this.m_timer.Change(this.m_pendingRequestTimeout, this.m_pendingRequestTimeout);
				}
			}
		}

		// Token: 0x06001CD5 RID: 7381 RVA: 0x00073F60 File Offset: 0x00072360
		private void OnUserLoggedIn(UserInfo.User user, ELoginType loginType)
		{
			if (!this.m_broadcastUpdates)
			{
				return;
			}
			ulong profileId = user.ProfileID;
			bool flag = false;
			object lockReqQueue = this.m_lockReqQueue;
			lock (lockReqQueue)
			{
				flag = this.m_pendingUpdateRequests.Remove(profileId);
			}
			if (flag)
			{
				this.m_sessionInfoService.UpdateProfileStatusAsync(user).ContinueWith(delegate(Task<object> t)
				{
					try
					{
						t.Wait();
					}
					catch (Exception e)
					{
						Log.Error(e);
						return;
					}
					this.InvokeProfileRenamedEvent(profileId);
				});
			}
		}

		// Token: 0x06001CD6 RID: 7382 RVA: 0x00073FFC File Offset: 0x000723FC
		private void OnCleanUpTick(object _)
		{
			object lockTimer = this.m_lockTimer;
			lock (lockTimer)
			{
				if (this.m_timer == null)
				{
					return;
				}
			}
			DateTime now = DateTime.Now;
			object lockReqQueue = this.m_lockReqQueue;
			lock (lockReqQueue)
			{
				KeyValuePair<ulong, DateTime>[] array = (from kv in this.m_pendingUpdateRequests
				where now - kv.Value > this.m_pendingRequestTimeout
				select kv).ToArray<KeyValuePair<ulong, DateTime>>();
				foreach (KeyValuePair<ulong, DateTime> keyValuePair in array)
				{
					this.m_pendingUpdateRequests.Remove(keyValuePair.Key);
				}
			}
		}

		// Token: 0x06001CD7 RID: 7383 RVA: 0x000740F0 File Offset: 0x000724F0
		public string GetNickname(ulong userId, ulong profileId)
		{
			SingleUserSeed singleUserSeed = this.m_gfaceApiService.Request<SingleUserSeed>(CallOptions.Reliable, GFaceAPIs.user_get, new object[]
			{
				"token",
				ServerTokenPlaceHolder.Instance,
				"userid",
				this.m_userRepository.UnmangleUserId(userId)
			});
			if (this.m_profileValidationService.ValidateNickname(singleUserSeed.seed.user.nickname) != NameValidationResult.NoError)
			{
				throw new NicknameProviderException(string.Format("Invalid nickname detected when user joining channel: user {0}", userId));
			}
			if (this.m_profanityCheckService.CheckProfileName(userId, singleUserSeed.seed.user.nickname) != ProfanityCheckResult.Succeeded)
			{
				throw new NicknameProviderException(string.Format("Nickname containing profanity word detected when user joining channel: user {0}", userId));
			}
			return singleUserSeed.seed.user.nickname;
		}

		// Token: 0x06001CD8 RID: 7384 RVA: 0x000741C4 File Offset: 0x000725C4
		public bool SyncNickname(ulong profileId, string nickname)
		{
			if (profileId != 0UL)
			{
				if (this.m_dalService.ProfileSystem.GetProfileInfo(profileId).Nickname != nickname)
				{
					if (!this.OverwriteDBProfileNickname(profileId, nickname))
					{
						return false;
					}
					if (this.m_broadcastUpdates)
					{
						object lockReqQueue = this.m_lockReqQueue;
						lock (lockReqQueue)
						{
							this.m_pendingUpdateRequests.Add(profileId, DateTime.Now);
						}
					}
				}
				return true;
			}
			return this.ResolveCollision(nickname);
		}

		// Token: 0x06001CD9 RID: 7385 RVA: 0x00074268 File Offset: 0x00072668
		public NameReservationResult ReserveNickname(ulong userId, string nickname)
		{
			return NameReservationResult.OK;
		}

		// Token: 0x06001CDA RID: 7386 RVA: 0x0007426B File Offset: 0x0007266B
		public NameReservationResult CancelNicknameReservation(ulong userId, string nickname)
		{
			return NameReservationResult.OK;
		}

		// Token: 0x06001CDB RID: 7387 RVA: 0x0007426E File Offset: 0x0007266E
		public NameReservationResult GetUserIdByReservedNickname(string nickname, out ulong userId)
		{
			userId = 0UL;
			return NameReservationResult.OK;
		}

		// Token: 0x06001CDC RID: 7388 RVA: 0x00074275 File Offset: 0x00072675
		private bool UpdateProfileNicknameInDB(ulong profileId, string newNickname)
		{
			if (!this.m_dalService.ProfileSystem.UpdateProfileNickname(profileId, newNickname))
			{
				return false;
			}
			this.m_dalService.ClanSystem.FlushClanCacheForMember(profileId);
			this.m_dalService.ProfileSystem.FlushProfileFriendsCache(profileId);
			return true;
		}

		// Token: 0x06001CDD RID: 7389 RVA: 0x000742B3 File Offset: 0x000726B3
		private bool OverwriteDBProfileNickname(ulong profileId, string newNickname)
		{
			if (this.UpdateProfileNicknameInDB(profileId, newNickname))
			{
				return true;
			}
			this.ResolveCollision(newNickname);
			return this.UpdateProfileNicknameInDB(profileId, newNickname);
		}

		// Token: 0x06001CDE RID: 7390 RVA: 0x000742D4 File Offset: 0x000726D4
		private bool ResolveCollision(string newNickname)
		{
			SProfileInfo conflictProfile = this.m_dalService.ProfileSystem.GetProfileByNickname(newNickname);
			if (conflictProfile.Id == 0UL)
			{
				return true;
			}
			if (this.m_userRepository.IsBootstrap(conflictProfile.UserID, 1UL))
			{
				this.m_punishmentService.ForceLogout(conflictProfile.Id);
				Utils.TryFor(5, () => this.UpdateProfileNicknameInDB(conflictProfile.Id, this.GenerateDummyNickname()));
				return true;
			}
			return false;
		}

		// Token: 0x06001CDF RID: 7391 RVA: 0x00074364 File Offset: 0x00072764
		private void InvokeProfileRenamedEvent(ulong profileId)
		{
			if (this.ProfileRenamed == null)
			{
				return;
			}
			this.ProfileRenamed.GetInvocationList().SafeForEach(delegate(Delegate func)
			{
				((ProfileRenamedDelegate)func)(profileId);
			});
		}

		// Token: 0x06001CE0 RID: 7392 RVA: 0x000743A8 File Offset: 0x000727A8
		private string GenerateDummyNickname()
		{
			return "$" + this.m_random.Next().ToString("X8");
		}

		// Token: 0x06001CE1 RID: 7393 RVA: 0x000743D7 File Offset: 0x000727D7
		private NicknameUnqalifiedReason TranslateReason(NameValidationResult result)
		{
			switch (result)
			{
			case NameValidationResult.NoError:
				return NicknameUnqalifiedReason.NoError;
			case NameValidationResult.LengthTooShort:
				return NicknameUnqalifiedReason.TooShort;
			case NameValidationResult.LengthTooLong:
				return NicknameUnqalifiedReason.TooLong;
			case NameValidationResult.UnsupportedCharacter:
				return NicknameUnqalifiedReason.UnsupportedCharacter;
			default:
				return NicknameUnqalifiedReason.Undefined;
			}
		}

		// Token: 0x04000DB8 RID: 3512
		private readonly IGFaceAPIService m_gfaceApiService;

		// Token: 0x04000DB9 RID: 3513
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000DBA RID: 3514
		private readonly IPunishmentService m_punishmentService;

		// Token: 0x04000DBB RID: 3515
		private readonly ISessionInfoService m_sessionInfoService;

		// Token: 0x04000DBC RID: 3516
		private readonly IDALService m_dalService;

		// Token: 0x04000DBD RID: 3517
		private readonly IProfileValidationService m_profileValidationService;

		// Token: 0x04000DBE RID: 3518
		private readonly IProfanityCheckService m_profanityCheckService;

		// Token: 0x04000DBF RID: 3519
		private readonly Random m_random;

		// Token: 0x04000DC0 RID: 3520
		private Dictionary<ulong, DateTime> m_pendingUpdateRequests = new Dictionary<ulong, DateTime>();

		// Token: 0x04000DC1 RID: 3521
		private object m_lockReqQueue = new object();

		// Token: 0x04000DC2 RID: 3522
		private object m_lockTimer = new object();

		// Token: 0x04000DC3 RID: 3523
		private SafeTimer m_timer;

		// Token: 0x04000DC4 RID: 3524
		private bool m_broadcastUpdates = true;

		// Token: 0x04000DC5 RID: 3525
		private TimeSpan m_pendingRequestTimeout = TimeSpan.FromSeconds(30.0);
	}
}
