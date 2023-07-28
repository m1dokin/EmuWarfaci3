using System;
using System.Collections.Generic;
using System.Reflection;
using HK2Net;
using MasterServer.CryOnlineNET;
using Util.Common;

namespace MasterServer.Core.Kernel.Scanners
{
	// Token: 0x0200011E RID: 286
	internal class QueryHandlerScanner : IScanner
	{
		// Token: 0x06000487 RID: 1159 RVA: 0x00013C9C File Offset: 0x0001209C
		public ScanResult Scan()
		{
			ScanResult scanResult = new ScanResult();
			this.ScanForQueries(scanResult);
			return scanResult;
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x00013CB8 File Offset: 0x000120B8
		private void ScanForQueries(ScanResult res)
		{
			ContractInfo item = new ContractInfo
			{
				ContractType = typeof(QueryHandler)
			};
			res.Contracts.Add(item);
			foreach (KeyValuePair<QueryAttributes, Type> keyValuePair in ReflectionUtils.GetTypesByAttribute<QueryAttributes>(Assembly.GetExecutingAssembly()))
			{
				Type value = keyValuePair.Value;
				if (Resources.DebugQueriesEnabled || value.GetCustomAttributes(typeof(DebugQueryAttribute), false).Length == 0)
				{
					ServiceInfo item2 = new ServiceInfo
					{
						ServiceType = value,
						Contracts = new List<ContractInfo>
						{
							item
						},
						ScopeType = typeof(SingletonScope)
					};
					res.Services.Add(item2);
				}
			}
		}
	}
}
