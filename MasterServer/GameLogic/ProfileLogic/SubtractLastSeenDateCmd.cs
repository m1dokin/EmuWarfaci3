using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200054F RID: 1359
	[ConsoleCmdAttributes(CmdName = "subtract_last_seen_date", ArgsSize = 2, Help = "Args: ProfileId, DaysToSubstract")]
	internal class SubtractLastSeenDateCmd : IConsoleCmd
	{
		// Token: 0x06001D4C RID: 7500 RVA: 0x00076770 File Offset: 0x00074B70
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			int num2 = int.Parse(args[2]);
			IColdStorageService service = ServicesManager.GetService<IColdStorageService>();
			DateTime dateTime = service.DebugUpdateLastSeenDate(num, DateTime.Now.AddDays((double)(-(double)num2)));
			if (dateTime != DateTime.MinValue)
			{
				Log.Info<ulong, DateTime>("Profile {0}'s last seen date has been set to {1}", num, dateTime);
			}
		}
	}
}
