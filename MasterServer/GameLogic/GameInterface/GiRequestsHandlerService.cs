using System;
using System.Threading.Tasks;
using HK2Net;
using Network.Interfaces;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002EF RID: 751
	[Service]
	[Singleton]
	internal class GiRequestsHandlerService : IGiController, IRemoteService<GiCommandRequest, GiCommandResponse>, IDisposable
	{
		// Token: 0x06001172 RID: 4466 RVA: 0x0004512F File Offset: 0x0004352F
		public GiRequestsHandlerService()
		{
			this.m_callback = new GIRpcCallback();
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x00045144 File Offset: 0x00043544
		public Task<GiCommandResponse> MakeRequest(GiCommandRequest request)
		{
			Console.WriteLine("GIRpc request: {0}", request.Text);
			return Task.Factory.StartNew<GiCommandResponse>(() => new GiCommandResponse
			{
				Text = this.m_callback.Execute(request.Text)
			});
		}

		// Token: 0x06001174 RID: 4468 RVA: 0x00045190 File Offset: 0x00043590
		public void Dispose()
		{
		}

		// Token: 0x040007AF RID: 1967
		private readonly GIRpcCallback m_callback;
	}
}
