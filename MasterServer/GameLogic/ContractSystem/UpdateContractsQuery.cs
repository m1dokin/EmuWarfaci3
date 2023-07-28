using System;
using System.Collections;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.ContractSystem
{
	// Token: 0x0200029E RID: 670
	[QueryAttributes(TagName = "update_contracts")]
	internal class UpdateContractsQuery : BaseQuery
	{
		// Token: 0x06000E6D RID: 3693 RVA: 0x0003A228 File Offset: 0x00038628
		public UpdateContractsQuery(IContractService contractService, ISessionStorage sessionStorage)
		{
			this.m_contractService = contractService;
			this.m_sessionStorage = sessionStorage;
		}

		// Token: 0x06000E6E RID: 3694 RVA: 0x0003A240 File Offset: 0x00038640
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			string attribute = request.GetAttribute("session_id");
			if (!this.m_sessionStorage.ValidateSession(fromJid, attribute))
			{
				Log.Warning<string, string>("Ignoring contract update request from server {0} which has incorrect session id {1}", fromJid, attribute);
				return -1;
			}
			bool flag = int.Parse(request.GetAttribute("session_end")) > 0;
			SessionContracts data = this.m_sessionStorage.GetData<SessionContracts>(attribute, ESessionData.Contracts);
			if (data != null)
			{
				object obj = data;
				lock (obj)
				{
					IEnumerator enumerator = request.ChildNodes.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object obj2 = enumerator.Current;
							XmlNode xmlNode = (XmlNode)obj2;
							if (!(xmlNode.Name != "contract"))
							{
								XmlElement xmlElement = xmlNode as XmlElement;
								ulong key = ulong.Parse(xmlElement.GetAttribute("profile_id"));
								uint num = uint.Parse(xmlElement.GetAttribute("progress"));
								if (num > 0U)
								{
									SessionContracts.SessionInfo sessionInfo = data.Contracts[key];
									this.m_contractService.SetContractProgress(sessionInfo.Contract, sessionInfo.ContractProfileItem, num, sessionInfo.Multiplier);
								}
								SessionContracts.SessionInfo sessionInfo2;
								data.Contracts.TryRemove(key, out sessionInfo2);
								if (flag && data.Contracts.Count == 0)
								{
									this.m_sessionStorage.RemoveData(attribute, ESessionData.Contracts);
								}
							}
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x040006A8 RID: 1704
		private readonly IContractService m_contractService;

		// Token: 0x040006A9 RID: 1705
		private readonly ISessionStorage m_sessionStorage;
	}
}
