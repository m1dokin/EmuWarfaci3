using System;
using MasterServer.Core;

namespace MasterServer.Platform.Payment
{
	// Token: 0x02000699 RID: 1689
	[ConsoleCmdAttributes(CmdName = "payment_request_failed", ArgsSize = 1, Help = "Get\\Sets payment request failed state")]
	internal class PaymentRequestFailedStateCmd : IConsoleCmd
	{
		// Token: 0x06002370 RID: 9072 RVA: 0x00095B00 File Offset: 0x00093F00
		public void ExecuteCmd(string[] args)
		{
			IDebugPaymentService service = ServicesManager.GetService<IDebugPaymentService>();
			if (service != null)
			{
				if (args.Length > 1)
				{
					service.SetRequestFailedState(args[1] == "1");
				}
				Log.Info<bool>("Debug Payment: request failed state is {0}", service.GetRequestFailedState());
			}
			else
			{
				Log.Warning("Can't find IDebugPaymentService to execute command.");
			}
		}
	}
}
