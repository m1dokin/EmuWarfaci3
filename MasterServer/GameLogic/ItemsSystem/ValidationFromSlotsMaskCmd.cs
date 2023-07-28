using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Core;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000786 RID: 1926
	[ConsoleCmdAttributes(CmdName = "validation_from_slots_mask", ArgsSize = 1)]
	internal class ValidationFromSlotsMaskCmd : IConsoleCmd
	{
		// Token: 0x060027EA RID: 10218 RVA: 0x000AB238 File Offset: 0x000A9638
		public void ExecuteCmd(string[] args)
		{
			int slotsMask = int.Parse(args[1]);
			HashSet<int> source = EquipCheck.DecodeSlotsMask(slotsMask);
			source.ToList<int>().ForEach(delegate(int e)
			{
				Log.Info<int>("Slot: {0}", e);
			});
		}
	}
}
