using System;
using System.Text;
using MasterServer.Core;
using Util.Common;

namespace MasterServer.Telemetry
{
	// Token: 0x020007D3 RID: 2003
	[ConsoleCmdAttributes(CmdName = "telemetry_mode", ArgsSize = 1)]
	internal class TelemetryModeCmd : IConsoleCmd
	{
		// Token: 0x06002906 RID: 10502 RVA: 0x000B1C20 File Offset: 0x000B0020
		public void ExecuteCmd(string[] args)
		{
			ITelemetryService service = ServicesManager.GetService<ITelemetryService>();
			if (args.Length == 1)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("telemetry_mode = ");
				stringBuilder.Append(ReflectionUtils.EnumFlagsToString<TelemetryMode>(service.Mode));
				stringBuilder.Append("\nAvailable modes: ");
				bool flag = true;
				foreach (string value in Enum.GetNames(typeof(TelemetryMode)))
				{
					if (!flag)
					{
						stringBuilder.Append(" | ");
					}
					stringBuilder.Append(value);
					flag = false;
				}
				Log.Info(stringBuilder.ToString());
			}
			else
			{
				TelemetryMode mode;
				if (!ReflectionUtils.EnumParseFlags<TelemetryMode>(args[1], out mode))
				{
					Log.Info("Invalid mode");
					return;
				}
				service.Mode = mode;
				Log.Info("telemetry_mode = " + ReflectionUtils.EnumFlagsToString<TelemetryMode>(service.Mode));
			}
		}
	}
}
