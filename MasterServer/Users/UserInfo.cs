using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Timers;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.GameLogic.GameInterface;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.Users
{
	// Token: 0x0200075E RID: 1886
	[Service(Name = "UserRepository")]
	[Singleton]
	internal class UserInfo : ServiceModule, IUserRepository
	{
		// Token: 0x060026F3 RID: 9971 RVA: 0x000A48DC File Offset: 0x000A2CDC
		public UserInfo(IUserFactory userFactory, IOnlineClient onlineClient, ITimerFactory timerFactory)
		{
			this.m_userFactory = userFactory;
			this.m_onlineClient = onlineClient;
			this.m_timerFactory = timerFactory;
		}

		// Token: 0x140000A9 RID: 169
		// (add) Token: 0x060026F4 RID: 9972 RVA: 0x000A49C8 File Offset: 0x000A2DC8
		// (remove) Token: 0x060026F5 RID: 9973 RVA: 0x000A4A00 File Offset: 0x000A2E00
		public event UserLoggingInDeleg UserLoggingIn = delegate(UserInfo.User A_0, ELoginType A_1, DateTime A_2)
		{
		};

		// Token: 0x140000AA RID: 170
		// (add) Token: 0x060026F6 RID: 9974 RVA: 0x000A4A38 File Offset: 0x000A2E38
		// (remove) Token: 0x060026F7 RID: 9975 RVA: 0x000A4A70 File Offset: 0x000A2E70
		public event UserLoginDeleg UserLoggedIn = delegate(UserInfo.User A_0, ELoginType A_1)
		{
		};

		// Token: 0x140000AB RID: 171
		// (add) Token: 0x060026F8 RID: 9976 RVA: 0x000A4AA8 File Offset: 0x000A2EA8
		// (remove) Token: 0x060026F9 RID: 9977 RVA: 0x000A4AE0 File Offset: 0x000A2EE0
		public event UserLogoutDeleg UserLoggedOut = delegate(UserInfo.User A_0, ELogoutType A_1)
		{
		};

		// Token: 0x140000AC RID: 172
		// (add) Token: 0x060026FA RID: 9978 RVA: 0x000A4B18 File Offset: 0x000A2F18
		// (remove) Token: 0x060026FB RID: 9979 RVA: 0x000A4B50 File Offset: 0x000A2F50
		public event UserInfoChangedDeleg UserInfoChanged = delegate(UserInfo.User A_0, UserInfo.User A_1)
		{
		};

		// Token: 0x060026FC RID: 9980 RVA: 0x000A4B88 File Offset: 0x000A2F88
		public override void Start()
		{
			base.Start();
			if (!Resources.XMPPSettings.TryGet("user_drop_timeout_sec", out this.m_userDropTimeout, default(TimeSpan)))
			{
				this.m_userDropTimeout = TimeSpan.Zero;
			}
			this.m_onlineClient.ConnectionStateChanged += this.OnConnectionStateChanged;
			ConfigSection section = Resources.QoSSettings.GetSection("limits");
			section.Get("max_online_users", out this.m_maxOnlineUsers);
			if (section.HasValue("max_join_users"))
			{
				section.Get("max_join_users", out this.m_maxJoinUsers);
			}
			else
			{
				this.m_maxJoinUsers = this.m_maxOnlineUsers;
			}
			section.OnConfigChanged += this.OnConfigChanged;
			Log.Info<int, int>("Max online on the channel is {0}/{1}", this.m_maxOnlineUsers, this.m_maxJoinUsers);
		}

		// Token: 0x060026FD RID: 9981 RVA: 0x000A4C5B File Offset: 0x000A305B
		public override void Stop()
		{
			this.m_onlineClient.ConnectionStateChanged -= this.OnConnectionStateChanged;
			this.UserLoggingIn = null;
			this.UserLoggedIn = null;
			this.UserLoggedOut = null;
			this.UserInfoChanged = null;
			base.Stop();
		}

		// Token: 0x060026FE RID: 9982 RVA: 0x000A4C98 File Offset: 0x000A3098
		public UserInfo.User Make(ulong uid, ulong pid, string nick, string jid, DateTime loginTime, ProfileProgressionInfo profileProgression, string token = "", string ip = "", string buildType = "", string regionId = "", AccessLevel accessLevel = AccessLevel.Basic, ulong exp = 0UL, int rank = 0, int ping = 0, SBannerInfo banner = default(SBannerInfo), ClientVersion version = default(ClientVersion))
		{
			return this.m_userFactory.Create(uid, pid, nick, jid, token, ip, buildType, regionId, accessLevel, loginTime, exp, rank, ping, banner, profileProgression, version);
		}

		// Token: 0x060026FF RID: 9983 RVA: 0x000A4CD0 File Offset: 0x000A30D0
		public UserInfo.User Make(ulong pid, string jid, string token, string ip, string buildType, string regionId, SProfileInfo profileInfo, AccessLevel accessLevel, DateTime loginTime, ProfileProgressionInfo profileProgression, ClientVersion version)
		{
			return this.m_userFactory.Create(pid, jid, token, ip, buildType, regionId, profileInfo, accessLevel, loginTime, profileProgression, version);
		}

		// Token: 0x06002700 RID: 9984 RVA: 0x000A4CFC File Offset: 0x000A30FC
		public UserInfo.User Make(SProfileInfo profileInfo, string onlineId, ProfileProgressionInfo profileProgression)
		{
			return this.m_userFactory.Create(profileInfo.Id, onlineId, string.Empty, "0.0.0.0", string.Empty, string.Empty, profileInfo, AccessLevel.Basic, DateTime.UtcNow, profileProgression, ClientVersion.Unknown);
		}

		// Token: 0x06002701 RID: 9985 RVA: 0x000A4D3D File Offset: 0x000A313D
		public UserInfo.User Make(SProfileInfo profileInfo, ProfileProgressionInfo profileProgression)
		{
			return this.Make(profileInfo, string.Empty, profileProgression);
		}

		// Token: 0x06002702 RID: 9986 RVA: 0x000A4D4C File Offset: 0x000A314C
		public UserInfo.User Make(ProfileInfo profileInfo, SProfileInfo sProfileInfo, ProfileProgressionInfo profileProgression)
		{
			return this.m_userFactory.Create(profileInfo.UserID, profileInfo.ProfileID, profileInfo.Nickname, profileInfo.OnlineID, string.Empty, profileInfo.IPAddress, string.Empty, string.Empty, AccessLevel.Basic, profileInfo.LoginTime, 0UL, sProfileInfo.RankInfo.RankId, 0, sProfileInfo.Banner, profileProgression, ClientVersion.Unknown);
		}

		// Token: 0x06002703 RID: 9987 RVA: 0x000A4DBC File Offset: 0x000A31BC
		public UserInfo.User MakeFake(ulong uid, ulong pid, string jid, int rank = 0, ulong experience = 0UL, string regionId = "")
		{
			ulong uid2 = uid;
			string nick = uid.ToString(CultureInfo.InvariantCulture);
			DateTime utcNow = DateTime.UtcNow;
			ProfileProgressionInfo profileProgression = new ProfileProgressionInfo(0UL, null);
			return this.Make(uid2, pid, nick, jid, utcNow, profileProgression, string.Empty, string.Empty, string.Empty, regionId, AccessLevel.Basic, experience, rank, 0, default(SBannerInfo), default(ClientVersion));
		}

		// Token: 0x06002704 RID: 9988 RVA: 0x000A4E34 File Offset: 0x000A3234
		public void UserPreLogin(UserInfo.User user, ELoginType loginType, DateTime lastSeen)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_users[user.ProfileID] = user;
				this.m_users_by_nick[user.Nickname] = user;
				this.m_users_by_online_id[user.OnlineID] = user;
				this.m_users_by_user_id[user.UserID] = user;
			}
			Log.Info<ulong, string, ulong>("User {0} LOGGING_IN, jid {1}, user {2}", user.ProfileID, user.OnlineID, user.UserID);
			if (this.UserLoggingIn != null)
			{
				this.UserLoggingIn(user, loginType, lastSeen);
			}
		}

		// Token: 0x06002705 RID: 9989 RVA: 0x000A4EF0 File Offset: 0x000A32F0
		public bool UserLogin(UserInfo.User user, ELoginType loginType)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (!this.m_users.ContainsKey(user.ProfileID))
				{
					Log.Warning<string>("User {0} disconnected between prelogin and login", user.OnlineID);
					return false;
				}
			}
			Log.Info<ulong, string, ulong>("User {0} LOGGED_IN, jid {1}, user {2}", user.ProfileID, user.OnlineID, user.UserID);
			this.RaiseLoggedInEvent(user, loginType);
			return true;
		}

		// Token: 0x06002706 RID: 9990 RVA: 0x000A4F84 File Offset: 0x000A3384
		public void UserLogout(UserInfo.User user, ELogoutType logoutType)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_users.ContainsKey(user.ProfileID))
				{
					this.m_users.Remove(user.ProfileID);
					this.m_users_by_nick.Remove(user.Nickname);
					this.m_users_by_online_id.Remove(user.OnlineID);
					this.m_users_by_user_id.Remove(user.UserID);
				}
				else
				{
					Log.Warning<string>("User {0} is logged out more than one time", user.OnlineID);
				}
			}
			Log.Info<ulong, string, ulong>("User {0} LOGGED_OUT, jid {1}, user {2}", user.ProfileID, user.OnlineID, user.UserID);
			this.RaiseLogoutEvent(user, logoutType);
		}

		// Token: 0x06002707 RID: 9991 RVA: 0x000A5058 File Offset: 0x000A3458
		public UserInfo.User GetUser(ulong profile_id)
		{
			object @lock = this.m_lock;
			UserInfo.User result;
			lock (@lock)
			{
				UserInfo.User user;
				if (this.m_users.TryGetValue(profile_id, out user))
				{
					user.Touch();
				}
				result = user;
			}
			return result;
		}

		// Token: 0x06002708 RID: 9992 RVA: 0x000A50B4 File Offset: 0x000A34B4
		public UserInfo.User GetUser(string nickname)
		{
			object @lock = this.m_lock;
			UserInfo.User result;
			lock (@lock)
			{
				UserInfo.User user;
				if (this.m_users_by_nick.TryGetValue(nickname, out user))
				{
					user.Touch();
				}
				result = user;
			}
			return result;
		}

		// Token: 0x06002709 RID: 9993 RVA: 0x000A5110 File Offset: 0x000A3510
		public UserInfo.User GetUserByOnlineId(string onlineId)
		{
			object @lock = this.m_lock;
			UserInfo.User result;
			lock (@lock)
			{
				UserInfo.User user;
				if (this.m_users_by_online_id.TryGetValue(onlineId, out user))
				{
					user.Touch();
				}
				result = user;
			}
			return result;
		}

		// Token: 0x0600270A RID: 9994 RVA: 0x000A516C File Offset: 0x000A356C
		public UserInfo.User GetUserByUserId(ulong userId)
		{
			object @lock = this.m_lock;
			UserInfo.User result;
			lock (@lock)
			{
				UserInfo.User user;
				if (this.m_users_by_user_id.TryGetValue(userId, out user))
				{
					user.Touch();
				}
				result = user;
			}
			return result;
		}

		// Token: 0x0600270B RID: 9995 RVA: 0x000A51C8 File Offset: 0x000A35C8
		public List<UserInfo.User> GetUsersWithoutTouch()
		{
			return this.GetUsersWithoutTouch((UserInfo.User _) => true);
		}

		// Token: 0x0600270C RID: 9996 RVA: 0x000A51F0 File Offset: 0x000A35F0
		public List<UserInfo.User> GetUsersWithoutTouch(Predicate<UserInfo.User> pred)
		{
			object @lock = this.m_lock;
			List<UserInfo.User> result;
			lock (@lock)
			{
				result = (from user in this.m_users.Values
				where pred(user)
				select user).ToList<UserInfo.User>();
			}
			return result;
		}

		// Token: 0x0600270D RID: 9997 RVA: 0x000A5260 File Offset: 0x000A3660
		public bool IsOnline(string onlineId)
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				result = this.m_users_by_online_id.ContainsKey(onlineId);
			}
			return result;
		}

		// Token: 0x0600270E RID: 9998 RVA: 0x000A52AC File Offset: 0x000A36AC
		public bool IsOnline(ulong profileId)
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				result = this.m_users.ContainsKey(profileId);
			}
			return result;
		}

		// Token: 0x0600270F RID: 9999 RVA: 0x000A52F8 File Offset: 0x000A36F8
		public void SetUserInfo(UserInfo.User newUser)
		{
			object @lock = this.m_lock;
			UserInfo.User old_info;
			lock (@lock)
			{
				if (!this.m_users.TryGetValue(newUser.ProfileID, out old_info))
				{
					return;
				}
				this.m_users[newUser.ProfileID] = newUser;
				this.m_users_by_nick[newUser.Nickname] = newUser;
				this.m_users_by_online_id[newUser.OnlineID] = newUser;
				this.m_users_by_user_id[newUser.UserID] = newUser;
			}
			this.RaiseChangedEvent(old_info, newUser);
		}

		// Token: 0x06002710 RID: 10000 RVA: 0x000A53A4 File Offset: 0x000A37A4
		public void DumpUsers()
		{
			object @lock = this.m_lock;
			bool flag = false;
			try
			{
				Monitor.Enter(@lock, ref flag);
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("Users:");
				this.m_users.Values.ToList<UserInfo.User>().ForEach(delegate(UserInfo.User u)
				{
					sb.AppendFormat("{0}\n", u);
				});
				Log.Info(sb.ToString());
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(@lock);
				}
			}
		}

		// Token: 0x06002711 RID: 10001 RVA: 0x000A5434 File Offset: 0x000A3834
		public int GetOnlineUsersCount()
		{
			object @lock = this.m_lock;
			int count;
			lock (@lock)
			{
				count = this.m_users.Count;
			}
			return count;
		}

		// Token: 0x06002712 RID: 10002 RVA: 0x000A5480 File Offset: 0x000A3880
		public IEnumerable<Tuple<string, int>> GetOnlineUsersCountPerRegion()
		{
			object @lock = this.m_lock;
			IEnumerable<Tuple<string, int>> result;
			lock (@lock)
			{
				result = (from user in this.m_users.Values
				group user by user.RegionId into g
				select new Tuple<string, int>(g.Key, g.Count<UserInfo.User>())).ToList<Tuple<string, int>>();
			}
			return result;
		}

		// Token: 0x06002713 RID: 10003 RVA: 0x000A5530 File Offset: 0x000A3930
		public int GetOnlineUsersLimit()
		{
			return this.m_maxOnlineUsers;
		}

		// Token: 0x06002714 RID: 10004 RVA: 0x000A5538 File Offset: 0x000A3938
		public int GetJoinedUsersLimit()
		{
			return this.m_maxJoinUsers;
		}

		// Token: 0x06002715 RID: 10005 RVA: 0x000A5540 File Offset: 0x000A3940
		public bool IsOnlineUsersLimitReached()
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				result = (this.m_users.Count >= this.m_maxOnlineUsers);
			}
			return result;
		}

		// Token: 0x06002716 RID: 10006 RVA: 0x000A5598 File Offset: 0x000A3998
		public bool IsJoinUsersLimitReached()
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				result = (this.m_users.Count >= this.m_maxJoinUsers);
			}
			return result;
		}

		// Token: 0x06002717 RID: 10007 RVA: 0x000A55F0 File Offset: 0x000A39F0
		public static bool IsServerJid(string jid)
		{
			return jid.StartsWith("dedicated") || jid.EndsWith("dedicated");
		}

		// Token: 0x06002718 RID: 10008 RVA: 0x000A5610 File Offset: 0x000A3A10
		public static string OnlineIdToPlayerOnlineId(string onlineId)
		{
			return onlineId.Substring(0, onlineId.LastIndexOf('/') + 1) + "GameClient";
		}

		// Token: 0x06002719 RID: 10009 RVA: 0x000A5630 File Offset: 0x000A3A30
		private void RaiseLoggedInEvent(UserInfo.User user, ELoginType loginType)
		{
			this.UserLoggedIn.GetInvocationList().SafeForEach(delegate(Delegate step)
			{
				((UserLoginDeleg)step)(user, loginType);
			});
		}

		// Token: 0x0600271A RID: 10010 RVA: 0x000A5670 File Offset: 0x000A3A70
		private void RaiseLogoutEvent(UserInfo.User user, ELogoutType logout_type)
		{
			this.UserLoggedOut.GetInvocationList().SafeForEach(delegate(Delegate step)
			{
				((UserLogoutDeleg)step)(user, logout_type);
			});
		}

		// Token: 0x0600271B RID: 10011 RVA: 0x000A56B0 File Offset: 0x000A3AB0
		private void RaiseChangedEvent(UserInfo.User old_info, UserInfo.User new_info)
		{
			this.UserInfoChanged.GetInvocationList().SafeForEach(delegate(Delegate step)
			{
				((UserInfoChangedDeleg)step)(old_info, new_info);
			});
		}

		// Token: 0x0600271C RID: 10012 RVA: 0x000A56F0 File Offset: 0x000A3AF0
		public void RemoveAllUsers()
		{
			object @lock = this.m_lock;
			List<UserInfo.User> list;
			lock (@lock)
			{
				list = new List<UserInfo.User>(this.m_users.Values);
			}
			list.ForEach(delegate(UserInfo.User u)
			{
				this.UserLogout(u, ELogoutType.ServerClosedConnection);
			});
		}

		// Token: 0x0600271D RID: 10013 RVA: 0x000A5750 File Offset: 0x000A3B50
		private void OnConnectionStateChanged(EConnectionState prev, EConnectionState current)
		{
			if (current == EConnectionState.Disconnected && prev != EConnectionState.Disconnected)
			{
				Log.Info("Connection lost");
				object @lock = this.m_lock;
				lock (@lock)
				{
					this.m_usersRemovalTimer = this.m_timerFactory.CreateTimer(delegate(object state)
					{
						Log.Info("Removing all users");
						this.RemoveAllUsers();
					}, null, this.m_userDropTimeout, TimeSpan.FromMilliseconds(-1.0));
				}
			}
			else if (current == EConnectionState.Connected && prev != EConnectionState.Connected)
			{
				object lock2 = this.m_lock;
				lock (lock2)
				{
					if (this.m_usersRemovalTimer != null)
					{
						this.m_usersRemovalTimer.Dispose();
						this.m_usersRemovalTimer = null;
					}
				}
			}
		}

		// Token: 0x0600271E RID: 10014 RVA: 0x000A5834 File Offset: 0x000A3C34
		private void OnConfigChanged(ConfigEventArgs e)
		{
			if (string.Equals(e.Name, "max_online_users", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_maxOnlineUsers = e.iValue;
			}
			if (string.Equals(e.Name, "max_join_users", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_maxJoinUsers = e.iValue;
			}
		}

		// Token: 0x0600271F RID: 10015 RVA: 0x000A5885 File Offset: 0x000A3C85
		public ulong UnmangleUserId(ulong userId)
		{
			return Resources.BootstrapMode ? (userId & 1152921504606846975UL) : userId;
		}

		// Token: 0x06002720 RID: 10016 RVA: 0x000A58A2 File Offset: 0x000A3CA2
		public ulong MangleUserId(ulong userId, ulong platformId)
		{
			return (!Resources.BootstrapMode) ? userId : ((userId & 1152921504606846975UL) | platformId << 60);
		}

		// Token: 0x06002721 RID: 10017 RVA: 0x000A58C4 File Offset: 0x000A3CC4
		public bool IsSameBootstrap(ulong userId1, ulong userId2)
		{
			return (userId1 & 17293822569102704640UL) == (userId2 & 17293822569102704640UL);
		}

		// Token: 0x06002722 RID: 10018 RVA: 0x000A58DE File Offset: 0x000A3CDE
		public bool IsBootstrap(ulong userId, ulong platformId)
		{
			return !Resources.BootstrapMode || (userId & 17293822569102704640UL) >> 60 == platformId;
		}

		// Token: 0x0400140B RID: 5131
		public const string BUILD_TYPE_PROFILE = "--profile";

		// Token: 0x0400140C RID: 5132
		public const string BUILD_TYPE_RELEASE = "--release";

		// Token: 0x0400140D RID: 5133
		public const string BUILD_TYPE_UNDEFINED = "";

		// Token: 0x0400140E RID: 5134
		public const ulong INVALID_PROFILE_ID = 0UL;

		// Token: 0x0400140F RID: 5135
		public const string LimitConfigSectionName = "limits";

		// Token: 0x04001410 RID: 5136
		public const string MaxOnlineUsersAttributeName = "max_online_users";

		// Token: 0x04001411 RID: 5137
		public const string MaxJoinUsersAttributeName = "max_join_users";

		// Token: 0x04001412 RID: 5138
		public const string UsersDropTimeoutNodeName = "user_drop_timeout_sec";

		// Token: 0x04001413 RID: 5139
		private int m_maxOnlineUsers;

		// Token: 0x04001414 RID: 5140
		private int m_maxJoinUsers;

		// Token: 0x04001415 RID: 5141
		private readonly IUserFactory m_userFactory;

		// Token: 0x04001416 RID: 5142
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x04001417 RID: 5143
		private readonly ITimerFactory m_timerFactory;

		// Token: 0x04001418 RID: 5144
		private ITimer m_usersRemovalTimer;

		// Token: 0x04001419 RID: 5145
		private TimeSpan m_userDropTimeout;

		// Token: 0x0400141A RID: 5146
		private readonly object m_lock = new object();

		// Token: 0x0400141B RID: 5147
		private readonly Dictionary<ulong, UserInfo.User> m_users = new Dictionary<ulong, UserInfo.User>();

		// Token: 0x0400141C RID: 5148
		private readonly Dictionary<string, UserInfo.User> m_users_by_nick = new Dictionary<string, UserInfo.User>();

		// Token: 0x0400141D RID: 5149
		private readonly Dictionary<string, UserInfo.User> m_users_by_online_id = new Dictionary<string, UserInfo.User>();

		// Token: 0x0400141E RID: 5150
		private readonly Dictionary<ulong, UserInfo.User> m_users_by_user_id = new Dictionary<ulong, UserInfo.User>();

		// Token: 0x04001423 RID: 5155
		private const ulong MANGLE_USER_ID_MASK = 17293822569102704640UL;

		// Token: 0x04001424 RID: 5156
		private const ulong UNMANGLE_USER_ID_MASK = 1152921504606846975UL;

		// Token: 0x0200075F RID: 1887
		public class User
		{
			// Token: 0x0600272D RID: 10029 RVA: 0x000A5944 File Offset: 0x000A3D44
			public User(ulong uid, ulong pid, string nick, string jid, string token, string ip, string buildType, string regionId, AccessLevel accessLevel, DateTime loginTime, ulong exp, int rank, SBannerInfo banner, ProfileProgressionInfo profileProgression, ClientVersion version)
			{
				this.UserID = uid;
				this.ProfileID = pid;
				this.Nickname = nick;
				this.OnlineID = jid;
				this.Token = token;
				this.AccessLvl = accessLevel;
				this.IP = ip;
				this.LoginTime = loginTime;
				this.Experience = exp;
				this.Rank = rank;
				this.Banner = banner;
				this.BuildType = buildType;
				this.RegionId = regionId;
				this.ProfileProgression = profileProgression;
				this.Version = version;
				this.m_lastTouched = DateTime.Now;
			}

			// Token: 0x0600272E RID: 10030 RVA: 0x000A59D8 File Offset: 0x000A3DD8
			public User(ulong pid, string jid, string token, string ip, string buildType, string regionId, SProfileInfo profileInfo, AccessLevel accessLevel, DateTime loginTime, ProfileProgressionInfo profileProgression, ClientVersion version) : this(profileInfo.UserID, pid, profileInfo.Nickname, jid, token, ip, buildType, regionId, accessLevel, loginTime, profileInfo.RankInfo.Points, profileInfo.RankInfo.RankId, profileInfo.Banner, profileProgression, version)
			{
			}

			// Token: 0x0600272F RID: 10031 RVA: 0x000A5A2C File Offset: 0x000A3E2C
			protected User(UserInfo.User user) : this(user.UserID, user.ProfileID, user.Nickname, user.OnlineID, user.Token, user.IP, user.BuildType, user.RegionId, user.AccessLvl, user.LoginTime, user.Experience, user.Rank, user.Banner, user.ProfileProgression, user.Version)
			{
			}

			// Token: 0x06002730 RID: 10032 RVA: 0x000A5A99 File Offset: 0x000A3E99
			protected User(UserInfo.User user, AccessLevel level) : this(user)
			{
				this.AccessLvl = level;
			}

			// Token: 0x06002731 RID: 10033 RVA: 0x000A5AA9 File Offset: 0x000A3EA9
			protected User(UserInfo.User user, SRankInfo rank) : this(user)
			{
				this.Rank = rank.RankId;
				this.Experience = rank.Points;
			}

			// Token: 0x06002732 RID: 10034 RVA: 0x000A5ACC File Offset: 0x000A3ECC
			protected User(UserInfo.User user, ProfileProgressionInfo progressionInfo) : this(user)
			{
				this.ProfileProgression = progressionInfo;
			}

			// Token: 0x06002733 RID: 10035 RVA: 0x000A5ADC File Offset: 0x000A3EDC
			protected User(UserInfo.User user, SBannerInfo banner) : this(user)
			{
				this.Banner = banner;
			}

			// Token: 0x170003AA RID: 938
			// (get) Token: 0x06002734 RID: 10036 RVA: 0x000A5AEC File Offset: 0x000A3EEC
			public TimeSpan UntouchedFor
			{
				get
				{
					return DateTime.Now - this.m_lastTouched;
				}
			}

			// Token: 0x06002735 RID: 10037 RVA: 0x000A5AFE File Offset: 0x000A3EFE
			public virtual UserInfo.User Clone()
			{
				return new UserInfo.User(this);
			}

			// Token: 0x06002736 RID: 10038 RVA: 0x000A5B06 File Offset: 0x000A3F06
			public virtual UserInfo.User CloneWithAccessLevel(AccessLevel level)
			{
				return new UserInfo.User(this, level);
			}

			// Token: 0x06002737 RID: 10039 RVA: 0x000A5B0F File Offset: 0x000A3F0F
			public virtual UserInfo.User CloneWithRank(SRankInfo rankInfo)
			{
				return new UserInfo.User(this, rankInfo);
			}

			// Token: 0x06002738 RID: 10040 RVA: 0x000A5B18 File Offset: 0x000A3F18
			public virtual UserInfo.User CloneWithProgression(ProfileProgressionInfo profileProgression)
			{
				return new UserInfo.User(this, profileProgression);
			}

			// Token: 0x06002739 RID: 10041 RVA: 0x000A5B21 File Offset: 0x000A3F21
			public virtual UserInfo.User CloneWithBanner(SBannerInfo banner)
			{
				return new UserInfo.User(this, banner);
			}

			// Token: 0x0600273A RID: 10042 RVA: 0x000A5B2A File Offset: 0x000A3F2A
			public void Touch()
			{
				this.m_lastTouched = DateTime.Now;
			}

			// Token: 0x170003AB RID: 939
			// (get) Token: 0x0600273B RID: 10043 RVA: 0x000A5B37 File Offset: 0x000A3F37
			public TimeSpan Playtime
			{
				get
				{
					return TimeSpan.Zero;
				}
			}

			// Token: 0x0600273C RID: 10044 RVA: 0x000A5B40 File Offset: 0x000A3F40
			public override string ToString()
			{
				return string.Format("UserId {0}, ProfileId {1}, Nickname {2}, RegionId {3}, OnlineId {4}, Token {5}, accessLevel {6}, IP {7}, Rank {8}, Progression {9}", new object[]
				{
					this.UserID,
					this.ProfileID,
					this.Nickname,
					this.RegionId,
					this.OnlineID,
					this.Token,
					this.AccessLvl,
					this.IP,
					this.Rank,
					this.ProfileProgression
				});
			}

			// Token: 0x170003AC RID: 940
			// (get) Token: 0x0600273D RID: 10045 RVA: 0x000A5BCD File Offset: 0x000A3FCD
			public bool IsOnline
			{
				get
				{
					return !string.IsNullOrEmpty(this.OnlineID);
				}
			}

			// Token: 0x0400142D RID: 5165
			public readonly ulong UserID;

			// Token: 0x0400142E RID: 5166
			public readonly ulong ProfileID;

			// Token: 0x0400142F RID: 5167
			public readonly string Nickname;

			// Token: 0x04001430 RID: 5168
			public readonly string OnlineID;

			// Token: 0x04001431 RID: 5169
			public readonly string Token;

			// Token: 0x04001432 RID: 5170
			public readonly AccessLevel AccessLvl;

			// Token: 0x04001433 RID: 5171
			public readonly string IP;

			// Token: 0x04001434 RID: 5172
			public readonly DateTime LoginTime;

			// Token: 0x04001435 RID: 5173
			public readonly ulong Experience;

			// Token: 0x04001436 RID: 5174
			public readonly int Rank;

			// Token: 0x04001437 RID: 5175
			public readonly SBannerInfo Banner;

			// Token: 0x04001438 RID: 5176
			public readonly string BuildType;

			// Token: 0x04001439 RID: 5177
			public readonly string RegionId;

			// Token: 0x0400143A RID: 5178
			public readonly ProfileProgressionInfo ProfileProgression;

			// Token: 0x0400143B RID: 5179
			public readonly ClientVersion Version;

			// Token: 0x0400143C RID: 5180
			private DateTime m_lastTouched;
		}
	}
}
