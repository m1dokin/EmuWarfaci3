using System;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.Platform.Payment
{
	// Token: 0x02000696 RID: 1686
	[ConsoleCmdAttributes(CmdName = "payment_spend_money", ArgsSize = 2, Help = "Spend user's external money (user_id amount).")]
	internal class SpendMoneyPaymentCommand : IConsoleCmd
	{
		// Token: 0x0600236A RID: 9066 RVA: 0x000959C8 File Offset: 0x00093DC8
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			ulong num2 = ulong.Parse(args[2]);
			IPaymentService service = ServicesManager.GetService<IPaymentService>();
			if (service != null)
			{
				PaymentResult paymentResult = service.SpendMoney(num, num2, SpendMoneyReason.Debug);
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
