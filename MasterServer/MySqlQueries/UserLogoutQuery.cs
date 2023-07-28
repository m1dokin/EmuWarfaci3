using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.Platform.Payment.Exceptions;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.MySqlQueries
{
	// Token: 0x0200067F RID: 1663
	[QueryAttributes(TagName = "user_logout")]
	internal class UserLogoutQuery : BaseQuery
	{
		// Token: 0x06002308 RID: 8968 RVA: 0x00092F45 File Offset: 0x00091345
		public UserLogoutQuery(ICatalogService catalogService, ILogService logService, ITagService tagService)
		{
			this.m_catalogService = catalogService;
			this.m_logService = logService;
			this.m_tagService = tagService;
		}

		// Token: 0x06002309 RID: 8969 RVA: 0x00092F64 File Offset: 0x00091364
		public override int HandleRequest(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			string attribute = request.GetAttribute("user");
			ulong profileId = ulong.Parse(request.GetAttribute("profile_id"));
			UserStatus userStatus = (UserStatus)int.Parse(request.GetAttribute("user_status"));
			ulong num = ulong.Parse(request.GetAttribute("ingame_playtime"));
			ulong num2 = ulong.Parse(request.GetAttribute("login_time"));
			UserInfo.User user = base.UserRepository.GetUser(profileId);
			if (user == null)
			{
				return 0;
			}
			DateTime t = DateTime.MaxValue;
			try
			{
				t = TimeUtils.UTCTimestampToUTCTime(num2);
			}
			catch (ArgumentOutOfRangeException e)
			{
				Log.Warning(string.Format("Can't create LoginTime from timestamp {0}", num2));
				Log.Warning(e);
			}
			if (user.LoginTime > t)
			{
				return 0;
			}
			ELogoutType elogoutType;
			if (userStatus != UserStatus.Logout)
			{
				if (userStatus != UserStatus.Offline)
				{
					Log.Warning<UserStatus>("Incorect status {0} for logout", userStatus);
					return -1;
				}
				elogoutType = ELogoutType.LostConnection;
			}
			else
			{
				elogoutType = ELogoutType.Logout;
			}
			base.UserRepository.UserLogout(user, elogoutType);
			this.WriteLogoutToLog(user, TimeSpan.FromSeconds(num), elogoutType);
			return 0;
		}

		// Token: 0x0600230A RID: 8970 RVA: 0x00093088 File Offset: 0x00091488
		private void WriteLogoutToLog(UserInfo.User user, TimeSpan ingamePlayTime, ELogoutType logout_type)
		{
			Dictionary<Currency, ulong> dictionary = new Dictionary<Currency, ulong>
			{
				{
					Currency.GameMoney,
					0UL
				},
				{
					Currency.CryMoney,
					0UL
				},
				{
					Currency.CrownMoney,
					0UL
				}
			};
			try
			{
				foreach (CustomerAccount customerAccount in this.m_catalogService.GetCustomerAccounts(user.UserID))
				{
					dictionary[customerAccount.Currency] = customerAccount.Money;
				}
			}
			catch (PaymentServiceException e)
			{
				Log.Error(e);
			}
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				TimeSpan timeSpan = DateTime.UtcNow - user.LoginTime;
				string tags = this.m_tagService.GetUserTags(user.UserID).ToString();
				switch (logout_type)
				{
				case ELogoutType.Logout:
				case ELogoutType.LostConnection:
				case ELogoutType.ServerClosedConnection:
					logGroup.CharacterLogoutLog(user.UserID, user.LoginTime, user.IP, user.ProfileID, user.Nickname, user.Rank, user.Experience, (timeSpan.TotalSeconds <= 0.0) ? TimeSpan.Zero : timeSpan, ingamePlayTime, dictionary[Currency.GameMoney], dictionary[Currency.CryMoney], dictionary[Currency.CrownMoney], user.OnlineID, user.Token, tags, logout_type);
					break;
				}
				if (logout_type == ELogoutType.LostConnection)
				{
					logGroup.CharacterLostConnectionLog(user.UserID, user.IP, user.ProfileID, user.OnlineID);
				}
			}
		}

		// Token: 0x040011A6 RID: 4518
		private readonly ICatalogService m_catalogService;

		// Token: 0x040011A7 RID: 4519
		private readonly ILogService m_logService;

		// Token: 0x040011A8 RID: 4520
		private readonly ITagService m_tagService;
	}
}
