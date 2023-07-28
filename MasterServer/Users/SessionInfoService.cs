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

namespace MasterServer.Users
{
	// Token: 0x020007F2 RID: 2034
	[Service]
	[Singleton]
	internal class SessionInfoService : ServiceModule, ISessionInfoService
	{
		// Token: 0x060029BC RID: 10684 RVA: 0x000B3BA1 File Offset: 0x000B1FA1
		public SessionInfoService(IUserRepository userRepository, IQueryManager queryManager, IOnlineClient onlineClient)
		{
			this.m_userRepository = userRepository;
			this.m_queryManager = queryManager;
			this.m_onlineClient = onlineClient;
		}

		// Token: 0x060029BD RID: 10685 RVA: 0x000B3BBE File Offset: 0x000B1FBE
		public override void Start()
		{
			this.m_userRepository.UserLoggedIn += this.OnUserLogin;
		}

		// Token: 0x060029BE RID: 10686 RVA: 0x000B3BD7 File Offset: 0x000B1FD7
		public override void Stop()
		{
			this.m_userRepository.UserLoggedIn -= this.OnUserLogin;
		}

		// Token: 0x060029BF RID: 10687 RVA: 0x000B3BF0 File Offset: 0x000B1FF0
		public SessionInfo GetSessionInfo(string online_id)
		{
			SessionInfo sessionInfo = null;
			try
			{
				Task<object> task = this.m_queryManager.RequestAsync("get_connection_info", this.m_onlineClient.TargetRoute, new object[]
				{
					online_id
				});
				task.Wait();
				sessionInfo = (SessionInfo)task.Result;
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
			finally
			{
				if (sessionInfo == null || string.IsNullOrEmpty(sessionInfo.IPAddress))
				{
					Log.Warning<string>("GetSessionInfo failed to resolve info for {0}", online_id);
					sessionInfo = new SessionInfo
					{
						IPAddress = "0.0.0.0",
						Tags = new UserTags(null)
					};
				}
			}
			return sessionInfo;
		}

		// Token: 0x060029C0 RID: 10688 RVA: 0x000B3CA8 File Offset: 0x000B20A8
		public async Task<SessionInfo> GetSessionInfoByOnlineIdAsync(string onlineId)
		{
			SessionInfo info = null;
			try
			{
				object result = await this.m_queryManager.RequestAsync("get_connection_info", this.m_onlineClient.TargetRoute, new object[]
				{
					onlineId
				});
				info = (SessionInfo)result;
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
			finally
			{
				if (info == null || string.IsNullOrEmpty(info.IPAddress))
				{
					Log.Warning<string>("GetSessionInfo failed to resolve info for {0}", onlineId);
					info = new SessionInfo
					{
						IPAddress = "0.0.0.0",
						Tags = new UserTags(null)
					};
				}
			}
			return info;
		}

		// Token: 0x060029C1 RID: 10689 RVA: 0x000B3CE8 File Offset: 0x000B20E8
		public async Task<SessionInfo> GetSessionInfoByOnlineIdAsync(string onlineId, int retries)
		{
			int count = 0;
			SessionInfo info = new SessionInfo();
			while (info.UserID == 0UL && count < retries)
			{
				info = await this.GetSessionInfoByOnlineIdAsync(onlineId);
				count++;
			}
			return info;
		}

		// Token: 0x060029C2 RID: 10690 RVA: 0x000B3D30 File Offset: 0x000B2130
		public SessionInfo GetSessionInfo(string online_id, int retries)
		{
			int num = 0;
			SessionInfo sessionInfo = new SessionInfo();
			while (sessionInfo.UserID == 0UL && num < retries)
			{
				sessionInfo = this.GetSessionInfo(online_id);
				num++;
			}
			return sessionInfo;
		}

		// Token: 0x060029C3 RID: 10691 RVA: 0x000B3D6C File Offset: 0x000B216C
		public ProfileInfo GetProfileInfo(string nickname)
		{
			IEnumerable<ProfileInfo> profileInfo = this.GetProfileInfo(new string[]
			{
				nickname
			});
			return profileInfo.FirstOrDefault<ProfileInfo>();
		}

		// Token: 0x060029C4 RID: 10692 RVA: 0x000B3D90 File Offset: 0x000B2190
		public IEnumerable<ProfileInfo> GetProfileInfo(IEnumerable<string> nicknames)
		{
			Task<List<ProfileInfo>> profileInfoAsync = this.GetProfileInfoAsync(nicknames);
			profileInfoAsync.Wait();
			return profileInfoAsync.Result;
		}

		// Token: 0x060029C5 RID: 10693 RVA: 0x000B3DB4 File Offset: 0x000B21B4
		public ProfileInfo GetProfileInfo(ulong profileId)
		{
			IEnumerable<ProfileInfo> profileInfo = this.GetProfileInfo(new ulong[]
			{
				profileId
			});
			return profileInfo.FirstOrDefault<ProfileInfo>();
		}

		// Token: 0x060029C6 RID: 10694 RVA: 0x000B3DD8 File Offset: 0x000B21D8
		public IEnumerable<ProfileInfo> GetProfileInfo(IEnumerable<ulong> profileIds)
		{
			Task<List<ProfileInfo>> profileInfoAsync = this.GetProfileInfoAsync(profileIds);
			profileInfoAsync.Wait();
			return profileInfoAsync.Result;
		}

		// Token: 0x060029C7 RID: 10695 RVA: 0x000B3DF9 File Offset: 0x000B21F9
		public void GetProfileInfo(IEnumerable<ulong> profileIds, ProfileInfoCallbackMulti clb)
		{
			this.GetProfileInfo(Enumerable.Empty<string>(), profileIds, clb);
		}

		// Token: 0x060029C8 RID: 10696 RVA: 0x000B3E08 File Offset: 0x000B2208
		public void GetProfileInfo(string nickname, ProfileInfoCallback clb)
		{
			this.GetProfileInfoAsync(nickname).ContinueWith(delegate(Task<ProfileInfo> t)
			{
				clb(t.Result);
			});
		}

		// Token: 0x060029C9 RID: 10697 RVA: 0x000B3E3C File Offset: 0x000B223C
		public void GetProfileInfo(IEnumerable<string> nicknames, ProfileInfoCallbackMulti clb)
		{
			this.GetProfileInfoAsync(nicknames).ContinueWith(delegate(Task<List<ProfileInfo>> t)
			{
				clb(t.Result);
			});
		}

		// Token: 0x060029CA RID: 10698 RVA: 0x000B3E6F File Offset: 0x000B226F
		public Task<ProfileInfo> GetProfileInfoAsync(string nickname)
		{
			return this.GetProfileInfoAsync(new string[]
			{
				nickname
			}).ContinueWith<ProfileInfo>((Task<List<ProfileInfo>> t) => t.Result.FirstOrDefault<ProfileInfo>());
		}

		// Token: 0x060029CB RID: 10699 RVA: 0x000B3EA3 File Offset: 0x000B22A3
		public Task<List<ProfileInfo>> GetProfileInfoAsync(IEnumerable<string> nicknames)
		{
			return this.GetProfileInfoAsync(nicknames, new ulong[0]);
		}

		// Token: 0x060029CC RID: 10700 RVA: 0x000B3EB2 File Offset: 0x000B22B2
		public Task<ProfileInfo> GetProfileInfoAsync(ulong profileId)
		{
			return this.GetProfileInfoAsync(new ulong[]
			{
				profileId
			}).ContinueWith<ProfileInfo>((Task<List<ProfileInfo>> t) => t.Result.FirstOrDefault<ProfileInfo>());
		}

		// Token: 0x060029CD RID: 10701 RVA: 0x000B3EE6 File Offset: 0x000B22E6
		public Task<List<ProfileInfo>> GetProfileInfoAsync(IEnumerable<ulong> profileIds)
		{
			return this.GetProfileInfoAsync(new string[0], profileIds);
		}

		// Token: 0x060029CE RID: 10702 RVA: 0x000B3EF8 File Offset: 0x000B22F8
		public void GetProfileInfo(IEnumerable<string> nicknames, IEnumerable<ulong> profileIds, ProfileInfoCallbackMulti clb)
		{
			this.GetProfileInfoAsync(nicknames, profileIds).ContinueWith(delegate(Task<List<ProfileInfo>> t)
			{
				clb(t.Result);
			});
		}

		// Token: 0x060029CF RID: 10703 RVA: 0x000B3F2C File Offset: 0x000B232C
		public Task<List<ProfileInfo>> GetProfileInfoAsync(IEnumerable<string> nicknames, IEnumerable<ulong> profileIds)
		{
			if (ServicesManager.ExecutionPhase < ExecutionPhase.Started)
			{
				throw new ProfileInfoStatusException(string.Format("Cannot resolve profile info on execution phase {0}", ServicesManager.ExecutionPhase));
			}
			List<NeutralProfileRef> copy = (from nick in nicknames
			select new NeutralProfileRef(nick)).ToList<NeutralProfileRef>();
			copy.AddRange(from pid in profileIds
			select new NeutralProfileRef(pid));
			StringBuilder stringBuilder = new StringBuilder();
			foreach (NeutralProfileRef neutralProfileRef in copy)
			{
				stringBuilder.AppendFormat("{0} ", neutralProfileRef.ToString());
			}
			Log.Verbose("Profile info: request for nicknames: {0}", new object[]
			{
				stringBuilder.ToString()
			});
			Task<object> task = this.m_queryManager.RequestAsync("profile_info_get_status", this.m_onlineClient.TargetRoute, new object[]
			{
				nicknames,
				profileIds
			});
			return task.ContinueWith<List<ProfileInfo>>(delegate(Task<object> t)
			{
				List<ProfileInfo> result;
				try
				{
					result = (List<ProfileInfo>)t.Result;
				}
				catch (AggregateException ex)
				{
					QueryException ex2 = ex.Flatten().InnerExceptions.OfType<QueryException>().FirstOrDefault<QueryException>();
					if (ex2 != null)
					{
						Log.Error<EOnlineError, string>("Failed to resolve profile info: {0} {1}", ex2.OnlineError, ex2.ErrorDescription);
					}
					else
					{
						Log.Error("Failed to resolve profile info");
					}
					Log.Error(ex);
					result = new List<ProfileInfo>();
				}
				SessionInfoService.FixResults(copy, result);
				return result;
			});
		}

		// Token: 0x060029D0 RID: 10704 RVA: 0x000B4078 File Offset: 0x000B2478
		private static void FixResults(IEnumerable<NeutralProfileRef> requested, List<ProfileInfo> result)
		{
			using (IEnumerator<NeutralProfileRef> enumerator = requested.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NeutralProfileRef elem = enumerator.Current;
					if (!Utils.Contains<ProfileInfo>(result, (ProfileInfo X) => elem.Check(X.Nickname, X.ProfileID)))
					{
						result.Add(new ProfileInfo(elem));
						Log.Verbose("Profile info: force set offline status for {0}", new object[]
						{
							elem.ToString()
						});
					}
				}
			}
		}

		// Token: 0x060029D1 RID: 10705 RVA: 0x000B4118 File Offset: 0x000B2518
		private void OnUserLogin(UserInfo.User user, ELoginType loginType)
		{
			this.UpdateProfileStatus(user);
		}

		// Token: 0x060029D2 RID: 10706 RVA: 0x000B4121 File Offset: 0x000B2521
		public void UpdateProfileStatus(UserInfo.User user)
		{
			this.UpdateProfileStatusAsync(user).ContinueWith(delegate(Task<object> t)
			{
				Log.Error(t.Exception);
			}, TaskContinuationOptions.OnlyOnFaulted);
		}

		// Token: 0x060029D3 RID: 10707 RVA: 0x000B4154 File Offset: 0x000B2554
		public Task<object> UpdateProfileStatusAsync(UserInfo.User user)
		{
			ProfileInfo profileInfo = new ProfileInfo
			{
				UserID = user.UserID,
				ProfileID = user.ProfileID,
				OnlineID = user.OnlineID,
				Nickname = user.Nickname,
				Status = UserStatus.Online,
				RankId = user.Rank,
				LoginTime = user.LoginTime
			};
			Log.Info<string>("Profile info: updating profile info of {0} on jabber", profileInfo.OnlineID);
			return this.m_queryManager.RequestAsync("profile_info_set_status", this.m_onlineClient.TargetRoute, new object[]
			{
				profileInfo
			});
		}

		// Token: 0x0400161E RID: 5662
		private readonly IUserRepository m_userRepository;

		// Token: 0x0400161F RID: 5663
		private readonly IQueryManager m_queryManager;

		// Token: 0x04001620 RID: 5664
		private readonly IOnlineClient m_onlineClient;
	}
}
