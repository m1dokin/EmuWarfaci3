using System;
using MasterServer.Core;

namespace MasterServer.Platform.Payment
{
	// Token: 0x02000698 RID: 1688
	[ConsoleCmdAttributes(CmdName = "payment_add_money", ArgsSize = 2, Help = "Add external money to user (user_id amount).")]
	internal class AddMoneyPaymentCommand : IConsoleCmd
	{
		// Token: 0x0600236E RID: 9070 RVA: 0x00095AA8 File Offset: 0x00093EA8
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			ulong amount = ulong.Parse(args[2]);
			IDebugPaymentService service = ServicesManager.GetService<IDebugPaymentService>();
			if (service != null)
			{
				service.AddMoney(num, amount);
				Log.Info<ulong>("Money has been added to player {0}.", num);
			}
			else
			{
				Log.Info("There is no external payment service.");
			}
		}
	}
}
