using System;
using System.Globalization;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.AntiCheat;
using MasterServer.GameRoomSystem;
using MasterServer.Telemetry;
using MasterServer.Users;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x02000796 RID: 1942
	[QueryAttributes(TagName = "mission_load")]
	internal class MissionLoadQuery : BaseQuery
	{
		// Token: 0x06002846 RID: 10310 RVA: 0x000AD738 File Offset: 0x000ABB38
		public MissionLoadQuery(IAntiCheatService antiCheatService, IMissionSystem missionSystem, IServerRepository serverRepository, IGameRoomServer gameRoomServer, IVerbosityControlService verbosityControlService, IOnlineVariables onlineVariables)
		{
			this.m_antiCheatService = antiCheatService;
			this.m_missionSystem = missionSystem;
			this.m_serverRepository = serverRepository;
			this.m_gameRoomServer = gameRoomServer;
			this.m_onlineVariables = onlineVariables;
			this.m_verbosityControlService = verbosityControlService;
		}

		// Token: 0x06002847 RID: 10311 RVA: 0x000AD770 File Offset: 0x000ABB70
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			IGameRoom gameRoom = (IGameRoom)queryParams[0];
			string value = (string)queryParams[1];
			XmlElement newChild = gameRoom.FullStateSnapshot(RoomUpdate.Target.Server, request.OwnerDocument);
			request.AppendChild(newChild);
			this.m_antiCheatService.WriteConfig(request);
			string missionKey = string.Empty;
			gameRoom.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				missionKey = r.MissionKey;
			});
			MissionContext mission = this.m_missionSystem.GetMission(missionKey);
			VerbosityLevel verbosityLevel = this.m_verbosityControlService.GetVerbosityLevel(mission.gameMode, mission.difficulty, missionKey);
			request.SetAttribute("bootstrap_mode", (!Resources.BootstrapMode) ? "0" : "1");
			request.SetAttribute("bootstrap_name", Resources.BootstrapName);
			request.SetAttribute("session_id", value);
			string name = "verbosity_level";
			int num = (int)verbosityLevel;
			request.SetAttribute(name, num.ToString(CultureInfo.InvariantCulture));
			XmlElement xmlElement = request.OwnerDocument.CreateElement("online_variables");
			this.m_onlineVariables.WriteVariables(xmlElement, (!mission.IsPvPMode()) ? OnlineVariableDestination.Session_PvE : OnlineVariableDestination.Session_PvP);
			request.AppendChild(xmlElement);
		}

		// Token: 0x06002848 RID: 10312 RVA: 0x000AD8A0 File Offset: 0x000ABCA0
		public override object HandleResponse(SOnlineQuery query, XmlElement response)
		{
			string serverID = this.m_serverRepository.GetServerID(query.online_id);
			string attribute = response.GetAttribute("load_result");
			string attribute2 = response.GetAttribute("session_id");
			MissionLoadResult result;
			if (attribute != null)
			{
				if (attribute == "success")
				{
					result = MissionLoadResult.MLR_OK;
					goto IL_6A;
				}
				if (attribute == "not_owner")
				{
					result = MissionLoadResult.MLR_NOT_OWER;
					goto IL_6A;
				}
			}
			result = MissionLoadResult.MLR_FAILED;
			IL_6A:
			this.m_gameRoomServer.OnMissionLoad(serverID, result, attribute2);
			return null;
		}

		// Token: 0x06002849 RID: 10313 RVA: 0x000AD926 File Offset: 0x000ABD26
		public override void OnQueryError(SQueryError error)
		{
			this.m_gameRoomServer.OnMissionLoad(this.m_serverRepository.GetServerID(error.online_id), MissionLoadResult.MLR_FAILED, null);
		}

		// Token: 0x0400151F RID: 5407
		private readonly IAntiCheatService m_antiCheatService;

		// Token: 0x04001520 RID: 5408
		private readonly IMissionSystem m_missionSystem;

		// Token: 0x04001521 RID: 5409
		private readonly IServerRepository m_serverRepository;

		// Token: 0x04001522 RID: 5410
		private readonly IGameRoomServer m_gameRoomServer;

		// Token: 0x04001523 RID: 5411
		private readonly IVerbosityControlService m_verbosityControlService;

		// Token: 0x04001524 RID: 5412
		private readonly IOnlineVariables m_onlineVariables;
	}
}
