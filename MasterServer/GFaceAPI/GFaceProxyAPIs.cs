using System;
using Network.Http;

namespace MasterServer.GFaceAPI
{
	// Token: 0x02000643 RID: 1603
	public static class GFaceProxyAPIs
	{
		// Token: 0x040010F6 RID: 4342
		public static APIBundle commerce_wallet_add = new APIBundle
		{
			Method = RequestMethod.POST,
			APIString = "commerce/wallet/add"
		};

		// Token: 0x040010F7 RID: 4343
		public static APIBundle commerce_wallet_set = new APIBundle
		{
			Method = RequestMethod.POST,
			APIString = "commerce/wallet/set"
		};
	}
}
