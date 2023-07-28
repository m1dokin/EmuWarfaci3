using System;
using System.Globalization;
using System.Xml;
using CommandLine;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameRoom.Commands.Debug;
using MasterServer.GameRoomSystem;
using MasterServer.Users;
using NLog;

namespace MasterServer.GameRoom
{
	// Token: 0x02000012 RID: 18
	[QueryAttributes(TagName = "debug_create_room")]
	internal class DebugCreateRoomQuery : BaseQuery
	{
		// Token: 0x06000049 RID: 73 RVA: 0x00005453 File Offset: 0x00003853
		public DebugCreateRoomQuery(IDebugGameRoomActivator roomActivator, IMissionSystem missionSystem, IProfileProgressionService profileProgression, IUserRepository userRepository, IRankSystem rankSystem)
		{
			this.m_roomActivator = roomActivator;
			this.m_missionSystem = missionSystem;
			this.m_profileProgression = profileProgression;
			this.m_userRepository = userRepository;
			this.m_rankSystem = rankSystem;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00005480 File Offset: 0x00003880
		public override int QueryGetResponse(string from, XmlElement request, XmlElement response)
		{
			if (!Resources.DebugQueriesEnabled)
			{
				return -1;
			}
			string attribute = request.GetAttribute("command_line");
			string[] args = ConsoleCmdManager.ParseCmdLine(attribute);
			DebugCreateRoomsCmdParams debugCreateRoomsCmdParams = new DebugCreateRoomsCmdParams();
			if (!Parser.Default.ParseArguments(args, debugCreateRoomsCmdParams))
			{
				return -1;
			}
			try
			{
				ulong num = DebugRoomHelpers.CreateRoom(this.m_roomActivator, this.m_missionSystem, this.m_profileProgression, this.m_userRepository, this.m_rankSystem, debugCreateRoomsCmdParams);
				if (num == 0UL)
				{
					DebugCreateRoomQuery.Log.Error("Failed to create fake room");
					return -2;
				}
				response.SetAttribute("room_id", num.ToString(CultureInfo.InvariantCulture));
			}
			catch (InvalidOperationException value)
			{
				DebugCreateRoomQuery.Log.Error<InvalidOperationException>(value);
				return -2;
			}
			return 0;
		}

		// Token: 0x04000021 RID: 33
		public const string QueryName = "debug_create_room";

		// Token: 0x04000022 RID: 34
		private const int InvalidCommandLineArgs = -1;

		// Token: 0x04000023 RID: 35
		private const int RoomCreationFailed = -2;

		// Token: 0x04000024 RID: 36
		private static ILogger Log = LogManager.GetCurrentClassLogger();

		// Token: 0x04000025 RID: 37
		private readonly IDebugGameRoomActivator m_roomActivator;

		// Token: 0x04000026 RID: 38
		private readonly IMissionSystem m_missionSystem;

		// Token: 0x04000027 RID: 39
		private readonly IProfileProgressionService m_profileProgression;

		// Token: 0x04000028 RID: 40
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000029 RID: 41
		private readonly IRankSystem m_rankSystem;
	}
}
