using System;
using System.Globalization;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.Matchmaking.Data;
using Util.Common;

namespace MasterServer.Matchmaking.Queries
{
	// Token: 0x02000518 RID: 1304
	[QueryAttributes(TagName = "gameroom_quickplay_started")]
	internal class QuickPlayStartedQuery : BaseQuery
	{
		// Token: 0x06001C54 RID: 7252 RVA: 0x00071DBE File Offset: 0x000701BE
		public QuickPlayStartedQuery(IMissionGenerationService missionGenerationService, IMatchmakingSystem matchmakingSystem)
		{
			this.m_missionGenerationService = missionGenerationService;
			this.m_matchmakingSystem = matchmakingSystem;
		}

		// Token: 0x06001C55 RID: 7253 RVA: 0x00071DD4 File Offset: 0x000701D4
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			string value = (string)queryParams[0];
			MMEntityInfo mmentityInfo = (MMEntityInfo)queryParams[1];
			request.SetAttribute("bcast_receivers", value);
			request.SetAttribute("uid", mmentityInfo.Id.ToString(CultureInfo.InvariantCulture));
			request.SetAttribute("time_to_maps_reset_notification", ((int)this.m_matchmakingSystem.GetConfig().TimeToMapsResetNotification.TotalSeconds).ToString(CultureInfo.InvariantCulture));
			request.SetAttribute("timestamp", TimeUtils.LocalTimeToUTCTimestamp(DateTime.Now).ToString(CultureInfo.InvariantCulture));
			MissionHash missionsHash = this.m_missionGenerationService.GetMissionsHash();
			request.SetAttribute("mission_hash", missionsHash.missionHash);
			request.SetAttribute("content_hash", missionsHash.ContentHash);
		}

		// Token: 0x04000D8C RID: 3468
		public const string QueryName = "gameroom_quickplay_started";

		// Token: 0x04000D8D RID: 3469
		private readonly IMatchmakingSystem m_matchmakingSystem;

		// Token: 0x04000D8E RID: 3470
		private readonly IMissionGenerationService m_missionGenerationService;
	}
}
