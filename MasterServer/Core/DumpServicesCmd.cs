using System;
using System.IO;
using MasterServer.Core.Services;

namespace MasterServer.Core
{
	// Token: 0x02000152 RID: 338
	[ConsoleCmdAttributes(CmdName = "dump_services", ArgsSize = 0, Help = "dump all services in text file and console")]
	internal class DumpServicesCmd : IConsoleCmd
	{
		// Token: 0x060005DF RID: 1503 RVA: 0x000177F8 File Offset: 0x00015BF8
		public void ExecuteCmd(string[] args)
		{
			string path = Path.Combine(Resources.RootDir, "services_order.txt");
			File.Delete(path);
			foreach (IServiceModule serviceModule in ServicesManager.GetAllServices())
			{
				Log.Info<string>("{0}", serviceModule.Name);
				File.AppendAllText(path, string.Format("{0}\n", serviceModule.Name));
			}
		}
	}
}
