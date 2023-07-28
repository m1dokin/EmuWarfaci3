using System;
using MasterServer.Core;

namespace MasterServer.Platform.Payment
{
	// Token: 0x02000695 RID: 1685
	[ConsoleCmdAttributes(CmdName = "payment_set_money", ArgsSize = 2, Help = "Set external money to user (user_id amount).")]
	internal class SetMoneyPaymentCommand : IConsoleCmd
	{
		// Token: 0x06002368 RID: 9064 RVA: 0x00095970 File Offset: 0x00093D70
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			ulong amount = ulong.Parse(args[2]);
			IDebugPaymentService service = ServicesManager.GetService<IDebugPaymentService>();
			if (service != null)
			{
				service.SetMoney(num, amount);
				Log.Info<ulong>("Money has been set to player {0}.", num);
			}
			else
			{
				Log.Info("There is no external payment service.");
			}
		}
	}
}
