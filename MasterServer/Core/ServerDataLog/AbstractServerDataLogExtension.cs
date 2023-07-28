using System;

namespace MasterServer.Core.ServerDataLog
{
	// Token: 0x02000129 RID: 297
	internal abstract class AbstractServerDataLogExtension : IServerDataLogExtension, IDisposable
	{
		// Token: 0x060004DE RID: 1246 RVA: 0x00014FF6 File Offset: 0x000133F6
		protected AbstractServerDataLogExtension(ILogService logService, bool enable)
		{
			this.LogService = logService;
			this.m_isEnable = enable;
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x0001500C File Offset: 0x0001340C
		public virtual void Start()
		{
			this.OnDataUpdated();
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x00015014 File Offset: 0x00013414
		public void Enable(bool enable)
		{
			if (!this.m_isEnable && enable)
			{
				this.LogData();
			}
			this.m_isEnable = enable;
		}

		// Token: 0x060004E1 RID: 1249
		public abstract void Dispose();

		// Token: 0x060004E2 RID: 1250
		protected abstract void LogData();

		// Token: 0x060004E3 RID: 1251 RVA: 0x00015034 File Offset: 0x00013434
		protected void OnDataUpdated()
		{
			if (this.m_isEnable)
			{
				this.LogData();
			}
		}

		// Token: 0x04000209 RID: 521
		protected readonly ILogService LogService;

		// Token: 0x0400020A RID: 522
		private bool m_isEnable;
	}
}
