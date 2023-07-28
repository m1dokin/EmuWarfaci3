using System;
using System.Threading;
using System.Xml;
using MasterServer.Core;
using MasterServer.Users;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000173 RID: 371
	internal abstract class DeferredQuery : BaseQuery
	{
		// Token: 0x060006AF RID: 1711 RVA: 0x0001A904 File Offset: 0x00018D04
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			object state = null;
			int num = this.OnDeferredRequest(fromJid, request, response, out state);
			if (num != 0)
			{
				return num;
			}
			IQoSQueue service = ServicesManager.GetService<IQoSQueue>();
			TShapingInfo shaping_info = new TShapingInfo
			{
				query_name = base.Tag,
				from_jid = fromJid,
				query_class = base.QoSClass
			};
			if (!service.QueueWorkItem(shaping_info, new WaitCallback(this.DispatchRequest), new DeferredQuery.DeferredResponse(fromJid, state)))
			{
				return -2;
			}
			return 0;
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x0001A97C File Offset: 0x00018D7C
		private void DispatchRequest(object dr)
		{
			try
			{
				DeferredQuery.DeferredResponse deferredResponse = (DeferredQuery.DeferredResponse)dr;
				if (UserInfo.IsServerJid(deferredResponse.To))
				{
					if (!base.ServerRepository.IsOnline(deferredResponse.To))
					{
						return;
					}
				}
				else if (!base.UserRepository.IsOnline(deferredResponse.To))
				{
					return;
				}
				QueryManager.RequestSt(base.Tag, deferredResponse.To, new object[]
				{
					deferredResponse.To,
					deferredResponse.State
				});
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x0001AA28 File Offset: 0x00018E28
		public override void SendRequest(string online_id, XmlElement parentElement, params object[] args)
		{
			string to = args[0].ToString();
			object state = args[1];
			int num = this.ComposeDeferredResult(to, parentElement, state);
			if (num != 0)
			{
				parentElement.SetAttribute("deferred_error", ((int)CryOnlineQueryBinder.ErrorCode(num)).ToString());
				parentElement.SetAttribute("deferred_error_msg", CryOnlineQueryBinder.ErrorMsg(num));
				parentElement.SetAttribute("deferred_error_custom", num.ToString());
			}
		}

		// Token: 0x060006B2 RID: 1714
		protected abstract int OnDeferredRequest(string fromJid, XmlElement request, XmlElement response, out object state);

		// Token: 0x060006B3 RID: 1715
		protected abstract int ComposeDeferredResult(string to, XmlElement resut, object state);

		// Token: 0x02000174 RID: 372
		private class DeferredResponse
		{
			// Token: 0x060006B4 RID: 1716 RVA: 0x0001AA9A File Offset: 0x00018E9A
			public DeferredResponse(string to, object state)
			{
				this.To = to;
				this.State = state;
			}

			// Token: 0x04000415 RID: 1045
			public string To;

			// Token: 0x04000416 RID: 1046
			public object State;
		}
	}
}
