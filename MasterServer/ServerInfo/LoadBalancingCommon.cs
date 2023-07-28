using System;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006CB RID: 1739
	internal static class LoadBalancingCommon
	{
		// Token: 0x06002499 RID: 9369 RVA: 0x000990C9 File Offset: 0x000974C9
		public static string GetLoadBalancingRouteDefault(string xmppHost)
		{
			return string.Format("k01.{0}.servers", xmppHost);
		}

		// Token: 0x0600249A RID: 9370 RVA: 0x000990D6 File Offset: 0x000974D6
		public static LoadBalancingTypes ToLoadBalancingType(string src)
		{
			return (LoadBalancingTypes)Enum.Parse(typeof(LoadBalancingTypes), src, true);
		}

		// Token: 0x04001269 RID: 4713
		public const string LB_SECTION_NAME = "LoadBalancing";

		// Token: 0x0400126A RID: 4714
		public const string LB_TYPE_ATTRIBUTE_NAME = "type";

		// Token: 0x0400126B RID: 4715
		public const string LB_ROUTE_ATTRIBUTE_NAME = "global_lbs_route";

		// Token: 0x0400126C RID: 4716
		public const string LB_MAX_PER_HOST_ATTRIBUTE_NAME = "max_per_host";

		// Token: 0x0400126D RID: 4717
		public const string LB_MIN_PVE_ATTRIBUTE_NAME = "min_pve";

		// Token: 0x0400126E RID: 4718
		public const string LB_MIN_PVP_ATTRIBUTE_NAME = "min_pvp";

		// Token: 0x0400126F RID: 4719
		public const string LB_TIMEOUT_ATTRIBUTE_NAME = "timeout_sec";

		// Token: 0x04001270 RID: 4720
		public const string LB_LDS_NODE_SEARCH_RANGE_ATTRIBUTE_NAME = "lds_node_search_range";

		// Token: 0x04001271 RID: 4721
		public const string LB_LDS_NODE_RECONNECT_ENABLED_ATTRIBUTE_NAME = "lds_node_reconnect_enabled";
	}
}
