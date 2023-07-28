using System;

namespace MasterServer.Core
{
	// Token: 0x0200014C RID: 332
	internal class RConListener : MarshalByRefObject, IRConListener
	{
		// Token: 0x060005C3 RID: 1475 RVA: 0x00017208 File Offset: 0x00015608
		public RConListener(RConCallback callback)
		{
			this.m_callback = callback;
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x00017217 File Offset: 0x00015617
		public string ExecConsoleCmd(string cmd)
		{
			return this.m_callback.ExecCmd(cmd);
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x00017225 File Offset: 0x00015625
		public override object InitializeLifetimeService()
		{
			return null;
		}

		// Token: 0x040003BF RID: 959
		private RConCallback m_callback;
	}
}
