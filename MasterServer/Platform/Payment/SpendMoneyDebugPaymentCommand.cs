using System;
using MasterServer.Core;

namespace MasterServer.Platform.Payment
{
	// Token: 0x02000697 RID: 1687
	[ConsoleCmdAttributes(CmdName = "debug_payment_spend_money", ArgsSize = 2, Help = "Spend user's external money (user_id amount).")]
	internal class SpendMoneyDebugPaymentCommand : IConsoleCmd
	{
		// Token: 0x0600236C RID: 9068 RVA: 0x00095A38 File Offset: 0x00093E38
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			ulong num2 = ulong.Parse(args[2]);
			IDebugPaymentService service = ServicesManager.GetService<IDebugPaymentService>();
			if (service != null)
			{
				PaymentResult paymentResult = service.SpendMoney(num, num2);
				if (paymentResult == PaymentResult.Ok)
				{
					Log.Info<ulong>("Player's {0} money has been spent.", num);
				}
				else
				{
					Log.Info<ulong, PaymentResult>("Can't spend {0} money, error: {1}.", num2, paymentResult);
				}
			}
			else
			{
				Log.Info("There is no external payment service.");
			}
		}
	}
}
