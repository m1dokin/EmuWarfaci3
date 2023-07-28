using System;
using System.Collections.Generic;
using MasterServer.Database;

namespace MasterServer.Telemetry.Aggregaton
{
	// Token: 0x0200072A RID: 1834
	internal class ProcedureAggregateStorage
	{
		// Token: 0x06002603 RID: 9731 RVA: 0x0009FCEC File Offset: 0x0009E0EC
		public void AddToAggregates(IEnumerable<DALProxyStats> dal_calls)
		{
			foreach (DALProxyStats dalproxyStats in dal_calls)
			{
				DALProcedureAggregate dalprocedureAggregate;
				if (!this.m_procedureAggregates.TryGetValue(dalproxyStats.Procedure, out dalprocedureAggregate))
				{
					dalprocedureAggregate = new DALProcedureAggregate();
					this.m_procedureAggregates.Add(dalproxyStats.Procedure, dalprocedureAggregate);
				}
				dalprocedureAggregate.Executed++;
				dalprocedureAggregate.CacheTime.apply(dalproxyStats.CacheTime);
				dalprocedureAggregate.L1CacheHits += dalproxyStats.L1CacheHits;
				dalprocedureAggregate.L1CacheMisses += dalproxyStats.L1CacheMisses;
				dalprocedureAggregate.L1CacheClear += dalproxyStats.L1CacheClear;
				dalprocedureAggregate.L2CacheHits += dalproxyStats.L2CacheHits;
				dalprocedureAggregate.L2CacheMisses += dalproxyStats.L2CacheMisses;
				dalprocedureAggregate.L2CacheClear += dalproxyStats.L2CacheClear;
				dalprocedureAggregate.DatabaseQueries += dalproxyStats.DBQueries;
				dalprocedureAggregate.ConnectingTime.apply(dalproxyStats.ConnectionAllocTime);
				dalprocedureAggregate.DatabaseTime.apply(dalproxyStats.DBTime);
			}
		}

		// Token: 0x06002604 RID: 9732 RVA: 0x0009FE3C File Offset: 0x0009E23C
		public Dictionary<string, DALProcedureAggregate> SwapAggregates()
		{
			Dictionary<string, DALProcedureAggregate> procedureAggregates = this.m_procedureAggregates;
			this.m_procedureAggregates = new Dictionary<string, DALProcedureAggregate>();
			return procedureAggregates;
		}

		// Token: 0x04001381 RID: 4993
		private Dictionary<string, DALProcedureAggregate> m_procedureAggregates = new Dictionary<string, DALProcedureAggregate>();
	}
}
