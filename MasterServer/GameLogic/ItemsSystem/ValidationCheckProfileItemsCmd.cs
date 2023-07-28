using System;
using MasterServer.Core;
using MasterServer.GameLogic.GameInterface;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000784 RID: 1924
	[ConsoleCmdAttributes(CmdName = "validation_check_profile", ArgsSize = 1)]
	internal class ValidationCheckProfileItemsCmd : IConsoleCmd
	{
		// Token: 0x060027E5 RID: 10213 RVA: 0x000AB115 File Offset: 0x000A9515
		public ValidationCheckProfileItemsCmd(IItemsValidator validator)
		{
			this.m_validator = validator;
		}

		// Token: 0x060027E6 RID: 10214 RVA: 0x000AB124 File Offset: 0x000A9524
		public void ExecuteCmd(string[] args)
		{
			ulong num = 0UL;
			foreach (ulong profileID in GameInterfaceCmd.GetProfiles(args[1]))
			{
				try
				{
					this.m_validator.CheckProfileItems(profileID);
				}
				catch (ValidationException e)
				{
					num += 1UL;
					Log.Error(e);
				}
			}
			Log.Info<ulong>("Failed profiles {0}", num);
		}

		// Token: 0x040014D8 RID: 5336
		private readonly IItemsValidator m_validator;
	}
}
