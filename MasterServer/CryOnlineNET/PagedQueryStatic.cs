using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x0200018D RID: 397
	internal abstract class PagedQueryStatic : PagedQuery
	{
		// Token: 0x06000735 RID: 1845
		protected abstract int GetMaxBatch();

		// Token: 0x06000736 RID: 1846
		protected abstract string GetDataHash();

		// Token: 0x06000737 RID: 1847
		protected abstract List<XmlElement> GetData(XmlDocument doc);

		// Token: 0x06000738 RID: 1848 RVA: 0x0001BF28 File Offset: 0x0001A328
		protected override PagedQuery.PagedResponse HandlePageRequest(SOnlineQuery query, PagedQuery.RequestParams prms, XmlElement request, XmlElement response)
		{
			string text;
			List<XmlElement> cachedData = this.GetCachedData(out text);
			PagedQuery.PagedResponse pagedResponse = base.ValidatePagedRequest(prms, text, this.GetMaxBatch());
			if (pagedResponse != null)
			{
				return pagedResponse;
			}
			int val = cachedData.Count - prms.From;
			int num = Math.Min(val, prms.To - prms.From);
			IEnumerable<XmlElement> enumerable = cachedData.Skip(prms.From).Take(num);
			foreach (XmlElement node in enumerable)
			{
				XmlNode newChild = response.OwnerDocument.ImportNode(node, true);
				response.AppendChild(newChild);
			}
			return new PagedQuery.PagedResponse
			{
				Code = ((prms.To >= cachedData.Count) ? PagedQuery.EResponseCode.Done : PagedQuery.EResponseCode.HasMore),
				From = prms.From,
				To = prms.From + num,
				Hash = text
			};
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x0001C03C File Offset: 0x0001A43C
		private List<XmlElement> GetCachedData(out string hash)
		{
			hash = this.GetDataHash();
			if (hash == this.m_cahedXmlHash && this.m_cachedXml != null)
			{
				return this.m_cachedXml;
			}
			object @lock = this.m_lock;
			List<XmlElement> cachedXml;
			lock (@lock)
			{
				hash = this.GetDataHash();
				if (hash == this.m_cahedXmlHash && this.m_cachedXml != null)
				{
					cachedXml = this.m_cachedXml;
				}
				else
				{
					XmlDocument doc = new XmlDocument();
					List<XmlElement> data = this.GetData(doc);
					Interlocked.Exchange<string>(ref this.m_cahedXmlHash, hash);
					Interlocked.Exchange<List<XmlElement>>(ref this.m_cachedXml, data);
					cachedXml = this.m_cachedXml;
				}
			}
			return cachedXml;
		}

		// Token: 0x04000454 RID: 1108
		private string m_cahedXmlHash;

		// Token: 0x04000455 RID: 1109
		private List<XmlElement> m_cachedXml;

		// Token: 0x04000456 RID: 1110
		private readonly object m_lock = new object();
	}
}
