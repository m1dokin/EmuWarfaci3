using System;
using System.IO;
using System.Text;

namespace MasterServer.Core.LogSenders
{
	// Token: 0x02000121 RID: 289
	internal interface ILogSender : IDisposable
	{
		// Token: 0x060004B6 RID: 1206
		ILocker Lock();

		// Token: 0x060004B7 RID: 1207
		MemoryStream GetStream(Encoding encoding);

		// Token: 0x060004B8 RID: 1208
		void Flush(MemoryStream stream);
	}
}
