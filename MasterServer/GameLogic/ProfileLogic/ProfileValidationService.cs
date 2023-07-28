using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.GameLogic.Achievements;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200000A RID: 10
	[Service]
	[Singleton]
	internal class ProfileValidationService : ServiceModule, IProfileValidationService
	{
		// Token: 0x06000019 RID: 25 RVA: 0x00004684 File Offset: 0x00002A84
		public ProfileValidationService(ILogService logService, IClanService clanService, IAchievementSystem achievementSystem, IItemCache itemCache, IConfigProvider<ProfileValidationServiceConfig> configProvider)
		{
			UnicodeCategory[] array = new UnicodeCategory[5];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>{33a8b3dd-83c5-4517-9b3a-a5bd83bb24dc}.$field-1).FieldHandle);
			this.allowed_categories = array;
			base..ctor();
			this.m_configProvider = configProvider;
			this.m_itemCache = itemCache;
			this.m_logService = logService;
			this.m_clanService = clanService;
			this.m_achievementSystem = achievementSystem;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000046EC File Offset: 0x00002AEC
		public override void Start()
		{
			ProfileValidationServiceConfig profileValidationServiceConfig = this.m_configProvider.Get();
			Log.Info<string>("Profile validation will {0} achievements", (!profileValidationServiceConfig.IsCheckAchievementsEnabled) ? "skip" : "check");
			Log.Info<string>("Profile validation will {0} profile info", (!profileValidationServiceConfig.IsCheckProfileEnabled) ? "skip" : "check");
			Log.Info<string>("Profile validation will {0} clan info", (!profileValidationServiceConfig.IsCheckClanEnabled) ? "skip" : "check");
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00004774 File Offset: 0x00002B74
		public bool Validate(UserInfo.User user_info, XmlElement profile_info)
		{
			ProfileValidationServiceConfig profileValidationServiceConfig = this.m_configProvider.Get();
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				if (profileValidationServiceConfig.IsCheckAchievementsEnabled && profile_info.HasAttribute("banner_badge") && !this.ValidateAchievements(user_info, profile_info))
				{
					logGroup.CharacterAlarmLog(user_info.UserID, user_info.ProfileID, user_info.IP, "achievements");
					return false;
				}
				if (profileValidationServiceConfig.IsCheckProfileEnabled && profile_info.HasAttribute("online_id") && !this.ValidateProfile(user_info, profile_info))
				{
					logGroup.CharacterAlarmLog(user_info.UserID, user_info.ProfileID, user_info.IP, "profile");
					return false;
				}
				if (profileValidationServiceConfig.IsCheckClanEnabled && profile_info.HasAttribute("clan_name") && !this.ValidateClan(user_info, profile_info))
				{
					logGroup.CharacterAlarmLog(user_info.UserID, user_info.ProfileID, user_info.IP, "clan");
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000048A4 File Offset: 0x00002CA4
		public NameValidationResult ValidateNickname(string nickname)
		{
			if (nickname.Length < 4)
			{
				return NameValidationResult.LengthTooShort;
			}
			if (nickname.Length > 16)
			{
				return NameValidationResult.LengthTooLong;
			}
			for (int num = 0; num != nickname.Length; num++)
			{
				if (!Utils.Contains<char>(this.special_chars, nickname[num]))
				{
					UnicodeCategory unicodeCategory = char.GetUnicodeCategory(nickname[num]);
					if (!Utils.Contains<UnicodeCategory>(this.allowed_categories, unicodeCategory))
					{
						return NameValidationResult.UnsupportedCharacter;
					}
				}
			}
			return NameValidationResult.NoError;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00004924 File Offset: 0x00002D24
		public bool IsHeadValid(string head)
		{
			ProfileValidationServiceConfig profileValidationServiceConfig = this.m_configProvider.Get();
			if (!profileValidationServiceConfig.IsCheckHeadEnabled)
			{
				return true;
			}
			Dictionary<string, SItem> allItemsByName = this.m_itemCache.GetAllItemsByName();
			SItem sitem;
			return allItemsByName.TryGetValue(head, out sitem) && sitem.Slots.Contains("defaulthead");
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00004978 File Offset: 0x00002D78
		private bool ValidateProfile(UserInfo.User user_info, XmlElement profile_info)
		{
			string attribute = profile_info.GetAttribute("online_id");
			if (user_info.OnlineID != attribute)
			{
				Log.Warning<string, string, ulong>("Profile validation received online id {0} expected {1} for profile {2}", attribute, user_info.OnlineID, user_info.ProfileID);
				return false;
			}
			string attribute2 = profile_info.GetAttribute("nickname");
			if (user_info.Nickname != attribute2)
			{
				Log.Warning<string, string, ulong>("Profile validation received nick {0} expected {1} for profile {2}", attribute2, user_info.Nickname, user_info.ProfileID);
				return false;
			}
			ulong num = ulong.Parse(profile_info.GetAttribute("experience"));
			if (num > user_info.Experience)
			{
				Log.Warning<ulong, ulong, ulong>("Profile validation received exp {0} expected {1} for profile {2}", num, user_info.Experience, user_info.ProfileID);
				return false;
			}
			return true;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00004A28 File Offset: 0x00002E28
		private bool ValidateClan(UserInfo.User user_info, XmlElement profile_info)
		{
			ClanMember memberInfo = this.m_clanService.GetMemberInfo(user_info.ProfileID);
			if (memberInfo == null)
			{
				Log.Warning<ulong>("Profile validation failed to get clan member info for profile {0}", user_info.ProfileID);
				return false;
			}
			int num = int.Parse(profile_info.GetAttribute("clan_role"));
			if (num != (int)memberInfo.ClanRole)
			{
				Log.Warning<int, EClanRole, ulong>("Profile validation received clan role {0} expected {1} for profile {2}", num, memberInfo.ClanRole, user_info.ProfileID);
				return false;
			}
			ClanInfo clanInfo = this.m_clanService.GetClanInfo(memberInfo.ClanID);
			if (clanInfo == null)
			{
				Log.Warning<ulong>("Profile validation failed to get clan info for profile {0}", user_info.ProfileID);
				return false;
			}
			string attribute = profile_info.GetAttribute("clan_name");
			if (clanInfo.Name != attribute)
			{
				Log.Warning<string, string, ulong>("Profile validation received clan name {0} expected {1} for profile {2}", attribute, clanInfo.Name, user_info.ProfileID);
				return false;
			}
			ulong num2 = ulong.Parse(profile_info.GetAttribute("clan_points"));
			if (num2 > clanInfo.ClanPoints)
			{
				Log.Warning<ulong, ulong, ulong>("Profile validation received clan points {0} expected {1} for profile {2}", num2, clanInfo.ClanPoints, user_info.ProfileID);
				return false;
			}
			return true;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00004B30 File Offset: 0x00002F30
		private bool ValidateAchievements(UserInfo.User user_info, XmlElement profile_info)
		{
			Dictionary<uint, AchievementUpdateChunk> currentProfileAchievements = this.m_achievementSystem.GetCurrentProfileAchievements(user_info.ProfileID);
			if (profile_info.HasAttribute("banner_badge"))
			{
				uint num = uint.Parse(profile_info.GetAttribute("banner_badge"));
				if (!this.IsValidAchievement(currentProfileAchievements, num))
				{
					Log.Warning<uint, ulong>("Profile validation failed to validate badge {0} for profile {1}", num, user_info.ProfileID);
					return false;
				}
			}
			if (profile_info.HasAttribute("banner_mark"))
			{
				uint num2 = uint.Parse(profile_info.GetAttribute("banner_mark"));
				if (!this.IsValidAchievement(currentProfileAchievements, num2))
				{
					Log.Warning<uint, ulong>("Profile validation failed to validate mark {0} for profile {1}", num2, user_info.ProfileID);
					return false;
				}
			}
			if (profile_info.HasAttribute("banner_stripe"))
			{
				uint num3 = uint.Parse(profile_info.GetAttribute("banner_stripe"));
				if (!this.IsValidAchievement(currentProfileAchievements, num3))
				{
					Log.Warning<uint, ulong>("Profile validation failed to validate stripe {0} for profile {1}", num3, user_info.ProfileID);
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00004C14 File Offset: 0x00003014
		private bool IsValidAchievement(Dictionary<uint, AchievementUpdateChunk> achievements, uint achievement_id)
		{
			if (achievement_id == 4294967295U)
			{
				return true;
			}
			AchievementDescription achievementDesc = this.m_achievementSystem.GetAchievementDesc(achievement_id);
			if (achievementDesc == null)
			{
				Log.Warning<uint>("Profile validation failed to find achievement {0}", achievement_id);
				return false;
			}
			AchievementUpdateChunk achievementUpdateChunk;
			if (!achievements.TryGetValue(achievement_id, out achievementUpdateChunk))
			{
				Log.Warning<uint>("Profile validation failed to get achievement progress for achievement id {0}", achievement_id);
				return false;
			}
			return (long)achievementUpdateChunk.progress >= (long)((ulong)achievementDesc.Amount);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00004C7C File Offset: 0x0000307C
		public bool ValidateProfileClass(UserInfo.User user_info, uint curr_class)
		{
			if (curr_class > SProfileInfo.MAX_CLASS || curr_class == SProfileInfo.HEAVY_CLASS_ID)
			{
				this.m_logService.Event.CharacterAlarmLog(user_info.UserID, user_info.ProfileID, user_info.IP, "class_id");
				return false;
			}
			return true;
		}

		// Token: 0x0400000D RID: 13
		private readonly ILogService m_logService;

		// Token: 0x0400000E RID: 14
		private readonly IClanService m_clanService;

		// Token: 0x0400000F RID: 15
		private readonly IAchievementSystem m_achievementSystem;

		// Token: 0x04000010 RID: 16
		private readonly IItemCache m_itemCache;

		// Token: 0x04000011 RID: 17
		private readonly IConfigProvider<ProfileValidationServiceConfig> m_configProvider;

		// Token: 0x04000012 RID: 18
		private char[] special_chars = new char[]
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

		// Token: 0x04000013 RID: 19
		private UnicodeCategory[] allowed_categories;
	}
}
