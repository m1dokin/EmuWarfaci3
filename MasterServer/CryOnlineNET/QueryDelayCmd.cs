using System;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000199 RID: 409
	internal static class QueryDelayCmd
	{
		// Token: 0x060007B4 RID: 1972 RVA: 0x0001D9AC File Offset: 0x0001BDAC
		public static void ExecuteCmd(QueryDelayCmd.DelayType delay_type, string[] args)
		{
			IQueryManager service = ServicesManager.GetService<IQueryManager>();
			QueryDelay queryDelay = (delay_type != QueryDelayCmd.DelayType.Request) ? service.ResponseDelays : service.RequestDelays;
			switch (args.Length)
			{
			case 1:
				queryDelay.Dump();
				break;
			case 2:
				if (int.Parse(args[1]) == 0)
				{
					queryDelay.ResetAllDelays();
				}
				break;
			case 3:
				queryDelay.SetDelay(args[1], int.Parse(args[2]));
				break;
			default:
				Log.Error("Invalid number of arguments");
				break;
			}
		}

		// Token: 0x0200019A RID: 410
		public enum DelayType
		{
			// Token: 0x0400048B RID: 1163
			Request,
			// Token: 0x0400048C RID: 1164
			Response
		}
	}
}
