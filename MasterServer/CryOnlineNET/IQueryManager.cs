using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using HK2Net;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000194 RID: 404
	[Contract]
	internal interface IQueryManager
	{
		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000767 RID: 1895
		// (set) Token: 0x06000768 RID: 1896
		int QueryExtraDelay { get; set; }

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000769 RID: 1897
		QueryDelay RequestDelays { get; }

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x0600076A RID: 1898
		QueryDelay ResponseDelays { get; }

		// Token: 0x0600076B RID: 1899
		void Response(QueryHandler handler, SOnlineQuery query, XmlElement response);

		// Token: 0x0600076C RID: 1900
		void ResponseError(QueryHandler handler, SOnlineQuery query, int code, XmlElement request);

		// Token: 0x0600076D RID: 1901
		void Request(string tag, string receiver, params object[] args);

		// Token: 0x0600076E RID: 1902
		Task<object> RequestAsync(string tag, string receiver, params object[] args);

		// Token: 0x14000023 RID: 35
		// (add) Token: 0x0600076F RID: 1903
		// (remove) Token: 0x06000770 RID: 1904
		event Action<QueryContext> QueryCompleted;

		// Token: 0x06000771 RID: 1905
		void BroadcastRequest(string tag, List<string> recievers, params object[] args);

		// Token: 0x06000772 RID: 1906
		void BroadcastRequest(string tag, string node, List<string> recievers, params object[] args);

		// Token: 0x06000773 RID: 1907
		IEnumerable<string> GetQueries();

		// Token: 0x06000774 RID: 1908
		IEnumerable<string> GetQueries(string subString);

		// Token: 0x06000775 RID: 1909
		void UpdateQueryBlockingFlags(IEnumerable<string> tags, bool isBlocked, EOnlineError errorCode);

		// Token: 0x06000776 RID: 1910
		void DumpQueryBlockingFlags();

		// Token: 0x06000777 RID: 1911
		void DumpQueryBlockingFlags(IEnumerable<string> tags);
	}
}
