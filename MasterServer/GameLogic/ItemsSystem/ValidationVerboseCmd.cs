using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000783 RID: 1923
	[ConsoleCmdAttributes(CmdName = "validation_verbose", ArgsSize = 1)]
	internal class ValidationVerboseCmd : IConsoleCmd
	{
		// Token: 0x060027E3 RID: 10211 RVA: 0x000AB0D2 File Offset: 0x000A94D2
		public ValidationVerboseCmd(IItemsValidator validator)
		{
			this.m_validator = validator;
		}

		// Token: 0x060027E4 RID: 10212 RVA: 0x000AB0E1 File Offset: 0x000A94E1
		public void ExecuteCmd(string[] args)
		{
			if (args.Length == 1)
			{
				Log.Info<int>("validation_verbose = {0}", this.m_validator.ValidationVerbose);
				return;
			}
			this.m_validator.ValidationVerbose = int.Parse(args[1]);
		}

		// Token: 0x040014D7 RID: 5335
		private readonly IItemsValidator m_validator;
	}
}
