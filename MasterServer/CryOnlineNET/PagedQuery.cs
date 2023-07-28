using System;
using System.Xml;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000189 RID: 393
	internal abstract class PagedQuery : BaseQuery
	{
		// Token: 0x0600072F RID: 1839 RVA: 0x0001BD08 File Offset: 0x0001A108
		public override int HandleRequest(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			if (base.IsBlocked)
			{
				return -5;
			}
			PagedQuery.RequestParams requestParams = new PagedQuery.RequestParams
			{
				From = ((!request.HasAttribute("from")) ? 0 : int.Parse(request.GetAttribute("from"))),
				To = ((!request.HasAttribute("to")) ? int.MaxValue : int.Parse(request.GetAttribute("to"))),
				RequestHash = request.GetAttribute("hash"),
				CachedHash = request.GetAttribute("cached")
			};
			if (requestParams.From < 0 || requestParams.To <= requestParams.From)
			{
				return -1;
			}
			PagedQuery.PagedResponse pagedResponse = this.HandlePageRequest(query, requestParams, request, response);
			string name = "code";
			int code = (int)pagedResponse.Code;
			response.SetAttribute(name, code.ToString());
			response.SetAttribute("from", pagedResponse.From.ToString());
			response.SetAttribute("to", pagedResponse.To.ToString());
			response.SetAttribute("hash", pagedResponse.Hash);
			if (pagedResponse.Code == PagedQuery.EResponseCode.RequestSequenceInterrupted)
			{
				Log.Error<string, string, string>("[PagedQuery:{0}] Hash mismatch on request sequence, server '{1}', request '{2}'", base.Tag, pagedResponse.Hash, requestParams.RequestHash);
				return -1;
			}
			return 0;
		}

		// Token: 0x06000730 RID: 1840 RVA: 0x0001BE68 File Offset: 0x0001A268
		protected PagedQuery.PagedResponse ValidatePagedRequest(PagedQuery.RequestParams prms, string serverHash, int maxSize)
		{
			if (!string.IsNullOrEmpty(prms.CachedHash) && prms.CachedHash.Equals(serverHash))
			{
				return new PagedQuery.PagedResponse
				{
					Code = PagedQuery.EResponseCode.NotModified,
					Hash = serverHash
				};
			}
			if (!string.IsNullOrEmpty(prms.RequestHash) && !prms.RequestHash.Equals(serverHash))
			{
				return new PagedQuery.PagedResponse
				{
					Code = PagedQuery.EResponseCode.RequestSequenceInterrupted,
					Hash = serverHash
				};
			}
			if (prms.To - prms.From > maxSize)
			{
				prms.To = prms.From + maxSize;
			}
			return null;
		}

		// Token: 0x06000731 RID: 1841
		protected abstract PagedQuery.PagedResponse HandlePageRequest(SOnlineQuery query, PagedQuery.RequestParams prms, XmlElement request, XmlElement response);

		// Token: 0x0200018A RID: 394
		protected enum EResponseCode
		{
			// Token: 0x04000448 RID: 1096
			RequestSequenceInterrupted,
			// Token: 0x04000449 RID: 1097
			NotModified,
			// Token: 0x0400044A RID: 1098
			HasMore,
			// Token: 0x0400044B RID: 1099
			Done
		}

		// Token: 0x0200018B RID: 395
		protected class RequestParams
		{
			// Token: 0x0400044C RID: 1100
			public int From;

			// Token: 0x0400044D RID: 1101
			public int To;

			// Token: 0x0400044E RID: 1102
			public string RequestHash;

			// Token: 0x0400044F RID: 1103
			public string CachedHash;
		}

		// Token: 0x0200018C RID: 396
		protected class PagedResponse
		{
			// Token: 0x04000450 RID: 1104
			public PagedQuery.EResponseCode Code;

			// Token: 0x04000451 RID: 1105
			public int From;

			// Token: 0x04000452 RID: 1106
			public int To;

			// Token: 0x04000453 RID: 1107
			public string Hash;
		}
	}
}
