using System;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000172 RID: 370
	internal static class CryOnlineExtensions
	{
		// Token: 0x060006A8 RID: 1704 RVA: 0x0001A74C File Offset: 0x00018B4C
		public static SOnlineQuery Clone(this SOnlineQuery query)
		{
			return new SOnlineQuery
			{
				id = query.id,
				sId = query.sId,
				online_id = query.online_id,
				tag = query.tag,
				type = query.type
			};
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x0001A79C File Offset: 0x00018B9C
		public static SQueryError Clone(this SQueryError error)
		{
			return new SQueryError
			{
				id = error.id,
				sId = error.sId,
				online_id = error.online_id,
				description = error.description,
				online_error = error.online_error,
				custom_code = error.custom_code
			};
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x0001A7F8 File Offset: 0x00018BF8
		public static SQueryError QueryError(this SOnlineQuery query, int errorCode)
		{
			return query.QueryError(CryOnlineQueryBinder.ErrorCode(errorCode), CryOnlineQueryBinder.ErrorMsg(errorCode), errorCode);
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x0001A80D File Offset: 0x00018C0D
		public static SQueryError QueryError(this SOnlineQuery query, EOnlineError error, int customCode)
		{
			return query.QueryError(error, CryOnlineQueryBinder.ErrorMsg((int)error), customCode);
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x0001A820 File Offset: 0x00018C20
		public static SQueryError QueryError(this SOnlineQuery query, EOnlineError err, string msg, int customCode)
		{
			return new SQueryError
			{
				id = query.id,
				sId = query.sId,
				online_id = query.online_id,
				online_error = err,
				description = msg,
				custom_code = customCode
			};
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x0001A870 File Offset: 0x00018C70
		public static OnlineQueryStats OnlineQueryStats(this SQueryStats stats)
		{
			return new OnlineQueryStats
			{
				Tag = stats.Query,
				Type = ((!stats.Request) ? QueryType.Incoming_Request : QueryType.Outgoing_Request),
				Succeeded = !stats.Failed,
				InboundCompressedSize = stats.InboundCompressedSize,
				InboundDataSize = stats.InboundDataSize,
				OutboundCompressedSize = stats.OutboundCompressedSize,
				OutboundDataSize = stats.OutboundDataSize,
				ServicingTime = TimeSpan.FromMilliseconds(stats.ResponseTime)
			};
		}
	}
}
