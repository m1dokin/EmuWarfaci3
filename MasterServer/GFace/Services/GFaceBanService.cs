using System;
using HK2Net;
using HK2Net.Attributes.Bootstrap;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.GameLogic.PunishmentSystem;
using MasterServer.GameLogic.PunishmentSystem.BanReporter;
using MasterServer.GFaceAPI;
using MasterServer.Users;

namespace MasterServer.GFace.Services
{
	// Token: 0x020004F2 RID: 1266
	[OrphanService]
	[Singleton]
	[BootstrapSpecific("west")]
	[BootstrapSpecific("west_emul")]
	internal class GFaceBanService : BanReporter
	{
		// Token: 0x06001B2D RID: 6957 RVA: 0x0006EEC0 File Offset: 0x0006D2C0
		public GFaceBanService(IGFaceAPIService gfaceApiService, IPunishmentService punishmentService, IUserRepository userRepository) : base(punishmentService)
		{
			this.m_gfaceApi = gfaceApiService;
			this.m_userRepository = userRepository;
			Config banRequestsConfig = Resources.BanRequestsConfig;
			this.m_gameId = banRequestsConfig.Get("game_id");
			this.m_serverId = banRequestsConfig.Get("server_id");
		}

		// Token: 0x06001B2E RID: 6958 RVA: 0x0006EF0C File Offset: 0x0006D30C
		protected override void ReportBan(ulong userId, DateTime expiresOn, string message)
		{
			userId = this.m_userRepository.UnmangleUserId(userId);
			int num = (int)(expiresOn - DateTime.Now).TotalSeconds;
			this.m_gfaceApi.Request(CallOptions.Reliable, GFaceAPIs.user_ban, new object[]
			{
				"user_id",
				userId,
				"time",
				num,
				"message",
				message,
				"token",
				this.m_gameId,
				"game_id",
				this.m_serverId,
				"server_id",
				ServerTokenPlaceHolder.Instance
			});
		}

		// Token: 0x06001B2F RID: 6959 RVA: 0x0006EFC0 File Offset: 0x0006D3C0
		protected override void ReportUnban(ulong userId)
		{
			userId = this.m_userRepository.UnmangleUserId(userId);
			this.m_gfaceApi.Request(CallOptions.Reliable, GFaceAPIs.user_unban, new object[]
			{
				"user_id",
				userId,
				"token",
				ServerTokenPlaceHolder.Instance
			});
		}

		// Token: 0x04000D01 RID: 3329
		private readonly IGFaceAPIService m_gfaceApi;

		// Token: 0x04000D02 RID: 3330
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000D03 RID: 3331
		private readonly string m_gameId;

		// Token: 0x04000D04 RID: 3332
		private readonly string m_serverId;
	}
}
