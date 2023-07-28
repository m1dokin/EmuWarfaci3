using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Core;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000785 RID: 1925
	[ConsoleCmdAttributes(CmdName = "validation_to_slots_mask")]
	internal class ValidationToSlotsMaskCmd : IConsoleCmd
	{
		// Token: 0x060027E8 RID: 10216 RVA: 0x000AB1C0 File Offset: 0x000A95C0
		public void ExecuteCmd(string[] args)
		{
			HashSet<int> excludeSlots = new HashSet<int>();
			args.Skip(1).ToList<string>().ForEach(delegate(string e)
			{
				excludeSlots.Add(int.Parse(e));
			});
			int p = EquipCheck.EncodeSlotsMask(excludeSlots);
			Log.Info<int>("Slots mask: {0}", p);
		}
	}
}
