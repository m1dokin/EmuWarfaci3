using System;
using MasterServer.Core;
using OLAPHypervisor;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000026 RID: 38
	internal class MasterServerLogRedirect : ILogger
	{
		// Token: 0x06000198 RID: 408 RVA: 0x0000EEDD File Offset: 0x0000D0DD
		public void Error(Exception e)
		{
			Log.Error(e);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000EEE5 File Offset: 0x0000D0E5
		public void Warning(Exception e)
		{
			Log.Warning(e);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000EEED File Offset: 0x0000D0ED
		public void Error(string format, params object[] args)
		{
			Log.Error(format, args);
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000EEF6 File Offset: 0x0000D0F6
		public void Warning(string format, params object[] args)
		{
			Log.Warning(format, args);
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000EEFF File Offset: 0x0000D0FF
		public void Info(string format, params object[] args)
		{
			Log.Info(format, args);
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000EF08 File Offset: 0x0000D108
		public void Verbose(string format, params object[] args)
		{
			Log.Verbose(Log.Group.Telemetry, format, args);
		}
	}
}
