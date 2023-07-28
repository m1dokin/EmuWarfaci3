using System;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001F9 RID: 505
	public interface IContractSystemClient
	{
		// Token: 0x06000A1A RID: 2586
		ProfileContract AddContract(ulong profileId, ulong rotationId, TimeSpan nextRotateTime);

		// Token: 0x06000A1B RID: 2587
		ProfileContract SetContractProgress(ulong profileId, uint progress);

		// Token: 0x06000A1C RID: 2588
		ProfileContract GetContractInfo(ulong profileId);

		// Token: 0x06000A1D RID: 2589
		ProfileContract ActivateContract(ulong profileId, ulong itemId, string itemName, uint progressTotal);

		// Token: 0x06000A1E RID: 2590
		ProfileContract DeactivateContract(ulong profileId);

		// Token: 0x06000A1F RID: 2591
		void SetContractInfo(ulong profileId, uint rotationId, ulong profileItemId, string contractName, uint currentProgress, uint totalProgress, TimeSpan localTimeToUtcTimestamp);
	}
}
