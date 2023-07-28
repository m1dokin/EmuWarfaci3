using System;
using System.Threading.Tasks;
using System.Xml;
using DedicatedPoolServer.Model;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using NLog;
using Util.Common;

namespace MasterServer.ServerInfo.Queries
{
	// Token: 0x020006BE RID: 1726
	[QueryAttributes(TagName = "debug_lock_server")]
	internal class DebugLockServerQuery : BaseQuery
	{
		// Token: 0x06002428 RID: 9256 RVA: 0x0009728E File Offset: 0x0009568E
		public DebugLockServerQuery(IServerInfo serverInfo)
		{
			this.m_serverInfo = serverInfo;
		}

		// Token: 0x06002429 RID: 9257 RVA: 0x000972A0 File Offset: 0x000956A0
		public override Task<int> HandleRequestAsync(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			if (!Resources.DebugQueriesEnabled)
			{
				return TaskHelpers.Completed<int>(-1);
			}
			string attribute = request.GetAttribute("build_type");
			string attribute2 = request.GetAttribute("mode");
			string attribute3 = request.GetAttribute("region_id");
			if (attribute2 != null)
			{
				DedicatedMode mode;
				if (!(attribute2 == "pvp"))
				{
					if (!(attribute2 == "pve"))
					{
						goto IL_7D;
					}
					mode = DedicatedMode.PVP_PVE;
				}
				else
				{
					mode = DedicatedMode.PurePVP;
				}
				return this.m_serverInfo.RequestServer(mode, "--" + attribute.ToLower(), attribute3).ContinueWith<int>(delegate(Task<DedicatedInfo> t)
				{
					int result;
					try
					{
						if (t.IsFaulted || t.Result == null)
						{
							result = -3;
						}
						else
						{
							response.SetAttribute("result", t.Result.DedicatedId);
							result = 0;
						}
					}
					catch (Exception value)
					{
						DebugLockServerQuery.Logger.Error<Exception>(value);
						result = -1;
					}
					return result;
				});
			}
			IL_7D:
			return TaskHelpers.Completed<int>(-2);
		}

		// Token: 0x04001220 RID: 4640
		public const string QueryName = "debug_lock_server";

		// Token: 0x04001221 RID: 4641
		private const int DlsInvalidMode = -2;

		// Token: 0x04001222 RID: 4642
		private const int DlsDedicatedNotFound = -3;

		// Token: 0x04001223 RID: 4643
		private const string BuildTypeAttributeName = "build_type";

		// Token: 0x04001224 RID: 4644
		private const string ModeAttributeName = "mode";

		// Token: 0x04001225 RID: 4645
		private const string RegionIdAttibuteName = "region_id";

		// Token: 0x04001226 RID: 4646
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		// Token: 0x04001227 RID: 4647
		private readonly IServerInfo m_serverInfo;
	}
}
