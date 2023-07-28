using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MasterServer.Core;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002E8 RID: 744
	[ConsoleCmdAttributes(CmdName = "gi_timeout", ArgsSize = 100, Help = "gi_timeout [command_1,...,command_N] [timeout]")]
	[ConsoleCmdAttributes(CmdName = "gi_throw", ArgsSize = 100, Help = "gi_throw [command_1,...,command_N] [should_throw = true|false]")]
	public class GiConfigureConsoleCmd : IConsoleCmd
	{
		// Token: 0x06001161 RID: 4449 RVA: 0x00044C68 File Offset: 0x00043068
		public void ExecuteCmd(string[] args)
		{
			if (args.Length < 2 || args.Length > 4)
			{
				Log.Info("Listing all GI commands configured to throw/timeout:");
				Predicate<ConsoleCmdAttributes> predicate = (ConsoleCmdAttributes a) => a.CmdName.StartsWith("gi_", StringComparison.OrdinalIgnoreCase);
				if (GiConfigureConsoleCmd.<>f__mg$cache0 == null)
				{
					GiConfigureConsoleCmd.<>f__mg$cache0 = new Func<ConsoleCmdManager.CommandEntity, string>(GiConfigureConsoleCmd.FormatCommand);
				}
				ConsoleCmdManager.Dump(predicate, GiConfigureConsoleCmd.<>f__mg$cache0);
			}
			else
			{
				HashSet<string> commands = new HashSet<string>(args[1].Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries).Except(new string[]
				{
					"gi_timeout",
					"gi_throw"
				}));
				if (args.Length < 3)
				{
					Log.Info("Listing GI commands throw/timeout configuration:");
					Predicate<ConsoleCmdAttributes> predicate2 = (ConsoleCmdAttributes a) => commands.Contains(a.CmdName);
					if (GiConfigureConsoleCmd.<>f__mg$cache1 == null)
					{
						GiConfigureConsoleCmd.<>f__mg$cache1 = new Func<ConsoleCmdManager.CommandEntity, string>(GiConfigureConsoleCmd.FormatCommand);
					}
					ConsoleCmdManager.Dump(predicate2, GiConfigureConsoleCmd.<>f__mg$cache1);
				}
				else
				{
					string text = args[0].ToLower();
					if (text != null)
					{
						if (!(text == "gi_timeout"))
						{
							if (text == "gi_throw")
							{
								bool shouldThrow = bool.Parse(args[2].ToLower());
								GameInterfaceCmd.SetGiCommandsShouldThrow(commands, shouldThrow);
							}
						}
						else
						{
							TimeSpan timeout = TimeSpan.FromMilliseconds((double)int.Parse(args[2]));
							GameInterfaceCmd.SetGiCommandsTimeout(commands, timeout);
						}
					}
				}
			}
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x00044DD0 File Offset: 0x000431D0
		private static string FormatCommand(ConsoleCmdManager.CommandEntity commandEntity)
		{
			string cmdName = commandEntity.Attributes.CmdName;
			bool flag = GameInterfaceCmd.ShouldGiCommandThrow(cmdName);
			TimeSpan? giCommandTimeout = GameInterfaceCmd.GetGiCommandTimeout(cmdName);
			return string.Format("{0} - throws: {1}; timeout: {2} ms", cmdName, (!flag) ? "No" : "Yes", (giCommandTimeout == null) ? 0.0 : giCommandTimeout.Value.TotalMilliseconds);
		}

		// Token: 0x040007A6 RID: 1958
		[CompilerGenerated]
		private static Func<ConsoleCmdManager.CommandEntity, string> <>f__mg$cache0;

		// Token: 0x040007A7 RID: 1959
		[CompilerGenerated]
		private static Func<ConsoleCmdManager.CommandEntity, string> <>f__mg$cache1;
	}
}
