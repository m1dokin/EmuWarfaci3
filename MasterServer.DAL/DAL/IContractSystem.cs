using System;

namespace MasterServer.DAL
{
	// Token: 0x02000022 RID: 34
	public interface IContractSystem
	{
		// Token: 0x06000052 RID: 82
		DALResult<ProfileContract> AddContract(ulong profileId, ulong rotationId, TimeSpan nextRotateTime);

		// Token: 0x06000053 RID: 83
		DALResult<ProfileContract> SetContractProgress(ulong profileId, uint progress);

		// Token: 0x06000054 RID: 84
		DALResult<ProfileContract> GetContractInfo(ulong profileId);

		// Token: 0x06000055 RID: 85
		DALResult<ProfileContract> ActivateContract(ulong profileId, ulong itemId, string itemName, uint progressTotal);

		// Token: 0x06000056 RID: 86
		DALResult<ProfileContract> DeactivateContract(ulong profileId);

		// Token: 0x06000057 RID: 87
		DALResultVoid SetContractInfo(ulong profileId, uint rotationId, ulong profileItemId, string contractName, uint currentProgress, uint totalProgress, TimeSpan localTimeToUtcTimestamp);
	}
}
