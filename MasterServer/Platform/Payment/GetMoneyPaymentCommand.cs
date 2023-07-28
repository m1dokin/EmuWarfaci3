using System;
using MasterServer.Core;

namespace MasterServer.Platform.Payment
{
	// Token: 0x02000694 RID: 1684
	[ConsoleCmdAttributes(CmdName = "payment_get_money", ArgsSize = 1, Help = "Get user's external money by user id.")]
	internal class GetMoneyPaymentCommand : IConsoleCmd
	{
		// Token: 0x06002366 RID: 9062 RVA: 0x00095920 File Offset: 0x00093D20
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			IPaymentService service = ServicesManager.GetService<IPaymentService>();
			if (service != null)
			{
				ulong money = service.GetMoney(num);
				Log.Info<ulong, ulong>("Player's {0} money is {1}.", num, money);
			}
			else
			{
				Log.Info("There is no external payment service.");
			}
		}
	}
}
