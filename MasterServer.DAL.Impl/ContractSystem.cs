using System;

namespace MasterServer.DAL.Impl
{
	// Token: 0x0200000D RID: 13
	internal class ContractSystem : IContractSystem
	{
		// Token: 0x06000051 RID: 81 RVA: 0x00003FA0 File Offset: 0x000021A0
		public ContractSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003FBC File Offset: 0x000021BC
		public DALResult<ProfileContract> AddContract(ulong profileId, ulong rotationId, TimeSpan nextUpdateTime)
		{
			CacheProxy.Options<ProfileContract> options = new CacheProxy.Options<ProfileContract>
			{
				db_serializer = this.m_profileContractSerializer
			};
			options.query("CALL AddContract(?pid, ?rid, ?time)", new object[]
			{
				"?pid",
				profileId,
				"?rid",
				rotationId,
				"?time",
				(uint)Math.Ceiling(nextUpdateTime.TotalSeconds)
			});
			return this.m_dal.CacheProxy.Get<ProfileContract>(options);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00004040 File Offset: 0x00002240
		public DALResult<ProfileContract> ActivateContract(ulong profileId, ulong itemId, string itemName, uint progressTotal)
		{
			CacheProxy.Options<ProfileContract> options = new CacheProxy.Options<ProfileContract>
			{
				db_serializer = this.m_profileContractSerializer
			};
			options.query("CALL ActivateContract(?pid, ?cid, ?name, ?progress)", new object[]
			{
				"?pid",
				profileId,
				"?cid",
				itemId,
				"?name",
				itemName,
				"?progress",
				progressTotal
			});
			return this.m_dal.CacheProxy.Get<ProfileContract>(options);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000040C4 File Offset: 0x000022C4
		public DALResult<ProfileContract> SetContractProgress(ulong profileId, uint progress)
		{
			CacheProxy.Options<ProfileContract> options = new CacheProxy.Options<ProfileContract>
			{
				db_serializer = this.m_profileContractSerializer
			};
			options.query("CALL SetContractProgress(?pid, ?progress)", new object[]
			{
				"?pid",
				profileId,
				"?progress",
				progress
			});
			return this.m_dal.CacheProxy.Get<ProfileContract>(options);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x0000412C File Offset: 0x0000232C
		public DALResult<ProfileContract> GetContractInfo(ulong profileId)
		{
			CacheProxy.Options<ProfileContract> options = new CacheProxy.Options<ProfileContract>
			{
				db_serializer = this.m_profileContractSerializer
			};
			options.query("CALL GetContractInfo(?pid)", new object[]
			{
				"?pid",
				profileId
			});
			return this.m_dal.CacheProxy.Get<ProfileContract>(options);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00004180 File Offset: 0x00002380
		public DALResult<ProfileContract> DeactivateContract(ulong profileId)
		{
			CacheProxy.Options<ProfileContract> options = new CacheProxy.Options<ProfileContract>
			{
				db_serializer = this.m_profileContractSerializer
			};
			options.query("CALL DeactivateContract(?pid)", new object[]
			{
				"?pid",
				profileId
			});
			return this.m_dal.CacheProxy.Get<ProfileContract>(options);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000041D4 File Offset: 0x000023D4
		public DALResultVoid SetContractInfo(ulong profileId, uint rotationId, ulong profileItemId, string contractName, uint currentProgress, uint totalProgress, TimeSpan localTimeToUtcTimestamp)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL SetContractInfo(?pid, ?rid, ?pitem, ?name, ?progress_current, ?progress_total, ?rotation_time)", new object[]
			{
				"?pid",
				profileId,
				"?rid",
				rotationId,
				"?pitem",
				profileItemId,
				"?name",
				contractName,
				"?progress_current",
				currentProgress,
				"?progress_total",
				totalProgress,
				"?rotation_time",
				(uint)Math.Ceiling(localTimeToUtcTimestamp.TotalSeconds)
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x0400001C RID: 28
		private DAL m_dal;

		// Token: 0x0400001D RID: 29
		private ProfileContractSerializer m_profileContractSerializer = new ProfileContractSerializer();
	}
}
