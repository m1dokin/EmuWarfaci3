using System;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;

namespace MasterServer.GameLogic.ContractSystem
{
	// Token: 0x02000297 RID: 663
	[ConsoleCmdAttributes(CmdName = "contract_reset_item", ArgsSize = 1, Help = "Force reset contract for user")]
	internal class ContractResetRotationCmd : IConsoleCmd
	{
		// Token: 0x06000E64 RID: 3684 RVA: 0x0003A044 File Offset: 0x00038444
		public void ExecuteCmd(string[] args)
		{
			ulong profileId = ulong.Parse(args[1]);
			IDALService service = ServicesManager.GetService<IDALService>();
			IContractService service2 = ServicesManager.GetService<IContractService>();
			ProfileContract profileContract = service2.GetProfileContract(profileId);
			if (profileContract != null)
			{
				Log.Info<string>("Contract {0}", profileContract.ToString());
				service.ContractSystem.SetContractInfo(profileContract.ProfileId, profileContract.RotationId, 0UL, string.Empty, 0U, profileContract.TotalProgress, TimeSpan.Zero);
				profileContract = service2.GetProfileContract(profileId);
				Log.Info<string>("Contract: {0}", profileContract.ToString());
			}
			else
			{
				Log.Info("No old contract is available");
			}
		}
	}
}
