using System;
using MasterServer.Core;

namespace MasterServer.Platform.Payment
{
	// Token: 0x0200069A RID: 1690
	[ConsoleCmdAttributes(CmdName = "payment_request_timeout", ArgsSize = 1, Help = "Get\\Sets payment request timeout")]
	internal class PaymentRequestTimeoutCmd : IConsoleCmd
	{
		// Token: 0x06002372 RID: 9074 RVA: 0x00095B5C File Offset: 0x00093F5C
		public void ExecuteCmd(string[] args)
		{
			IDebugPaymentService service = ServicesManager.GetService<IDebugPaymentService>();
			if (service != null)
			{
				if (args.Length > 1)
				{
					service.SetRequestTimeout(TimeSpan.FromMilliseconds(uint.Parse(args[1])));
				}
				Log.Info<TimeSpan>("Debug Payment: request timeout is {0}", service.GetRequestTimeout());
			}
			else
			{
				Log.Warning("Can't find IDebugPaymentService to execute command.");
			}
		}
	}
}
