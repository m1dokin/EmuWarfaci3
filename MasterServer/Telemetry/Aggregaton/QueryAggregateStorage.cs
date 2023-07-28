using System;
using System.Collections.Generic;
using MasterServer.CryOnlineNET;
using MasterServer.Database;

namespace MasterServer.Telemetry.Aggregaton
{
	// Token: 0x02000728 RID: 1832
	internal class QueryAggregateStorage
	{
		// Token: 0x060025FE RID: 9726 RVA: 0x0009FAD4 File Offset: 0x0009DED4
		public void AddToAggregates(QueryContext ctx)
		{
			QueryAggregate queryAggregate;
			if (!this.m_queryAggregates.TryGetValue(ctx.Tag, out queryAggregate))
			{
				queryAggregate = new QueryAggregate();
				this.m_queryAggregates.Add(ctx.Tag, queryAggregate);
			}
			TimeSpan timeSpan = TimeSpan.Zero;
			foreach (DALProxyStats dalproxyStats in ctx.Stats.DALCalls)
			{
				int num;
				queryAggregate.DALCalls.TryGetValue(dalproxyStats.Procedure, out num);
				queryAggregate.DALCalls[dalproxyStats.Procedure] = num + 1;
				queryAggregate.DALCallsTotal++;
				timeSpan += dalproxyStats.DALTime;
			}
			queryAggregate.Executed++;
			if (!ctx.Stats.Succeeded)
			{
				queryAggregate.Failed++;
			}
			queryAggregate.ProcessingTime.apply(ctx.Stats.ProcessingTime);
			queryAggregate.DALTime.apply(timeSpan);
		}

		// Token: 0x060025FF RID: 9727 RVA: 0x0009FBF4 File Offset: 0x0009DFF4
		public void AddToAggregates(OnlineQueryStats stats)
		{
			QueryAggregate queryAggregate;
			if (!this.m_queryAggregates.TryGetValue(stats.Tag, out queryAggregate))
			{
				queryAggregate = new QueryAggregate();
				this.m_queryAggregates.Add(stats.Tag, queryAggregate);
			}
			if (stats.Type == QueryType.Incoming_Request)
			{
				queryAggregate.ServicingTime.apply(stats.ServicingTime);
			}
			QueryAggregate queryAggregate2 = queryAggregate;
			queryAggregate2.Download.TotalData = queryAggregate2.Download.TotalData + stats.InboundDataSize;
			QueryAggregate queryAggregate3 = queryAggregate;
			queryAggregate3.Download.CompressedData = queryAggregate3.Download.CompressedData + stats.InboundCompressedSize;
			QueryAggregate queryAggregate4 = queryAggregate;
			queryAggregate4.Upload.TotalData = queryAggregate4.Upload.TotalData + stats.OutboundDataSize;
			QueryAggregate queryAggregate5 = queryAggregate;
			queryAggregate5.Upload.CompressedData = queryAggregate5.Upload.CompressedData + stats.OutboundCompressedSize;
		}

		// Token: 0x06002600 RID: 9728 RVA: 0x0009FCB0 File Offset: 0x0009E0B0
		public Dictionary<string, QueryAggregate> SwapAggregates()
		{
			Dictionary<string, QueryAggregate> queryAggregates = this.m_queryAggregates;
			this.m_queryAggregates = new Dictionary<string, QueryAggregate>();
			return queryAggregates;
		}

		// Token: 0x04001375 RID: 4981
		private Dictionary<string, QueryAggregate> m_queryAggregates = new Dictionary<string, QueryAggregate>();
	}
}
