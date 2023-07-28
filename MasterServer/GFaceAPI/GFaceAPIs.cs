using System;
using Network.Http;

namespace MasterServer.GFaceAPI
{
	// Token: 0x02000642 RID: 1602
	public static class GFaceAPIs
	{
		// Token: 0x040010E2 RID: 4322
		public static APIBundle auth_login = new APIBundle
		{
			Method = RequestMethod.POST,
			APIString = "/auth/login"
		};

		// Token: 0x040010E3 RID: 4323
		public static APIBundle auth_login_server = new APIBundle
		{
			Method = RequestMethod.POST,
			APIString = "/auth/login/server"
		};

		// Token: 0x040010E4 RID: 4324
		public static APIBundle auth_logout = new APIBundle
		{
			Method = RequestMethod.POST,
			APIString = "/auth/logout"
		};

		// Token: 0x040010E5 RID: 4325
		public static APIBundle auth_session_check = new APIBundle
		{
			Method = RequestMethod.GET,
			APIString = "/auth/session/check"
		};

		// Token: 0x040010E6 RID: 4326
		public static APIBundle wallet_get = new APIBundle
		{
			Method = RequestMethod.GET,
			APIString = "/commerce/wallet/get"
		};

		// Token: 0x040010E7 RID: 4327
		public static APIBundle wallet_get_my = new APIBundle
		{
			Method = RequestMethod.GET,
			APIString = "/commerce/wallet/get/my"
		};

		// Token: 0x040010E8 RID: 4328
		public static APIBundle purchase_ext_log = new APIBundle
		{
			Method = RequestMethod.POST,
			APIString = "/commerce/purchase/ext/log"
		};

		// Token: 0x040010E9 RID: 4329
		public static APIBundle wallet_points_add = new APIBundle
		{
			Method = RequestMethod.POST,
			APIString = "/commerce/wallet/points/add"
		};

		// Token: 0x040010EA RID: 4330
		public static APIBundle peoplecloud_member_get_all = new APIBundle
		{
			Method = RequestMethod.GET,
			APIString = "/peoplecloud/member/get/all"
		};

		// Token: 0x040010EB RID: 4331
		public static APIBundle socialconnect_create = new APIBundle
		{
			Method = RequestMethod.POST,
			APIString = "/socialconnect/create"
		};

		// Token: 0x040010EC RID: 4332
		public static APIBundle seed_get = new APIBundle
		{
			Method = RequestMethod.GET,
			APIString = "/seed/get"
		};

		// Token: 0x040010ED RID: 4333
		public static APIBundle channel_create = new APIBundle
		{
			Method = RequestMethod.POST,
			APIString = "channel/create"
		};

		// Token: 0x040010EE RID: 4334
		public static APIBundle channel_close = new APIBundle
		{
			Method = RequestMethod.POST,
			APIString = "channel/close"
		};

		// Token: 0x040010EF RID: 4335
		public static APIBundle channel_livesession_attach = new APIBundle
		{
			Method = RequestMethod.POST,
			APIString = "channel/livesession/attach"
		};

		// Token: 0x040010F0 RID: 4336
		public static APIBundle channel_livesession_detach = new APIBundle
		{
			Method = RequestMethod.POST,
			APIString = "channel/livesession/detach"
		};

		// Token: 0x040010F1 RID: 4337
		public static APIBundle channel_user_add = new APIBundle
		{
			Method = RequestMethod.POST,
			APIString = "channel/user/add"
		};

		// Token: 0x040010F2 RID: 4338
		public static APIBundle channel_user_remove = new APIBundle
		{
			Method = RequestMethod.POST,
			APIString = "channel/user/remove"
		};

		// Token: 0x040010F3 RID: 4339
		public static APIBundle user_get = new APIBundle
		{
			Method = RequestMethod.GET,
			APIString = "/user/get"
		};

		// Token: 0x040010F4 RID: 4340
		public static APIBundle user_ban = new APIBundle
		{
			Method = RequestMethod.POST,
			APIString = "/user/ban"
		};

		// Token: 0x040010F5 RID: 4341
		public static APIBundle user_unban = new APIBundle
		{
			Method = RequestMethod.POST,
			APIString = "/user/unban"
		};
	}
}
