using System;
using System.Collections.Generic;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000180 RID: 384
	[ConsoleCmdAttributes(CmdName = "online_compress_xmpp", ArgsSize = 1)]
	internal class SetXMPPCompressionCmd : IConsoleCmd
	{
		// Token: 0x0600070A RID: 1802 RVA: 0x0001B454 File Offset: 0x00019854
		static SetXMPPCompressionCmd()
		{
			SetXMPPCompressionCmd.compres_map.Add("default", ECompressType.eCS_Default);
			SetXMPPCompressionCmd.compres_map.Add("none", ECompressType.eCS_NoCompression);
			SetXMPPCompressionCmd.compres_map.Add("0", ECompressType.eCS_NoCompression);
			SetXMPPCompressionCmd.compres_map.Add("force_none", ECompressType.eCS_ForceNoCompression);
			SetXMPPCompressionCmd.compres_map.Add("compress", ECompressType.eCS_Compress);
			SetXMPPCompressionCmd.compres_map.Add("1", ECompressType.eCS_Compress);
			SetXMPPCompressionCmd.compres_map.Add("smart", ECompressType.eCS_SmartCompress);
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x0001B4E4 File Offset: 0x000198E4
		public void ExecuteCmd(string[] args)
		{
			IOnlineClient service = ServicesManager.GetService<IOnlineClient>();
			if (args.Length == 1)
			{
				Log.Info<ECompressType>("online_compress_xmpp = {0}", service.GetDefaultCompression());
				return;
			}
			ECompressType defaultCompression;
			if (!SetXMPPCompressionCmd.compres_map.TryGetValue(args[1], out defaultCompression))
			{
				Log.Info("Invalid compression type");
			}
			else
			{
				service.SetDefaultCompression(defaultCompression);
			}
		}

		// Token: 0x0400042E RID: 1070
		private static Dictionary<string, ECompressType> compres_map = new Dictionary<string, ECompressType>();
	}
}
