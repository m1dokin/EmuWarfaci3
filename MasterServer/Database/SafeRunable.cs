using System;
using MasterServer.Core;

namespace MasterServer.Database
{
	// Token: 0x02000826 RID: 2086
	public class SafeRunable
	{
		// Token: 0x06002B01 RID: 11009 RVA: 0x000B9D9C File Offset: 0x000B819C
		public static bool Run(SafeRunable.VoidDeleg deleg)
		{
			bool result;
			try
			{
				deleg();
				result = true;
			}
			catch (Exception e)
			{
				Log.Warning(e);
				result = false;
			}
			return result;
		}

		// Token: 0x02000827 RID: 2087
		// (Invoke) Token: 0x06002B03 RID: 11011
		public delegate void VoidDeleg();
	}
}
