using System;
using CommandLine;

namespace MasterServer.Core
{
	// Token: 0x0200079C RID: 1948
	internal abstract class ConsoleCommand<T> : IConsoleCmd where T : new()
	{
		// Token: 0x06002852 RID: 10322 RVA: 0x000078A8 File Offset: 0x00005CA8
		public void ExecuteCmd(string[] args)
		{
			T t = Activator.CreateInstance<T>();
			if (!Parser.Default.ParseArguments(args, t))
			{
				throw new ArgumentException("Can't parse command line parameters.", "args");
			}
			this.Execute(t);
		}

		// Token: 0x06002853 RID: 10323
		protected abstract void Execute(T param);
	}
}
