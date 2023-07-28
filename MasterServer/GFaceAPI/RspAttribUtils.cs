using System;

namespace MasterServer.GFaceAPI
{
	// Token: 0x02000655 RID: 1621
	public static class RspAttribUtils
	{
		// Token: 0x0600228F RID: 8847 RVA: 0x00090B50 File Offset: 0x0008EF50
		public static bool IsResponseType(Type t)
		{
			object[] customAttributes = t.GetCustomAttributes(typeof(GResponseAttribute), false);
			if (customAttributes.Length == 0)
			{
				return false;
			}
			if (customAttributes.Length > 1)
			{
				throw new RspAttribUtils.TooManyAttributesException(t);
			}
			return true;
		}

		// Token: 0x02000656 RID: 1622
		private class AttribException : ApplicationException
		{
			// Token: 0x06002290 RID: 8848 RVA: 0x00090B8A File Offset: 0x0008EF8A
			public AttribException(string msg) : base(msg)
			{
			}
		}

		// Token: 0x02000657 RID: 1623
		private class TooManyAttributesException : RspAttribUtils.AttribException
		{
			// Token: 0x06002291 RID: 8849 RVA: 0x00090B93 File Offset: 0x0008EF93
			public TooManyAttributesException(Type type) : base("There should be only one GFaceResponseTag per definition. type: " + type.FullName)
			{
			}
		}
	}
}
