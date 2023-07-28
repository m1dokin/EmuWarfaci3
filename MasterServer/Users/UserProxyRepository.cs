using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.Users
{
	// Token: 0x02000803 RID: 2051
	[Service]
	[Singleton]
	internal class UserProxyRepository : IUserProxyRepository
	{
		// Token: 0x06002A10 RID: 10768 RVA: 0x000B5A95 File Offset: 0x000B3E95
		public UserProxyRepository(IUserRepository userRepository, IProfileProgressionService profileProgressionService, ISessionInfoService sessionInfoService, IDALService dalService)
		{
			this.m_userRepository = userRepository;
			this.m_profileProgressionService = profileProgressionService;
			this.m_sessionInfoService = sessionInfoService;
			this.m_dalService = dalService;
		}

		// Token: 0x06002A11 RID: 10769 RVA: 0x000B5ABC File Offset: 0x000B3EBC
		public UserInfo.User GetUserOrProxyByProfileId(ulong profileId)
		{
			return this.GetUserOrProxyByProfileId(new List<ulong>
			{
				profileId
			}).FirstOrDefault<UserInfo.User>();
		}

		// Token: 0x06002A12 RID: 10770 RVA: 0x000B5AE2 File Offset: 0x000B3EE2
		public IEnumerable<UserInfo.User> GetUserOrProxyByProfileId(IEnumerable<ulong> profileIds)
		{
			return this.GetUserOrProxyByProfileId(profileIds, false);
		}

		// Token: 0x06002A13 RID: 10771 RVA: 0x000B5AEC File Offset: 0x000B3EEC
		public UserInfo.User GetUserOrProxyByProfileId(ulong profileId, bool full)
		{
			return this.GetUserOrProxyByProfileId(new List<ulong>
			{
				profileId
			}, full).FirstOrDefault<UserInfo.User>();
		}

		// Token: 0x06002A14 RID: 10772 RVA: 0x000B5B14 File Offset: 0x000B3F14
		public IEnumerable<UserInfo.User> GetUserOrProxyByProfileId(IEnumerable<ulong> profileIds, bool full)
		{
			List<UserInfo.User> list = new List<UserInfo.User>();
			List<ulong> list2 = new List<ulong>();
			foreach (ulong num in profileIds.Distinct<ulong>())
			{
				UserInfo.User user = this.m_userRepository.GetUser(num);
				if (user != null)
				{
					list.Add(user);
				}
				else
				{
					list2.Add(num);
				}
			}
			if (!list2.Any<ulong>())
			{
				return list;
			}
			Dictionary<ulong, ProfileInfo> dictionary = null;
			if (full)
			{
				dictionary = this.m_sessionInfoService.GetProfileInfo(list2).ToDictionary((ProfileInfo key) => key.ProfileID, (ProfileInfo value) => value);
			}
			foreach (ulong num2 in list2)
			{
				SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(num2);
				if (profileInfo.Id != 0UL)
				{
					UserInfo.User item;
					if (full)
					{
						ProfileInfo profileInfo2 = dictionary[num2];
						item = this.m_userRepository.Make(profileInfo2, profileInfo, this.m_profileProgressionService.GetProgression(profileInfo.Id));
					}
					else
					{
						item = this.m_userRepository.Make(profileInfo, this.m_profileProgressionService.GetProgression(profileInfo.Id));
					}
					list.Add(item);
				}
			}
			return list;
		}

		// Token: 0x06002A15 RID: 10773 RVA: 0x000B5CC4 File Offset: 0x000B40C4
		public UserInfo.User GetUserOrProxyByUserId(ulong userId)
		{
			return this.GetUserOrProxyByUserId(userId, false);
		}

		// Token: 0x06002A16 RID: 10774 RVA: 0x000B5CD0 File Offset: 0x000B40D0
		public UserInfo.User GetUserOrProxyByUserId(ulong userId, bool full)
		{
			UserInfo.User user = this.m_userRepository.GetUserByUserId(userId);
			if (user == null)
			{
				SAuthProfile sauthProfile = this.m_dalService.ProfileSystem.GetUserProfiles(userId).FirstOrDefault<SAuthProfile>();
				if (sauthProfile.ProfileID != 0UL)
				{
					SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(sauthProfile.ProfileID);
					if (full)
					{
						ProfileInfo profileInfo2 = this.m_sessionInfoService.GetProfileInfo(sauthProfile.ProfileID);
						user = this.m_userRepository.Make(profileInfo2, profileInfo, this.m_profileProgressionService.GetProgression(profileInfo.Id));
					}
					else
					{
						user = this.m_userRepository.Make(profileInfo, this.m_profileProgressionService.GetProgression(profileInfo.Id));
					}
				}
			}
			return user;
		}

		// Token: 0x06002A17 RID: 10775 RVA: 0x000B5D8B File Offset: 0x000B418B
		public UserInfo.User GetUserOrProxyByNickname(string nickname)
		{
			return this.GetUserOrProxyByNickname(nickname, false);
		}

		// Token: 0x06002A18 RID: 10776 RVA: 0x000B5D98 File Offset: 0x000B4198
		public UserInfo.User GetUserOrProxyByNickname(string nickname, bool full)
		{
			UserInfo.User user = this.m_userRepository.GetUser(nickname);
			if (user == null)
			{
				SProfileInfo profileByNickname = this.m_dalService.ProfileSystem.GetProfileByNickname(nickname);
				if (profileByNickname.Id != 0UL)
				{
					if (full)
					{
						ProfileInfo profileInfo = this.m_sessionInfoService.GetProfileInfo(profileByNickname.Id);
						user = this.m_userRepository.Make(profileInfo, profileByNickname, this.m_profileProgressionService.GetProgression(profileByNickname.Id));
					}
					else
					{
						user = this.m_userRepository.Make(profileByNickname, this.m_profileProgressionService.GetProgression(profileByNickname.Id));
					}
				}
			}
			return user;
		}

		// Token: 0x04001664 RID: 5732
		private readonly IUserRepository m_userRepository;

		// Token: 0x04001665 RID: 5733
		private readonly IProfileProgressionService m_profileProgressionService;

		// Token: 0x04001666 RID: 5734
		private readonly ISessionInfoService m_sessionInfoService;

		// Token: 0x04001667 RID: 5735
		private readonly IDALService m_dalService;
	}
}
