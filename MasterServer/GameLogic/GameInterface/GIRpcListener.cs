using System;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002ED RID: 749
	internal class GIRpcListener : MarshalByRefObject, IGIRpcListener
	{
		// Token: 0x0600116F RID: 4463 RVA: 0x0004510F File Offset: 0x0004350F
		public GIRpcListener(GIRpcCallback callback)
		{
			this.m_callback = callback;
		}

		// Token: 0x06001170 RID: 4464 RVA: 0x0004511E File Offset: 0x0004351E
		public string Execute(string cmd)
		{
			return this.m_callback.Execute(cmd);
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x0004512C File Offset: 0x0004352C
		public override object InitializeLifetimeService()
		{
			return null;
		}

		// Token: 0x040007AE RID: 1966
		private GIRpcCallback m_callback;
	}
}
