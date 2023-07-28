using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020000AE RID: 174
	[ConsoleCmdAttributes(CmdName = "set_restriction", Help = "set default value for restriction")]
	internal class SetGameModeRestrictionCmd : ConsoleCommand<SetGameModeRestrictionCmdParams>
	{
		// Token: 0x060002CE RID: 718 RVA: 0x0000DD05 File Offset: 0x0000C105
		public SetGameModeRestrictionCmd(IGameModesSystem gameModesSystem)
		{
			this.m_gameModesSystem = gameModesSystem;
		}

		// Token: 0x060002CF RID: 719 RVA: 0x0000DD14 File Offset: 0x0000C114
		protected override void Execute(SetGameModeRestrictionCmdParams param)
		{
			RoomRestrictionDesc restrictionDesc = this.m_gameModesSystem.GetRestrictionDesc(param.Restriction);
			restrictionDesc.SetDefault(param.Option, param.Value);
		}

		// Token: 0x04000131 RID: 305
		private readonly IGameModesSystem m_gameModesSystem;
	}
}
