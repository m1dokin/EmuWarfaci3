using System;
using MasterServer.Core;
using MasterServer.Database;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000547 RID: 1351
	[ConsoleCmdAttributes(CmdName = "create_profiles", ArgsSize = 5)]
	internal class CreateProfilesCmd : IConsoleCmd
	{
		// Token: 0x06001D2D RID: 7469 RVA: 0x00075FFA File Offset: 0x000743FA
		public CreateProfilesCmd(IDALService dalService)
		{
			this.m_dalService = dalService;
		}

		// Token: 0x06001D2E RID: 7470 RVA: 0x0007600C File Offset: 0x0007440C
		public void ExecuteCmd(string[] args)
		{
			if (args.Length != 5)
			{
				Log.Error("Incorrect usage: create_profiles prefix, offset, number, <addr;db;user;pwd>");
				return;
			}
			string str = args[1];
			uint num = uint.Parse(args[2]);
			uint num2 = uint.Parse(args[3]);
			string csb = args[4];
			for (uint num3 = num; num3 < num + num2; num3 += 1U)
			{
				string text = str + num3.ToString();
				ulong num4 = this.m_dalService.ProfileSystem.CreateUser(csb, text, text, "autobot@crytek.de");
				if (num4 == 0UL)
				{
					Log.Warning<string>("User {0} already exist", text);
				}
				else
				{
					ulong num5 = this.m_dalService.ProfileSystem.CreateProfile(num4, text, string.Empty);
					this.m_dalService.ProfileSystem.CreateProfile(num5, num4, text);
					DefaultProfile.ResetProfileItems(num5, DefaultProfile.ResetType.ResetToDefault);
					Log.Info<ulong, string, string>("User with profile id - {0} created, nickname:{1} password:{2}", num5, text, text);
				}
			}
		}

		// Token: 0x04000DF6 RID: 3574
		private readonly IDALService m_dalService;
	}
}
