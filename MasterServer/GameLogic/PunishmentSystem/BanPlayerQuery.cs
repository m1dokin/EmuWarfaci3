using System;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.PunishmentSystem
{
	// Token: 0x02000405 RID: 1029
	[QueryAttributes(TagName = "ban_player")]
	internal class BanPlayerQuery : SessionQuery
	{
		// Token: 0x06001645 RID: 5701 RVA: 0x0005DE6E File Offset: 0x0005C26E
		public BanPlayerQuery(IPunishmentService punishmentService, ISessionStorage sessionStorage, IGameRoomManager roomManager) : base(sessionStorage)
		{
			this.m_punishmentService = punishmentService;
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x0005DE80 File Offset: 0x0005C280
		protected override int QueryGetResponseImpl(string fromJid, XmlElement request, XmlElement response)
		{
			ulong profileId = ulong.Parse(request.GetAttribute("profile_id"));
			TimeSpan time = TimeSpan.FromSeconds(ulong.Parse(request.GetAttribute("seconds")));
			string message = request.GetAttribute("message").ToString();
			this.m_punishmentService.BanPlayer(profileId, time, message, BanReportSource.Anticheat);
			return 0;
		}

		// Token: 0x04000AD1 RID: 2769
		private readonly IPunishmentService m_punishmentService;
	}
}
