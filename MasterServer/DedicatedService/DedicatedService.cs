using System;
using System.Threading.Tasks;
using DedicatedPoolServer.Model;
using HK2Net;
using MasterServer.Core.Services;
using Network.Amqp.Interfaces;
using Network.Interfaces;

namespace MasterServer.DedicatedService
{
	// Token: 0x020006B8 RID: 1720
	[Service]
	[Singleton]
	public class DedicatedService : ServiceModule, IDedicatedService
	{
		// Token: 0x06002414 RID: 9236 RVA: 0x00096F7C File Offset: 0x0009537C
		public DedicatedService(IRpc rpc, ITypeNameSerializer typeNameSerializer)
		{
			this.m_rpc = rpc;
			typeNameSerializer.AddMappingFor<DedicatedLockRequest>();
			typeNameSerializer.AddMappingFor<DedicatedUnlockRequest>();
			typeNameSerializer.AddMappingFor<DedicatedControlResponse>();
		}

		// Token: 0x06002415 RID: 9237 RVA: 0x00096FA0 File Offset: 0x000953A0
		public Task<DedicatedInfo> LockDedicatedAsync(DedicatedFilter filter)
		{
			DedicatedLockRequest request = new DedicatedLockRequest
			{
				Filter = filter
			};
			return this.m_rpc.Request<DedicatedLockRequest, DedicatedControlResponse>(request).ContinueWith<DedicatedInfo>((Task<DedicatedControlResponse> t) => t.Result.Info);
		}

		// Token: 0x06002416 RID: 9238 RVA: 0x00096FEC File Offset: 0x000953EC
		public Task UnlockDedicatedAsync(string dedicatedId, string masterId)
		{
			DedicatedUnlockRequest request = new DedicatedUnlockRequest
			{
				DedicatedId = dedicatedId,
				MasterId = masterId
			};
			return this.m_rpc.Request<DedicatedUnlockRequest, DedicatedControlResponse>(request);
		}

		// Token: 0x04001218 RID: 4632
		private readonly IRpc m_rpc;
	}
}
