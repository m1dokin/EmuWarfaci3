using System;
using Network.Http.Builders;

namespace MasterServer.Core.Web
{
	// Token: 0x0200016C RID: 364
	internal static class MSHttpClientBuilderExtensions
	{
		// Token: 0x06000683 RID: 1667 RVA: 0x0001A664 File Offset: 0x00018A64
		public static IHttpClientBuilder Failover(this IHttpClientBuilder builder, Uri[] hostUrls)
		{
			return builder.Failover(hostUrls, Math.Abs(MSHttpClientBuilderExtensions.s_random.Next()));
		}

		// Token: 0x04000405 RID: 1029
		private static readonly Random s_random = new Random((int)DateTime.Now.Ticks);
	}
}
