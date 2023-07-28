using System;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.Matchmaking.Queries
{
	// Token: 0x02000773 RID: 1907
	[QueryAttributes(TagName = "gameroom_quickplay_reset_maps_settings")]
	internal class QuickPlayResetMapsSettingsQuery : BaseQuery
	{
		// Token: 0x06002795 RID: 10133 RVA: 0x000A90E7 File Offset: 0x000A74E7
		public QuickPlayResetMapsSettingsQuery(IMatchmakingSystem matchmakingSystem)
		{
			this.m_matchmakingSystem = matchmakingSystem;
		}

		// Token: 0x06002796 RID: 10134 RVA: 0x000A90F8 File Offset: 0x000A74F8
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User user;
			if (!base.GetClientInfo(fromJid, out user))
			{
				return -3;
			}
			this.m_matchmakingSystem.ResetMapsSettings(user.ProfileID);
			return 0;
		}

		// Token: 0x04001494 RID: 5268
		private readonly IMatchmakingSystem m_matchmakingSystem;
	}
}
