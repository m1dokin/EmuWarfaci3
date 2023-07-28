using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020002FF RID: 767
	public class RestrictionHelper
	{
		// Token: 0x060011C0 RID: 4544 RVA: 0x00046360 File Offset: 0x00044760
		public static ERoomRestriction Parse(string kind)
		{
			ERoomRestriction result;
			try
			{
				result = (ERoomRestriction)Enum.Parse(typeof(ERoomRestriction), kind, true);
			}
			catch
			{
				Log.Warning<string>("[RestrictionHelper] Restriction config contains {0}, but it isn't supported", kind);
				throw;
			}
			return result;
		}
	}
}
