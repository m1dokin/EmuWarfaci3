using System;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;

namespace MasterServer.GameLogic.ContractSystem
{
	// Token: 0x02000296 RID: 662
	[ConsoleCmdAttributes(CmdName = "get_next_rotation", ArgsSize = 1, Help = "Force change rotation to next")]
	internal class ContractGetNextRotationCmd : IConsoleCmd
	{
		// Token: 0x06000E62 RID: 3682 RVA: 0x00039F8C File Offset: 0x0003838C
		public void ExecuteCmd(string[] args)
		{
			ulong profileId = ulong.Parse(args[1]);
			IDALService service = ServicesManager.GetService<IDALService>();
			IContractService service2 = ServicesManager.GetService<IContractService>();
			ProfileContract profileContract = service2.GetProfileContract(profileId);
			if (profileContract != null)
			{
				Log.Info<uint>("Old rotaion id {0}", profileContract.RotationId);
				service.ContractSystem.SetContractInfo(profileContract.ProfileId, profileContract.RotationId, profileContract.ProfileItemId, profileContract.ContractName, profileContract.CurrentProgress, profileContract.TotalProgress, TimeSpan.Zero);
			}
			else
			{
				Log.Info("No old contract is available");
			}
			profileContract = service2.RotateContract(profileId);
			if (profileContract != null)
			{
				Log.Info<uint>("New rotaion id {0}", profileContract.RotationId);
			}
			else
			{
				Log.Info("No new contract is available");
			}
		}
	}
}
