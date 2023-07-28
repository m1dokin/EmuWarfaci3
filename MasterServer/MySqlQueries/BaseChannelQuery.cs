using System;
using System.Linq;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Regions;
using MasterServer.CryOnlineNET;
using MasterServer.Database;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SponsorSystem;
using MasterServer.Platform.Payment;
using MasterServer.Users;

namespace MasterServer.MySqlQueries
{
	// Token: 0x0200067A RID: 1658
	internal abstract class BaseChannelQuery : BaseQuery
	{
		// Token: 0x060022FB RID: 8955 RVA: 0x0009263C File Offset: 0x00090A3C
		public BaseChannelQuery(ISessionInfoService sessionInfoService, IDALService dalService, IProfileProgressionService profileProgressionService, IUserRepository userRepository, ILogService logService, IRankSystem rankSystem, IOnlineVariables onlineVariables, IPaymentService paymentService, IAbuseReportService abuseReportService, IItemsPurchase itemsPurchase, ISponsorUnlock sponsorUnlock, IClientVersionsManagementService clientVersionsManagementService, IRegionsService regionsService)
		{
			this.m_sessionInfoService = sessionInfoService;
			this.m_dalService = dalService;
			this.m_profileProgressionService = profileProgressionService;
			this.m_userRepository = userRepository;
			this.m_logService = logService;
			this.m_rankSystem = rankSystem;
			this.m_onlineVariables = onlineVariables;
			this.m_paymentService = paymentService;
			this.m_abuseReportService = abuseReportService;
			this.m_itemsPurchase = itemsPurchase;
			this.m_sponsorUnlock = sponsorUnlock;
			this.m_clientVersionsManagementService = clientVersionsManagementService;
			this.m_regionsService = regionsService;
		}

		// Token: 0x060022FC RID: 8956 RVA: 0x000926B4 File Offset: 0x00090AB4
		protected void FlushCachedProfileData(ulong userId, ulong profileId, bool fast_switch)
		{
			this.m_dalService.ProfileSystem.FlushCatalogCache(userId);
			ConfigSection section = Resources.DBMasterSettings.GetSection("Memcached");
			if (section.HasValue("flush_profile"))
			{
				int num;
				section.Get("flush_profile", out num);
				if ((!fast_switch && (num & 1) != 0) || (fast_switch && (num & 2) != 0))
				{
					this.m_dalService.ProfileSystem.FlushProfileCache(userId, profileId);
				}
			}
		}

		// Token: 0x060022FD RID: 8957 RVA: 0x00092730 File Offset: 0x00090B30
		protected bool CheckBootstrap(SessionInfo sessionInfo)
		{
			string text = sessionInfo.Tags.List.FirstOrDefault((string t) => t.StartsWith("bootstrap_"));
			return Resources.BootstrapMode ? (!string.IsNullOrEmpty(text) && text.Substring("bootstrap_".Length) == Resources.BootstrapName) : string.IsNullOrEmpty(text);
		}

		// Token: 0x060022FE RID: 8958 RVA: 0x000927A8 File Offset: 0x00090BA8
		protected bool CheckRankRestrictions(ProfileProxy profile)
		{
			if (!this.m_rankSystem.CanJoinChannel(profile.ProfileInfo.RankInfo.RankId))
			{
				return false;
			}
			this.m_rankSystem.ValidateExperience(profile);
			return true;
		}

		// Token: 0x060022FF RID: 8959 RVA: 0x000927E7 File Offset: 0x00090BE7
		protected bool CheckRegionId(ulong userId, string regionId)
		{
			if (!this.m_regionsService.IsRegionValid(regionId))
			{
				Log.Warning<ulong, string>("User {0} joined with \"{1}\" region id. This looks suspicious. Rejecting login.", userId, regionId);
				return false;
			}
			return true;
		}

		// Token: 0x04001192 RID: 4498
		protected const int E_PROFILE_NOT_EXIST = 1;

		// Token: 0x04001193 RID: 4499
		protected const int E_VERSION_MISMATCH = 2;

		// Token: 0x04001194 RID: 4500
		protected const int E_USER_BANNED = 3;

		// Token: 0x04001195 RID: 4501
		protected const int E_MAX_CHANNEL_LIMIT = 4;

		// Token: 0x04001196 RID: 4502
		protected const int E_RANK_RESTRICTED = 5;

		// Token: 0x04001197 RID: 4503
		protected readonly ISessionInfoService m_sessionInfoService;

		// Token: 0x04001198 RID: 4504
		protected readonly IDALService m_dalService;

		// Token: 0x04001199 RID: 4505
		protected readonly IProfileProgressionService m_profileProgressionService;

		// Token: 0x0400119A RID: 4506
		protected readonly IUserRepository m_userRepository;

		// Token: 0x0400119B RID: 4507
		protected readonly ILogService m_logService;

		// Token: 0x0400119C RID: 4508
		protected readonly IRankSystem m_rankSystem;

		// Token: 0x0400119D RID: 4509
		protected readonly IOnlineVariables m_onlineVariables;

		// Token: 0x0400119E RID: 4510
		protected readonly IPaymentService m_paymentService;

		// Token: 0x0400119F RID: 4511
		protected readonly IAbuseReportService m_abuseReportService;

		// Token: 0x040011A0 RID: 4512
		protected readonly IItemsPurchase m_itemsPurchase;

		// Token: 0x040011A1 RID: 4513
		protected readonly ISponsorUnlock m_sponsorUnlock;

		// Token: 0x040011A2 RID: 4514
		protected readonly IClientVersionsManagementService m_clientVersionsManagementService;

		// Token: 0x040011A3 RID: 4515
		protected readonly IRegionsService m_regionsService;
	}
}
