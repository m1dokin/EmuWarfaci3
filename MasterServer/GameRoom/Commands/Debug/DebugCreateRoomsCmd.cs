using System;
using MasterServer.Core;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x0200045D RID: 1117
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "debug_create_room", Help = "Create room filled with fake players.")]
	internal class DebugCreateRoomsCmd : ConsoleCommand<DebugCreateRoomsCmdParams>
	{
		// Token: 0x0600179C RID: 6044 RVA: 0x000627C5 File Offset: 0x00060BC5
		public DebugCreateRoomsCmd(IDebugGameRoomActivator roomActivator, IMissionSystem missionSystem, IProfileProgressionService profileProgression, IUserRepository userRepository, IRankSystem rankSystem)
		{
			this.m_roomActivator = roomActivator;
			this.m_missionSystem = missionSystem;
			this.m_profileProgression = profileProgression;
			this.m_userRepository = userRepository;
			this.m_rankSystem = rankSystem;
		}

		// Token: 0x0600179D RID: 6045 RVA: 0x000627F4 File Offset: 0x00060BF4
		protected override void Execute(DebugCreateRoomsCmdParams param)
		{
			try
			{
				ulong num = DebugRoomHelpers.CreateRoom(this.m_roomActivator, this.m_missionSystem, this.m_profileProgression, this.m_userRepository, this.m_rankSystem, param);
				if (num == 0UL)
				{
					Log.Error("Failed to create fake room");
				}
				else
				{
					Log.Info(string.Format("Fake room {0} created", num));
				}
			}
			catch (InvalidOperationException ex)
			{
				Log.Info(ex.Message);
			}
		}

		// Token: 0x04000B63 RID: 2915
		private readonly IDebugGameRoomActivator m_roomActivator;

		// Token: 0x04000B64 RID: 2916
		private readonly IMissionSystem m_missionSystem;

		// Token: 0x04000B65 RID: 2917
		private readonly IProfileProgressionService m_profileProgression;

		// Token: 0x04000B66 RID: 2918
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000B67 RID: 2919
		private readonly IRankSystem m_rankSystem;
	}
}
