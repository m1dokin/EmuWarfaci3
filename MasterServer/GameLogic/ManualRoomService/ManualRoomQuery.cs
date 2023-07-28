using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.ManualRoomService
{
	// Token: 0x02000065 RID: 101
	internal abstract class ManualRoomQuery : BaseQuery
	{
		// Token: 0x0600017F RID: 383 RVA: 0x000044DC File Offset: 0x000028DC
		protected ManualRoomQuery(IManualRoomService manualRoomService)
		{
			this.m_manualRoomService = manualRoomService;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x000044EC File Offset: 0x000028EC
		public override int QueryGetResponse(string from, XmlElement request, XmlElement response)
		{
			if (!base.IsQueryFromMs(from))
			{
				return -1;
			}
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, this.GetQueryName()))
			{
				string value = string.Empty;
				try
				{
					value = this.ActivateRequest(request);
				}
				catch (Exception ex)
				{
					value = ex.Message;
					Log.Error(ex);
				}
				response.SetAttribute("result", value);
				result = 0;
			}
			return result;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00004578 File Offset: 0x00002978
		public override object HandleResponse(SOnlineQuery query, XmlElement response)
		{
			return response.GetAttribute("result");
		}

		// Token: 0x06000182 RID: 386
		protected abstract string ActivateRequest(XmlElement request);

		// Token: 0x06000183 RID: 387
		protected abstract string GetQueryName();

		// Token: 0x040000B4 RID: 180
		protected readonly IManualRoomService m_manualRoomService;
	}
}
