using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000573 RID: 1395
	[QueryAttributes(TagName = "set_banner")]
	internal class SetProfileBannerQuery : BaseQuery
	{
		// Token: 0x06001E0D RID: 7693 RVA: 0x00079CF9 File Offset: 0x000780F9
		public SetProfileBannerQuery(IProfileValidationService profileValidationService, IDALService dalService, IUserRepository userRepository, IGameRoomManager gameRoomManager)
		{
			this.m_profileValidationService = profileValidationService;
			this.m_dalService = dalService;
			this.m_userRepository = userRepository;
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x06001E0E RID: 7694 RVA: 0x00079D20 File Offset: 0x00078120
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User user_info;
			if (!base.GetClientInfo(fromJid, out user_info))
			{
				return -3;
			}
			SBannerInfo banner = new SBannerInfo(uint.MaxValue, uint.MaxValue, uint.MaxValue);
			if (this.m_profileValidationService.Validate(user_info, request))
			{
				banner.Badge = uint.Parse(request.GetAttribute("banner_badge"));
				banner.Mark = uint.Parse(request.GetAttribute("banner_mark"));
				banner.Stripe = uint.Parse(request.GetAttribute("banner_stripe"));
			}
			this.m_dalService.ProfileSystem.SetProfileBanner(user_info.ProfileID, banner);
			UserInfo.User userInfo = user_info.CloneWithBanner(banner);
			this.m_userRepository.SetUserInfo(userInfo);
			IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(user_info.ProfileID);
			if (roomByPlayer != null)
			{
				roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					RoomPlayer roomPlayer;
					if (r.GetState<CoreState>(AccessMode.ReadWrite).Players.TryGetValue(user_info.ProfileID, out roomPlayer))
					{
						roomPlayer.Banner = banner;
						r.SignalPlayersChanged();
					}
				});
			}
			return 0;
		}

		// Token: 0x04000E87 RID: 3719
		private readonly IProfileValidationService m_profileValidationService;

		// Token: 0x04000E88 RID: 3720
		private readonly IDALService m_dalService;

		// Token: 0x04000E89 RID: 3721
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000E8A RID: 3722
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
