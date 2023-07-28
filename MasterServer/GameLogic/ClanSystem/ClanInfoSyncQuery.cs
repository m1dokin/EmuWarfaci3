using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x0200026E RID: 622
	[QueryAttributes(TagName = "clan_info_sync")]
	internal class ClanInfoSyncQuery : BaseQuery
	{
		// Token: 0x06000D77 RID: 3447 RVA: 0x00035AE0 File Offset: 0x00033EE0
		public ClanInfoSyncQuery(IClanInfoUpdater clanInfoUpdater)
		{
			this.m_clanInfoUpdater = clanInfoUpdater;
		}

		// Token: 0x06000D78 RID: 3448 RVA: 0x00035AF0 File Offset: 0x00033EF0
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "SetClanInfoQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					this.m_clanInfoUpdater.RepairAndSendClanInfo(user.OnlineID, user.ProfileID);
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x04000637 RID: 1591
		private readonly IClanInfoUpdater m_clanInfoUpdater;
	}
}
