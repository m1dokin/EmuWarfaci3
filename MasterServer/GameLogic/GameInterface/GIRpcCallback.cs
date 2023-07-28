using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002EB RID: 747
	public class GIRpcCallback : MarshalByRefObject
	{
		// Token: 0x0600116C RID: 4460 RVA: 0x000450BC File Offset: 0x000434BC
		public string Execute(string cmd)
		{
			string result;
			try
			{
				string text = GameInterfaceCmd.Execute(AccessLevel.Admin, cmd);
				Log.Info<string, string>("[GIRpc] '{0}' : {1}", cmd, text);
				result = text;
			}
			catch (Exception ex)
			{
				Log.Warning<string, string>("[GIRpc] '{0}' error: {1} ", cmd, ex.Message);
				throw;
			}
			return result;
		}

		// Token: 0x0600116D RID: 4461 RVA: 0x0004510C File Offset: 0x0004350C
		public override object InitializeLifetimeService()
		{
			return null;
		}
	}
}
