using System;
using System.Globalization;
using System.Xml;
using MasterServer.DAL;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.InvitationSystem
{
	// Token: 0x02000310 RID: 784
	[Serializable]
	internal struct CommonInitiatorData
	{
		// Token: 0x060011FF RID: 4607 RVA: 0x0004741C File Offset: 0x0004581C
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("initiator_info");
			xmlElement.SetAttribute("online_id", this.OnlineId);
			xmlElement.SetAttribute("profile_id", this.ProfileId.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("is_online", ((!this.IsOnline) ? 0 : 1).ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("name", this.Nickname);
			xmlElement.SetAttribute("clan_name", this.ClanName);
			xmlElement.SetAttribute("experience", this.Experience.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("badge", this.BannerInfo.Badge.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("mark", this.BannerInfo.Mark.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("stripe", this.BannerInfo.Stripe.ToString(CultureInfo.InvariantCulture));
			return xmlElement;
		}

		// Token: 0x06001200 RID: 4608 RVA: 0x0004752C File Offset: 0x0004592C
		internal static CommonInitiatorData CreateInitiatorData(IClanService clanService, IUserRepository userRepository, ulong profileId)
		{
			UserInfo.User user = userRepository.GetUser(profileId);
			ClanInfo clanInfoByPid = clanService.GetClanInfoByPid(profileId);
			return new CommonInitiatorData
			{
				UserId = user.UserID,
				OnlineId = user.OnlineID,
				ProfileId = profileId,
				Nickname = user.Nickname,
				ClanName = ((clanInfoByPid != null) ? clanInfoByPid.Name : string.Empty),
				BannerInfo = user.Banner,
				Experience = user.Experience,
				IsOnline = user.IsOnline
			};
		}

		// Token: 0x04000820 RID: 2080
		public ulong UserId;

		// Token: 0x04000821 RID: 2081
		public ulong ProfileId;

		// Token: 0x04000822 RID: 2082
		public string OnlineId;

		// Token: 0x04000823 RID: 2083
		public string Nickname;

		// Token: 0x04000824 RID: 2084
		public string ClanName;

		// Token: 0x04000825 RID: 2085
		public bool IsOnline;

		// Token: 0x04000826 RID: 2086
		public ulong Experience;

		// Token: 0x04000827 RID: 2087
		public SBannerInfo BannerInfo;
	}
}
