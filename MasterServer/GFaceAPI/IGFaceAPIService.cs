using System;
using System.Threading.Tasks;
using HK2Net;
using HK2Net.Attributes.Bootstrap;

namespace MasterServer.GFaceAPI
{
	// Token: 0x02000647 RID: 1607
	[Contract]
	[BootstrapSpecific("west")]
	[BootstrapSpecific("west_emul")]
	public interface IGFaceAPIService
	{
		// Token: 0x0600222B RID: 8747
		TResult Request<TResult>(CallOptions opt, APIBundle api, params object[] args) where TResult : class;

		// Token: 0x0600222C RID: 8748
		void RequestAsync<TResult>(Action<TResult, Exception> responseCB, CallOptions opt, APIBundle api, params object[] args) where TResult : class;

		// Token: 0x0600222D RID: 8749
		Task<TResult> RequestAsync<TResult>(CallOptions opt, APIBundle api, params object[] args) where TResult : class;

		// Token: 0x0600222E RID: 8750
		bool Request(out GFaceError err, CallOptions opt, APIBundle api, params object[] args);

		// Token: 0x0600222F RID: 8751
		bool Request(CallOptions opt, APIBundle api, params object[] args);

		// Token: 0x06002230 RID: 8752
		void RequestAsync(Action<Exception> responseCB, CallOptions opt, APIBundle api, params object[] args);

		// Token: 0x06002231 RID: 8753
		Task RequestAsync(CallOptions opt, APIBundle api, params object[] args);
	}
}
